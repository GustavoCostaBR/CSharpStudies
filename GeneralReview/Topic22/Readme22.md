# Topic 22: Memory and Resource Management in C#

## Introduction

Effective memory and resource management is central to building performant, reliable, and scalable .NET applications. While the .NET Garbage Collector (GC) automatically reclaims managed memory, developers remain responsible for *deterministically releasing* non-memory resources (file handles, sockets, database connections, OS handles) and for writing allocation-conscious code. Understanding the GC model (generations, collections, large object heap), the `IDisposable` / `IAsyncDisposable` patterns, and advanced constructs like weak references equips you to avoid leaks, minimize pauses, and ensure graceful resource usage across varied workloads.

## Core Concepts

### Key Terms
- **Managed Memory**: Memory allocated on the managed heap; reclaimed automatically by GC.
- **Garbage Collector (GC)**: Runtime subsystem that identifies and reclaims memory no longer reachable by live references, compacting the heap (for most segments) and updating references.
- **Generational GC**: Optimizes collection by grouping objects by age (Gen 0, 1, 2). Younger objects die more often; collecting them is cheaper.
- **Large Object Heap (LOH)**: Separate heap for large allocations (≥ 85,000 bytes) to reduce copying costs. Historically not compacted every cycle.
- **Finalizer** (`~TypeName()`): A destructor-like method executed before the GC reclaims an object, if registered. Non-deterministic timing.
- **IDisposable**: Interface exposing `Dispose()` for deterministic release of unmanaged or scarce resources.
- **IAsyncDisposable**: Async variant providing `ValueTask DisposeAsync()` for resources requiring asynchronous cleanup (e.g., async flushing, network operations).
- **`using` Statement / Declaration**: Language construct ensuring `Dispose`/`DisposeAsync` is called automatically (scope-based cleanup).
- **Implicit Release**: GC reclaiming managed memory when no references remain.
- **Explicit Release**: Developer-initiated resource cleanup via `Dispose`, `Close`, or releasing handles/pointers.
- **WeakReference**: Reference that does not prevent its target from being collected.
- **Pinned Object**: Object fixed in memory (e.g., via `fixed` statement) preventing compaction moves.

### Why It Matters
- Prevent **resource leaks** (file handle exhaustion, socket leaks).
- Reduce **GC pressure** (fewer allocations → fewer collections → improved throughput/latency).
- Avoid **finalizer delays** & **promotion of short-lived objects** causing memory bloat.
- Enable **predictable performance** in high-throughput, low-latency services.

## Syntax & Usage

### 1. Garbage Collector (Basic Syntax & Example)
```csharp
// Basic allocation and forcing a collection (FOR DEMO ONLY — avoid forcing GC in production)
var list = new List<byte[]>();
for (int i = 0; i < 10; i++)
{
    list.Add(new byte[1024 * 100]); // allocate ~100 KB chunks
}
Console.WriteLine($"Allocated {list.Count} arrays");

// Drop references
list.Clear();

// Hint GC (not guaranteed immediate reclaim, but often triggers)
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect(); // Second collect to reclaim objects resurrected by finalizers (rare case)
```
Step-by-step:
1. Allocate several arrays increasing Gen 0 allocations.
2. Clear strong references.
3. Force collection (`GC.Collect`) for demonstration—normally *avoid* manual collection; the GC is self-tuning.

### 2. Implicit vs Explicit Release
```csharp
// Implicit: ordinary objects become unreachable and are collected automatically.
class PlainData { public int Value; }

// Explicit: resource wrapper implementing IDisposable.
class FileHolder : IDisposable
{
    private readonly FileStream _stream;
    public FileHolder(string path) => _stream = File.OpenRead(path);
    public int ReadByte() => _stream.ReadByte();
    public void Dispose() => _stream.Dispose(); // explicit release of OS handle
}

var data = new PlainData { Value = 123 }; // implicitly reclaimed later
using (var f = new FileHolder("sample.txt"))
{
    Console.WriteLine(f.ReadByte());
} // Dispose called here explicitly (deterministic)
```
Explanation: Managed memory (PlainData) is implicit; OS file handle inside `FileHolder` is explicitly released at scope end.

