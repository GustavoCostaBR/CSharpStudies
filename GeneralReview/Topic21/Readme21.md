# Topic 21: Assemblies, Attributes, and Conditional Compilation

## Introduction

Assemblies are the fundamental deployment, versioning, and security units in .NET. They package compiled IL (Intermediate Language) code plus metadata describing types, members, references, and resources. Attributes are declarative metadata elements you attach to code (types, methods, properties, assemblies) to convey *additional semantics* consumed by the runtime, tools, analyzers, serializers, ORMs, or your own reflection logic. Conditional compilation and compiler directives let you include or exclude code, control warnings, and adapt behavior per build configuration without scattering environment checks across your logic. Mastering these concepts gives you precision in distribution, runtime behavior, diagnostics, and cross‑environment builds.

## Core Concepts

### Assemblies
- **Assembly**: A compiled output (DLL or EXE) containing IL + metadata + manifest.
- **Manifest**: Metadata describing: assembly identity (name, version, culture, public key), referenced assemblies, files, exported types.
- **Module**: A single PE file inside an assembly (multi-module assemblies are rare today).
- **Metadata**: Tables describing types, methods, properties, attributes—enables reflection and tooling.
- **Strong Name**: Cryptographic identity (public/private key pair) ensuring assembly uniqueness & tamper detection.
- **Versioning**: Four-part version: `major.minor.build.revision` used in binding & compatibility decisions.
- **Assembly Load Context**: (Runtime concept) Determines how assemblies are resolved/isolated (important for plugins, .NET Core).
- **Satellite Assemblies**: Culture-specific resource assemblies for localization.

### Attributes
- **Attribute**: A class deriving from `System.Attribute` that can annotate program elements.
- **Targets**: Types, methods, parameters, return values, properties, fields, events, assemblies, modules.
- **Usage Control**: `[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]`.
- **Common Built-ins**:
  - `[Obsolete]` / `[Obsolete("message", true)]` – warn or error on usage.
  - `[Serializable]`, `[NonSerialized]` – legacy binary/XML serialization markers.
  - `[DataContract]`, `[DataMember]` – contract-based serialization.
  - `[Required]` (data annotations), `[Range]`, etc. – validation.
  - `[Conditional("DEBUG")]` – calls to the method are omitted unless symbol is defined.
  - `[CLSCompliant(true)]`, `[AssemblyMetadata]`, `[InternalsVisibleTo]`, `[AssemblyVersion]`.
- **Custom Attribute**: Domain-specific metadata your code can inspect via reflection.
- **Reflection**: Reading attributes at runtime using `GetCustomAttributes`, or compile-time via source generators/analyzers.

### Conditional Compilation & Compiler Directives
- **Preprocessor Symbols**: Logical flags you define (e.g., `DEBUG`, `TRACE`, `FEATURE_X`). Controlled via project file `<DefineConstants>` or `#define`.
- **Directives**:
  - `#if`, `#elif`, `#else`, `#endif` – include/exclude code.
  - `#define`, `#undef` – local symbol control (file scope).
  - `#error`, `#warning` – emit custom diagnostics.
  - `#nullable` – control nullable analysis context (`enable`, `disable`, `restore`).
  - `#pragma warning disable/restore` – selectively silence warnings.
  - `#line` – override file/line info (rarely needed).
- **`[Conditional]` Attribute vs `#if`**:
  - `#if` prevents code from compiling (removed at parse time).
  - `[Conditional]` removes *calls* to the decorated method; the method still compiles (can be referenced by reflection, etc.).

## Syntax & Usage

### Example 1: Defining and Using a Custom Attribute (Assemblies & Attributes)
```csharp
using System;
using System.Reflection;

// 1. Define a custom attribute to annotate domain services.
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class AuditAttribute : Attribute
{
    public string Action { get; }
    public string? Severity { get; init; }
    public AuditAttribute(string action) => Action = action;
}

// 2. Apply the attribute to a service class and method.
[Audit("UserService", Severity = "Info")]
public class UserService
{
    [Audit("CreateUser", Severity = "High")]
    public void Create(string username)
    {
        Console.WriteLine($"Creating user {username}");
    }
}

// 3. Reflection utility reading attributes.
public static class AuditScanner
{
    public static void PrintAuditMetadata<T>()
    {
        var type = typeof(T);
        var typeAudit = type.GetCustomAttribute<AuditAttribute>();
        if (typeAudit != null)
            Console.WriteLine($"Type Audit: Action={typeAudit.Action}, Severity={typeAudit.Severity}");

        foreach (var m in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
        {
            var audit = m.GetCustomAttribute<AuditAttribute>();
            if (audit != null)
                Console.WriteLine($" Method {m.Name}: Action={audit.Action}, Severity={audit.Severity}");
        }
    }
}

// 4. Usage
var service = new UserService();
service.Create("alice");
AuditScanner.PrintAuditMetadata<UserService>();
```
Step-by-step:
1. `[AttributeUsage]` restricts targets and multiplicity; constructor sets required metadata; `init` property for optional metadata.
2. Attributes applied to class and method.
3. Reflection enumerates metadata for runtime auditing/logging.
4. Execution shows domain logic and metadata extraction.

