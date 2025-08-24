# Topic 13: Delegates and Events

Delegates and events are the foundation of event-driven programming in .NET. They provide a powerful mechanism for creating loosely coupled systems where one object (the publisher) can notify other objects (subscribers) when something interesting happens, without either having to know about the other's concrete type.

## Delegates: A Type-Safe Function Pointer

A **delegate** is a reference type that can hold a reference to a method. Think of it as a "contract" or a "signature" for a method. Any method that matches the delegate's signature (return type and parameters) can be assigned to it.

### Declaring and Using a Custom Delegate

```csharp
// 1. Declare a delegate signature
public delegate void WorkPerformedHandler(int hours, string workType);

public class Worker
{
    public void DoWork(WorkPerformedHandler callback)
    {
        // Do some work...
        Console.WriteLine("Work started.");
        // Now, call the delegate to notify the caller.
        callback(8, "Programming");
    }
}

// --- Usage ---
var worker = new Worker();
// Create an instance of the delegate pointing to a method
var handler = new WorkPerformedHandler(WorkIsDone);
worker.DoWork(handler);

// This method matches the delegate's signature
public void WorkIsDone(int hours, string workType)
{
    Console.WriteLine($"Work complete! Type: {workType}, Hours: {hours}");
}
```

### Multicast Delegates

A single delegate instance can reference multiple methods. When the delegate is invoked, all methods are called in the order they were added.

```csharp
WorkPerformedHandler handler1 = WorkIsDone;
WorkPerformedHandler handler2 = LogWork;
// Use '+' or '+=' to combine delegates
WorkPerformedHandler multicastHandler = handler1 + handler2;

multicastHandler(5, "Testing");
// Output:
// Work complete! Type: Testing, Hours: 5
// Logging: 5 hours of Testing.
```

### Modern Approach: `Action<T>` and `Func<T, TResult>`

In modern C#, you rarely need to define your own custom delegates. .NET provides built-in generic delegates that cover almost every scenario:

-   **`Action<T>`**: For methods that return `void`. There are versions for 0 to 16 input parameters (e.g., `Action`, `Action<int>`, `Action<int, string>`).
-   **`Func<T, TResult>`**: For methods that return a value. The *last* generic type is always the return type (e.g., `Func<int, string>` takes an `int` and returns a `string`).

```csharp
// No custom delegate needed!
public void DoWorkModern(Action<int, string> callback)
{
    callback(8, "Refactoring");
}
```

## Events: Making Delegates Safe

An **event** is a wrapper around a delegate that exposes limited access. It enforces the publisher-subscriber pattern by only allowing outside classes to add (`+=`) or remove (`-=`) handlers. They cannot clear the list of subscribers or invoke the event directly.

### The Standard .NET Event Pattern

The standard pattern uses the `System.EventHandler` or `System.EventHandler<TEventArgs>` delegates.

1.  **Publisher Class**: The class that raises the event.
2.  **EventArgs**: A class that inherits from `EventArgs` to carry custom data with the event.
3.  **Subscriber Class**: The class that listens for and handles the event.

```csharp
// 2. Custom EventArgs to carry data
public class WorkPerformedEventArgs : EventArgs
{
    public int Hours { get; }
    public string WorkType { get; }

    public WorkPerformedEventArgs(int hours, string workType)
    {
        Hours = hours;
        WorkType = workType;
    }
}

// 1. The Publisher
public class EventWorker
{
    // Define the event using the standard EventHandler<T> delegate
    public event EventHandler<WorkPerformedEventArgs> WorkPerformed;

    public void DoWork()
    {
        Console.WriteLine("Worker is doing work...");
        // When work is done, raise the event
        OnWorkPerformed(8, "Documentation");
    }

    // Protected virtual method to raise the event
    protected virtual void OnWorkPerformed(int hours, string workType)
    {
        // Check if there are any subscribers before raising the event
        // The ?. operator is a thread-safe way to do this.
        WorkPerformed?.Invoke(this, new WorkPerformedEventArgs(hours, workType));
    }
}

// 3. The Subscriber
public class Supervisor
{
    public Supervisor(EventWorker worker)
    {
        // Subscribe to the event
        worker.WorkPerformed += OnWorkPerformed;
    }

    // The event handler method matches the EventHandler signature
    private void OnWorkPerformed(object sender, WorkPerformedEventArgs e)
    {
        Console.WriteLine($"Supervisor notified. Work type: {e.WorkType}, Hours: {e.Hours}");
    }
}
```

### Best Practices & Advanced Usage

#### Using Lambda Expressions

For simple handlers, lambda expressions provide a very concise syntax for subscribing to events.

```csharp
var worker = new EventWorker();
worker.WorkPerformed += (sender, e) =>
{
    Console.WriteLine($"Lambda handler: {e.Hours} hours of {e.WorkType}.");
};
worker.DoWork();
```

#### Unsubscribing to Prevent Memory Leaks

If a subscriber object has a shorter lifetime than a publisher object, it **must** unsubscribe from the event when it is no longer needed. If it doesn't, the publisher will hold a reference to the subscriber, preventing the Garbage Collector from cleaning it up. This is a common source of memory leaks.

```csharp
public class TemporaryListener : IDisposable
{
    private readonly EventWorker _worker;

    public TemporaryListener(EventWorker worker)
    {
        _worker = worker;
        _worker.WorkPerformed += OnWorkPerformed; // Subscribe
    }

    private void OnWorkPerformed(object sender, WorkPerformedEventArgs e)
    {
        Console.WriteLine("Temporary listener was notified.");
    }

    // Implement IDisposable to handle cleanup
    public void Dispose()
    {
        _worker.WorkPerformed -= OnWorkPerformed; // Unsubscribe!
        Console.WriteLine("Temporary listener has unsubscribed.");
    }
}
```