### 3. Interface `IDisposable` (Pattern)
```csharp
public class NativeBuffer : IDisposable
{
    private IntPtr _ptr;
    private bool _disposed;

    public NativeBuffer(int size)
    {
        _ptr = Marshal.AllocHGlobal(size); // unmanaged allocation
    }

    public void Use() { if (_disposed) throw new ObjectDisposedException(nameof(NativeBuffer)); /* operate */ }

    // Public dispose entry point
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this); // No need to run finalizer now
    }

    // Protected virtual pattern (support inheritance)
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (_ptr != IntPtr.Zero)
        {
            Marshal.FreeHGlobal(_ptr);
            _ptr = IntPtr.Zero;
        }
        _disposed = true;
    }

    ~NativeBuffer() => Dispose(disposing: false); // finalizer as safety net
}
```
Key points:
- `Dispose()` calls core cleanup and suppresses finalizer.
- Finalizer frees unmanaged memory if `Dispose` not called.
- `disposing` flag would allow disposing of managed fields (if any) only when explicitly called.

#### Deeper Explanation of the `NativeBuffer` Example
Purpose: Show the *full* disposable pattern when you directly allocate unmanaged memory (memory the GC does not track).

Line-by-line rationale:
1. `IntPtr _ptr;` is just an integer-sized holder for a native (unmanaged) memory address. It is opaque—C# cannot automatically free what it points to.
2. `Marshal.AllocHGlobal(size)` asks the OS for a raw memory block (roughly like `malloc` in C). The GC knows nothing about this allocation, so if you forget to free it you leak native memory.
3. If we instead created `new byte[size]` we would get *managed* memory, automatically reclaimed by the GC—no `Dispose` needed. We purposely use unmanaged memory to illustrate why `IDisposable` + finalizer can be required.
4. `Dispose()` is the public method callers use for deterministic cleanup. It calls the protected `Dispose(bool disposing)` overload then suppresses the finalizer so the object skips the (slower) finalization queue.
5. `Dispose(bool disposing)` pattern: if `disposing == true` you are being called from the explicit `Dispose()` path; this is where you may also dispose other managed `IDisposable` fields you own. If `false` you are in the finalizer, and should *not* touch other managed objects (they may already be collected or in an indeterminate state). In this specific sample we only manage unmanaged memory, so the branch is minimal.
6. The guard `if (_disposed) return;` makes disposal idempotent (safe to call multiple times).
7. Setting `_ptr = IntPtr.Zero` after freeing is defensive—prevents double free and makes debugging easier.
8. The finalizer `~NativeBuffer()` exists as a **safety net**. If a caller forgets to call `Dispose()`, the GC will eventually finalize the object and release the unmanaged memory. This is *non-deterministic* (could happen much later) and more expensive because finalizable objects survive at least one GC cycle (promotion). Hence: always dispose explicitly when possible.
9. `GC.SuppressFinalize(this)` removes the object from the finalization queue after successful manual cleanup, improving performance and memory turnover.

When do you really need this pattern?
- Only when you directly handle unmanaged resources (native memory, OS handles, file descriptors) *or* you wrap another type that itself needs deterministic cleanup and you add extra unmanaged responsibility.
- If you only aggregate other managed `IDisposable` objects, you often do **not** need a finalizer—just implement `IDisposable` and forward calls.

Common mistakes beginners make:
- Adding a finalizer unnecessarily (adds GC overhead).
- Forgetting `GC.SuppressFinalize` (forces an unneeded finalizer run).
- Omitting the idempotent guard, risking double free.
- Performing managed cleanup inside the finalizer path (`disposing: false`).

Simpler alternative (if unmanaged memory is **not** required):
```csharp
// Managed buffer—no finalizer, simpler pattern
public sealed class ManagedBuffer : IDisposable
{
    private byte[]? _buffer;
    public ManagedBuffer(int size) => _buffer = new byte[size];
    public void Dispose() => _buffer = null; // Let GC reclaim array
}
```
Why not always use unmanaged memory? Because managed arrays give you: bounds checking, automatic lifetime management, zero manual free logic, and better integration with modern APIs (`Span<byte>` over a managed array). You drop to unmanaged only for interoperability (P/Invoke), very large pinned scenarios, or specialized performance cases.

### 4. Using an `IDisposable` (`using` patterns)
```csharp
// Classic using statement
using (var buffer = new NativeBuffer(1024))
{
    buffer.Use();
}

// C# 8+ using declaration (disposes at end of scope automatically)
using var buf2 = new NativeBuffer(2048);
buf2.Use();
```
Explanation: Both ensure deterministic cleanup even if exceptions occur inside the scope.

### 5. Weak References (Optional)
```csharp
var heavy = new byte[10_000_000]; // 10 MB
var weak = new WeakReference<byte[]>(heavy);
Console.WriteLine(weak.TryGetTarget(out _)); // True
heavy = null; // Drop strong reference
GC.Collect();
Console.WriteLine(weak.TryGetTarget(out var arr) ? $"Alive size={arr.Length}" : "Collected");
```
Use case: Caching where re-computation is cheap; weak references allow GC to reclaim memory under pressure.