#### Simpler Custom Attribute Example (Enum Display Labels)
A very common and concrete benefit: attach user-friendly labels to enum values without a big `switch` statement.

```csharp
using System;
using System.Reflection;

// 1. Define a tiny attribute holding a display string.
[AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
public sealed class DisplayTextAttribute : Attribute
{
    public string Text { get; }
    public DisplayTextAttribute(string text) => Text = text;
}

// 2. Annotate enum values with readable labels.
public enum JobStatus
{
    [DisplayText("Pending Approval")] Pending,
    [DisplayText("In Progress") ]      Active,
    [DisplayText("Completed Successfully")] Completed,
    [DisplayText("Failed ❌")]          Failed
}

// 3. Extension method to fetch the label (reflection cached in a real system).
public static class JobStatusExtensions
{
    public static string Display(this JobStatus status)
    {
        var member = typeof(JobStatus).GetMember(status.ToString())[0];
        var attr = member.GetCustomAttribute<DisplayTextAttribute>();
        return attr?.Text ?? status.ToString();
    }
}

// 4. Usage demonstration.
foreach (var s in Enum.GetValues<JobStatus>())
    Console.WriteLine($"{s} => {s.Display()}");

/*
Expected Output:
Pending => Pending Approval
Active => In Progress
Completed => Completed Successfully
Failed => Failed ❌
*/
```
Why this helps:
- Eliminates repetitive `switch` / mapping dictionaries scattered across UI or API layers.
- Keeps the label *next to* the enum definition (single source of truth).
- Adding a new enum value only requires annotating it—extension method keeps working.
- Reflection cost is negligible for small enums; can be optimized (cache) if needed.

### Example 2: Conditional Compilation & `[Conditional]` vs `#if`
```csharp
#define FEATURE_VERBOSE
using System;
using System.Diagnostics; // For ConditionalAttribute

static class Log
{
    [Conditional("DEBUG")]
    public static void Debug(string message) => Console.WriteLine($"[DEBUG] {message}");

    [Conditional("FEATURE_VERBOSE")]
    public static void Verbose(string message) => Console.WriteLine($"[VERBOSE] {message}");
}

public class Runner
{
    public void Execute()
    {
#if DEBUG
        Console.WriteLine("Debug build extras initialized.");
#endif
        Log.Debug("Starting execution");  // Removed entirely in non-DEBUG builds
        Log.Verbose("Detailed path chosen"); // Removed if FEATURE_VERBOSE undefined
        Console.WriteLine("Core logic running.");
#if FEATURE_VERBOSE
        Console.WriteLine("Extra metrics collection active.");
#else
        Console.WriteLine("Minimal metrics mode.");
#endif
    }
}

new Runner().Execute();
```
Step-by-step:
1. `#define FEATURE_VERBOSE` sets a symbol (file-local). In projects prefer csproj `<DefineConstants>`.
2. `[Conditional("DEBUG")]` ensures calls compile only when `DEBUG` symbol defined.
3. `#if DEBUG` encloses code that is stripped out earlier in compilation stages.
4. `Log.Verbose` call is omitted if symbol not defined – reduces overhead without manual condition checks.
5. Compile under different configurations to compare outputs.

## Practical Example: Library with Attributes and Build-Time Variations

Scenario: A plugin-based telemetry library where:
- Assembly-level attributes declare metadata (version, description).
- Custom `MetricAttribute` marks methods exporting metrics.
- Conditional compilation enables lightweight vs. full instrumentation builds.

```csharp
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

// Assembly-level attributes (normally placed in AssemblyInfo.cs or top of a file)
[assembly: AssemblyMetadata("Repo", "https://example.com/project")]
[assembly: AssemblyMetadata("Module", "TelemetryCore")]

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class MetricAttribute : Attribute
{
    public string Name { get; }
    public string? Unit { get; init; }
    public MetricAttribute(string name) => Name = name;
}

public interface ICollector
{
    void Emit(string name, double value, string? unit = null);
}

public sealed class ConsoleCollector : ICollector
{
    public void Emit(string name, double value, string? unit = null)
        => Console.WriteLine($"METRIC {name}={value}{(unit is null ? string.Empty : unit)}");
}

public class TelemetrySource
{
    private readonly Random _rnd = new();
    private readonly ICollector _collector;
    public TelemetrySource(ICollector collector) => _collector = collector;

    [Metric("cpu.load", Unit = "%")]
    public double CpuLoad() => 15 + _rnd.NextDouble() * 50;

    [Metric("mem.used", Unit = "MB")]
    public double MemoryUsed() => 256 + _rnd.NextDouble() * 512;

#if FULL_INSTRUMENTATION
    [Metric("disk.latency", Unit = "ms")]
    public double DiskLatency() => 5 + _rnd.NextDouble() * 10;
#endif
}

public static class TelemetryRunner
{
    public static void CaptureMetrics(object target, ICollector collector)
    {
        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
            .Where(m => m.GetCustomAttribute<MetricAttribute>() != null);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<MetricAttribute>()!;
            var value = method.Invoke(target, null);
            if (value is double d)
                collector.Emit(attr.Name, d, attr.Unit);
        }
    }
}

var collector = new ConsoleCollector();
var source = new TelemetrySource(collector);
TelemetryRunner.CaptureMetrics(source, collector);
```
Why:
1. **Assembly Metadata** aids diagnostics tooling (CI/CD, support logs).
2. **Custom Attribute** marks metric-producing methods declaratively, avoiding manual registration lists.
3. **Reflection** discovers metrics dynamically—facilitates plugin modules without recompiling central registry.
4. **Conditional Compilation (`FULL_INSTRUMENTATION`)** toggles inclusion of expensive metrics for production vs. diagnostic builds.
5. **Separation of Concerns**: Metric collection orchestrated externally; source methods stay simple.