### 6. Generations (Optional Introspection)
```csharp
object o = new object();
Console.WriteLine(GC.GetGeneration(o)); // Likely 0
GC.Collect();
Console.WriteLine(GC.GetGeneration(o)); // May be promoted to Gen 1 or 2 depending on timing
```
Generations reduce collection overhead by focusing on young objects frequently; survivors move to older generations.

## Practical Example: Buffered File Processing with Deterministic & Conditional Resource Management
Scenario: Process a large log file line-by-line, caching parsed results weakly to avoid re-parsing while not preventing GC reclamation.
```csharp
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Disposable wrapper for file read operations with an unmanaged buffer
public sealed class LogReader : IDisposable
{
    private FileStream _stream;
    private readonly byte[] _buffer;
    private bool _disposed;

    public LogReader(string path, int bufferSize = 32 * 1024)
    {
        _stream = File.OpenRead(path);
        _buffer = ArrayPool<byte>.Shared.Rent(bufferSize); // managed pooling reduces allocations
    }

    public IEnumerable<string> ReadLines()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(LogReader));
        using var sr = new StreamReader(_stream, leaveOpen: true);
        string? line;
        while ((line = sr.ReadLine()) is not null)
            yield return line;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _stream.Dispose();
        ArrayPool<byte>.Shared.Return(_buffer);
        _disposed = true;
    }
}

// Cache parsed results; entries may be reclaimed under memory pressure
public sealed class ParsedLineCache
{
    private readonly ConcurrentDictionary<int, WeakReference<ParsedLine>> _cache = new();

    public ParsedLine GetOrAdd(int lineNo, string raw)
    {
        if (_cache.TryGetValue(lineNo, out var weak) && weak.TryGetTarget(out var existing))
            return existing; // reuse cached parsed object if still alive

        var parsed = ParsedLine.Parse(raw);
        _cache[lineNo] = new WeakReference<ParsedLine>(parsed);
        return parsed;
    }
}

public sealed class ParsedLine
{
    public DateTime Timestamp { get; }
    public string Level { get; }
    public string Message { get; }

    private ParsedLine(DateTime ts, string level, string msg)
        => (Timestamp, Level, Message) = (ts, level, msg);

    public static ParsedLine Parse(string raw)
    {
        // naive parsing: "2025-08-15T12:34:56Z|INFO|Something happened"
        var parts = raw.Split('|');
        return new ParsedLine(DateTime.Parse(parts[0]), parts[1], parts[2]);
    }
}

// Orchestration
public static class LogProcessor
{
    public static IEnumerable<ParsedLine> ReadAndParse(string path)
    {
        using var reader = new LogReader(path);
        var cache = new ParsedLineCache();
        int lineNo = 0;
        foreach (var line in reader.ReadLines())
        {
            var parsed = cache.GetOrAdd(lineNo++, line);
            yield return parsed; // consumer enumerates lazily
        }
    }
}

// Usage Example
foreach (var item in LogProcessor.ReadAndParse("application.log"))
{
    // Do something with parsed lines
    if (item.Level == "ERROR")
        Console.WriteLine($"[{item.Timestamp:u}] {item.Message}");
}
```
Why:
1. **Deterministic Disposal**: `LogReader` ensures stream & rented buffer returned promptly.
2. **Array Pooling**: Reduces per-read allocations, lowering GC pressure.
3. **WeakReference Cache**: Allows parsed objects to be reclaimed if memory tight; avoids strong memory growth.
4. **Lazy Enumeration**: Pull-based consumption avoids reading whole file into memory.
5. **Separation of Concerns**: Parsing, caching, and orchestrating responsibilities isolated for clarity.

## Best Practices & Common Pitfalls

### Best Practices
- Implement `IDisposable` for any type owning unmanaged or scarce managed resources (streams, DbConnections, HttpClient handlers, timers, pooled buffers).
- Use `using` (or `await using` for async disposables) to enforce scope-based cleanup.
- Prefer **composition over finalizers**; only add a finalizer if directly owning unmanaged resources (or wrap via `SafeHandle`).
- Call `GC.SuppressFinalize(this)` after successful deterministic cleanup to avoid unnecessary finalization cost.
- Avoid forcing GC with `GC.Collect()`—let the runtime optimize collection scheduling.
- Minimize allocations in tight loops: reuse buffers (e.g., `ArrayPool<T>`), prefer `Span<T>/Memory<T>` over copying.
- Use **structs** for tiny, immutable value types to reduce heap traffic; but avoid large structs that cause copying overhead.
- Monitor with tooling: **dotnet-counters**, **dotnet-trace**, **PerfView**, **EventCounters** to identify allocation hot spots.
- For caching, consider eviction strategies (size limits) and use weak or conditional weak references only when recomputation is cheap.
- Guard against double disposal; make `Dispose` idempotent.

### Common Pitfalls
- Implementing a finalizer without unmanaged resources → unnecessary GC overhead (object promoted to finalizer queue).
- Forgetting to dispose `IDisposable` objects (can leak OS handles, delay file release, exhaust sockets).
- Catching all exceptions and swallowing them inside a `using` scope, hiding resource retention.
- Overusing weak references as a general caching strategy—may cause excessive churn & recomputation.
- Holding references longer than needed (e.g., static caches) increasing Gen 2 pressure and LOH fragmentation.
- Allocating large temporary arrays repeatedly rather than pooling—causing LOH growth.
- Using `Task.Result` / blocking inside `DisposeAsync` leading to deadlocks.

### When to Use Features vs Alternatives
- Use `IDisposable` vs finalizer: Prefer `IDisposable` for predictable release; finalizer only as safety net.
- Use `IAsyncDisposable` when cleanup involves async I/O (flush network streams, database transactions).
- Use WeakReference for optional caching; prefer strong references for core data.
- Use pooling (`ArrayPool`, object pools) when reusing instances decreases total allocations measurably.
- Use memory profiling to decide optimization; avoid premature micro-optimizations.

## Advanced Topics
- **IAsyncDisposable / await using**: Async cleanup pattern for network streams, `DbContext` (EF Core 7+), asynchronous flush operations.
- **SafeHandle**: Recommended wrapper for OS handles; implement `IDisposable` once and minimize finalizer complexity.
- **GC Modes**: Workstation vs Server GC; *Background GC* reduces pause times; configure via runtimeconfig or environment variables.
- **SustainedLowLatency / Latency Modes**: `GC.TryStartNoGCRegion` for critical low-latency windows (use cautiously; ensure capacity).
- **Large Object Heap Compaction**: .NET can compact LOH optionally (`GCSettings.LargeObjectHeapCompactionMode`).
- **Pinned Memory**: Excessive pinning prevents compaction leading to fragmentation—keep pin lifetimes short.
- **Generational Promotion Tuning**: Limit long-lived references; break object graphs when no longer needed to prevent promotion.
- **Span<T> & stackalloc**: Use stack allocations for small, short-lived buffers to reduce heap allocations.
- **ValueTask**: Reduces allocations in high-frequency async paths.
- **Source Generators**: Can reduce reflection-driven allocation (e.g., serialization code-gen) by producing static code.
- **Pooling Frameworks**: `Microsoft.Extensions.ObjectPool`, custom pools for reusable objects.

### Example: Async Disposable
```csharp
public sealed class AsyncLogWriter : IAsyncDisposable
{
    private readonly StreamWriter _writer;
    public AsyncLogWriter(string path) => _writer = new StreamWriter(File.OpenWrite(path));
    public ValueTask WriteAsync(string line) => _writer.WriteLineAsync(line);
    public async ValueTask DisposeAsync()
    {
        await _writer.FlushAsync();
        _writer.Dispose(); // underlying stream
    }
}

await using (var logger = new AsyncLogWriter("out.log"))
{
    await logger.WriteAsync("Application started");
}
```

### Example: SafeHandle Wrapper (Pattern Sketch)
```csharp
public sealed class MyHandle : SafeHandle
{
    public MyHandle() : base(IntPtr.Zero, ownsHandle: true) { }
    public override bool IsInvalid => handle == IntPtr.Zero;
    protected override bool ReleaseHandle()
    {
        // call native close API
        return true; // return success
    }
}
```
Why: Offloads finalization complexity to `SafeHandle`, simplifying high-level resource types.

## Summary

The .NET runtime relieves you from manual memory management but *not* from responsibility over non-memory resources and allocation discipline. Understand GC generations, minimize unnecessary allocations, and leverage deterministic disposal (`using`, `IDisposable`, `IAsyncDisposable`) to release resources promptly. Use weak references sparingly for optional caching, and avoid finalizers unless you directly manage unmanaged memory or wrap native handles (prefer `SafeHandle`). Adopt pooling and modern value-centric constructs (`Span`, `ArrayPool`) to reduce pressure. Practiced correctly, these techniques yield robust, performant, and maintainable applications with predictable resource behavior.