## Best Practices & Common Pitfalls

### Best Practices
- Centralize assembly versioning in project file (`<Version>`, `<FileVersion>`) instead of multiple `AssemblyVersion` attributes scattered.
- Use attributes to **declare intent**, not to hide complex logic—logic belongs in code, metadata should remain descriptive.
- Prefer `[Conditional]` for diagnostic/log methods to avoid `#if` fragmentation across many files.
- Keep custom attributes **lightweight & immutable**; they should not execute heavy logic in constructors.
- Use **symbol naming conventions** (`FEATURE_X`, `EXPERIMENTAL_Y`) and document them in the README / build scripts.
- Guard broad `#pragma warning disable` blocks; re-enable with `restore` and scope narrowly.
- For plugin systems: constrain reflection scanning (binding flags + attribute filters) to reduce startup cost.
- Use `InternalsVisibleTo` carefully; treat it as exposing internal API surface – limit to signed test assemblies.

### Common Pitfalls
- Overusing preprocessor directives causing **configuration maze** (hard to reason about). Prefer runtime configuration when feasible.
- Placing execution logic inside attribute constructors (runs at *compile time* for source generators or at *reflection load time* unexpectedly).
- Forgetting that `[Conditional]` only applies to *void* returning methods—using it on non-void method changes semantics.
- Relying on `AssemblyVersion` for patch updates—binding redirects may be required. Consider stable `AssemblyVersion` + variable `FileVersion` & `InformationalVersion`.
- Reflection scanning every assembly repeatedly — use caching.
- Mixing debug-only code that alters state flows (#if) leading to behavior divergence hard to test.

### When to Use
- Attributes: declarative, cross-cutting metadata (serialization, validation, mapping, classification, instrumentation).
- Conditional compilation: exclude code that cannot exist in certain targets (platform APIs, heavy instrumentation, profiling hooks).
- `[Conditional]`: enabling/disabling *calls* to diagnostic helpers with minimal code clutter.

### When NOT to Use
- Instead of dependency injection or configuration (attributes are static, not dynamic runtime choices).
- To encode large rule sets (prefer data/config or rule engines to attribute combinatorial explosion).
- To replace version-controlled feature flags maintained at runtime.

## Advanced Topics
- **Strong Naming & Signing**: Use `dotnet sign` or project properties to sign assemblies for GAC or tamper resistance.
- **Assembly Load Contexts**: Custom contexts isolate plugins (hot reload, version side-by-side). Use `AssemblyLoadContext` for dynamic reloading.
- **Trimming & AOT**: Reflection-heavy attribute scanning can break trimming—annotate with `DynamicDependency` or `DynamicallyAccessedMembers` as needed.
- **Source Generators**: Attributes can trigger compile-time code generation (e.g., `[JsonSerializable]`, custom analyzers producing boilerplate).
- **Conditional Symbols per Target Framework**: Multi-target libraries define symbols like `NET8_0_OR_GREATER` for granular feature gating.
- **InternalsVisibleTo & Friend Assemblies**: Share internals with test assemblies; keep strong name keys secure.
- **Caller Info Attributes**: `[CallerMemberName]`, `[CallerFilePath]`, `[CallerLineNumber]` enrich logging without manual arguments.
- **Custom Attribute Data**: Use `CustomAttributeData` for reflection without instantiating attribute objects (performance, trimming safety).
- **Attribute Inheritance & AllowMultiple**: Design attribute usage with future extensibility—restrict duplication unless stacking is meaningful.

## Summary

Assemblies encapsulate deployment, isolation, and versioning; attributes layer structured metadata onto your code, powering reflection, tooling, and declarative programming. Compiler directives and conditional compilation tailor builds to environments while keeping runtime lean. Use attributes to *describe* behavior, not perform it; adopt conditional mechanisms judiciously to prevent configuration sprawl. Mastery of these tools improves clarity, maintainability, and adaptability of your C# solutions across varied runtime and deployment constraints.
