## Advanced Event-Driven Programming Examples

### Tightly-Coupled Approach
In this approach, the `VideoEncoder` class directly references the notification services (e.g., `MailService` and `MessageService`). This creates a rigid system that violates the Open/Closed Principle.

#### Code Example:
```csharp
public class MailService
{
    public void Send(string message)
    {
        Console.WriteLine($"MAIL SERVICE: Sending email with message: '{message}'");
    }
}

public class MessageService
{
    public void Send(string message)
    {
        Console.WriteLine($"MESSAGE SERVICE: Sending text with message: '{message}'");
    }
}

public class VideoEncoder
{
    private readonly MailService _mailService;
    private readonly MessageService _messageService;

    public VideoEncoder()
    {
        _mailService = new MailService();
        _messageService = new MessageService();
    }

    public void Encode(string videoTitle)
    {
        Console.WriteLine($"Encoding video '{videoTitle}'...");
        Thread.Sleep(2000);
        Console.WriteLine("Finished encoding.");

        _mailService.Send($"Video '{videoTitle}' has been encoded.");
        _messageService.Send($"Video '{videoTitle}' encoded.");
    }
}
```

### Decoupled, Event-Driven Approach
This approach uses events to decouple the `VideoEncoder` class from the notification services. The `VideoEncoder` raises an event when encoding is complete, and other classes subscribe to this event.

#### Code Example:
```csharp
public class VideoEventArgs : EventArgs
{
    public string VideoTitle { get; set; }
}

public class VideoEncoder
{
    public event EventHandler<VideoEventArgs> VideoEncoded;

    public void Encode(string videoTitle)
    {
        Console.WriteLine($"Encoding video '{videoTitle}'...");
        Thread.Sleep(2000);
        Console.WriteLine("Finished encoding.");

        OnVideoEncoded(videoTitle);
    }

    protected virtual void OnVideoEncoded(string title)
    {
        VideoEncoded?.Invoke(this, new VideoEventArgs() { VideoTitle = title });
    }
}

public class MailService
{
    public void OnVideoEncoded(object source, VideoEventArgs args)
    {
        Console.WriteLine($"MAIL SERVICE: Sending email with message: 'Video '{args.VideoTitle}' has been encoded.'");
    }
}

public class MessageService
{
    public void OnVideoEncoded(object source, VideoEventArgs args)
    {
        Console.WriteLine($"MESSAGE SERVICE: Sending text with message: 'Video '{args.VideoTitle}' encoded.'");
    }
}
```

### In-Memory Event Bus
An abstraction for publish/subscribe messaging within the same process. Handlers are executed sequentially.

#### Code Example:
```csharp
public interface IDomainEvent { }

public interface IEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent @event, CancellationToken ct = default);
}

public sealed class InMemoryEventBus
{
    private readonly ConcurrentDictionary<Type, List<Func<IDomainEvent, CancellationToken, Task>>> _routes = new();

    public void Subscribe<TEvent>(IEventHandler<TEvent> handler) where TEvent : IDomainEvent
    {
        var list = _routes.GetOrAdd(typeof(TEvent), _ => new List<Func<IDomainEvent, CancellationToken, Task>>());
        lock (list)
        {
            list.Add(async (e, ct) => await handler.HandleAsync((TEvent)e, ct));
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : IDomainEvent
    {
        if (_routes.TryGetValue(@event.GetType(), out var handlers))
        {
            foreach (var h in handlers.ToList())
            {
                await h(@event, ct);
            }
        }
    }
}
```

#### Real-World Usage of In-Memory Event Bus
The `InMemoryEventBus` can be used to decouple components in an application. Below is an example demonstrating how to use it in a real-world scenario.

```csharp
// Define a domain event
public class OrderPlacedEvent : IDomainEvent
{
    public string OrderId { get; set; }
    public DateTime PlacedAt { get; set; }
}

// Define an event handler for sending email notifications
public class EmailNotificationHandler : IEventHandler<OrderPlacedEvent>
{
    public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
    {
        Console.WriteLine($"[EMAIL] Order '{@event.OrderId}' placed at {@event.PlacedAt}.");
        return Task.CompletedTask;
    }
}

// Define an event handler for updating inventory
public class InventoryUpdateHandler : IEventHandler<OrderPlacedEvent>
{
    public Task HandleAsync(OrderPlacedEvent @event, CancellationToken ct = default)
    {
        Console.WriteLine($"[INVENTORY] Updating inventory for order '{@event.OrderId}'.");
        return Task.CompletedTask;
    }
}

// Usage
var eventBus = new InMemoryEventBus();
eventBus.Subscribe(new EmailNotificationHandler());
eventBus.Subscribe(new InventoryUpdateHandler());

// Publish an event
await eventBus.PublishAsync(new OrderPlacedEvent { OrderId = "12345", PlacedAt = DateTime.UtcNow });
```

### MassTransit Example
MassTransit is an enterprise-level message bus abstraction. It supports asynchronous messaging, retries, and more.

#### Code Example:
```csharp
public record VideoEncoded(string Title, DateTime OccurredAtUtc);

public sealed class MailServiceConsumer : IConsumer<VideoEncoded>
{
    public Task Consume(ConsumeContext<VideoEncoded> context)
    {
        Console.WriteLine($"[MassTransit][MAIL] Video '{context.Message.Title}' encoded at {context.Message.OccurredAtUtc:O}");
        return Task.CompletedTask;
    }
}

public sealed class MessageServiceConsumer : IConsumer<VideoEncoded>
{
    public Task Consume(ConsumeContext<VideoEncoded> context)
    {
        Console.WriteLine($"[MassTransit][MSG ] Video '{context.Message.Title}' encoded.");
        return Task.CompletedTask;
    }
}

public static class MassTransitExample
{
    public static async Task RunAsync()
    {
        var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
        {
            cfg.ReceiveEndpoint("video-encoded-consumers", e =>
            {
                e.Consumer<MailServiceConsumer>();
                e.Consumer<MessageServiceConsumer>();
            });
        });

        await busControl.StartAsync();
        try
        {
            await busControl.Publish(new VideoEncoded("WeddingHighlights.mov", DateTime.UtcNow));
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}
```

#### Improvements to MassTransit Example

MassTransit is a robust library for enterprise-level messaging. To use it effectively:
1. Install the `MassTransit` NuGet package.
2. Configure the message bus with a transport (e.g., RabbitMQ, Azure Service Bus, or InMemory for testing).
3. Define message contracts and consumers.
4. Publish messages and handle them in consumers.

```csharp
// Define a message contract
public record OrderShipped(string OrderId, DateTime ShippedAt);

// Define a consumer for the message
public class ShippingNotificationConsumer : IConsumer<OrderShipped>
{
    public Task Consume(ConsumeContext<OrderShipped> context)
    {
        Console.WriteLine($"[MassTransit][SHIPPING] Order '{context.Message.OrderId}' shipped at {context.Message.ShippedAt:O}.");
        return Task.CompletedTask;
    }
}

// Main program to configure and run MassTransit
public class Program
{
    public static async Task Main()
    {
        var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
        {
            cfg.ReceiveEndpoint("order-shipped-consumers", e =>
            {
                e.Consumer<ShippingNotificationConsumer>();
            });
        });

        await busControl.StartAsync();
        try
        {
            var orderShipped = new OrderShipped("12345", DateTime.UtcNow);
            await busControl.Publish(orderShipped);
        }
        finally
        {
            await busControl.StopAsync();
        }
    }
}
```

## Events and Delegates (Advanced)

### Delegates
Delegates are type-safe function pointers in C#. They allow methods to be passed as parameters, enabling dynamic method invocation. Delegates are used extensively in event-driven programming and callback mechanisms.

#### Why Delegates Exist
- **Flexibility**: Delegates allow you to define methods that can be called dynamically at runtime.
- **Decoupling**: They enable loose coupling between components by allowing one component to call methods in another without knowing the implementation details.
- **Event Handling**: Delegates are the foundation of events in C#.

#### How to Use Delegates
1. Define a delegate type.
2. Create an instance of the delegate.
3. Assign a method to the delegate.
4. Invoke the delegate.

#### Code Example:
```csharp
// Step 1: Define a delegate type
public delegate void Notify(string message);

// Step 2: Create a class that uses the delegate
public class Process
{
    public Notify OnProcessCompleted;

    public void StartProcess()
    {
        Console.WriteLine("Process started...");
        Thread.Sleep(2000); // Simulate work
        Console.WriteLine("Process completed.");

        // Step 4: Invoke the delegate
        OnProcessCompleted?.Invoke("Process finished successfully.");
    }
}

// Step 3: Assign a method to the delegate
public class Program
{
    public static void Main()
    {
        Process process = new Process();
        process.OnProcessCompleted = NotifyUser;
        process.StartProcess();
    }

    public static void NotifyUser(string message)
    {
        Console.WriteLine($"Notification: {message}");
    }
}
```

### Events
Events are a specialized form of delegates that provide a way for a class to notify other classes or objects when something of interest occurs. Events are used to implement the observer pattern.

#### Why Events Exist
- **Encapsulation**: Events restrict direct access to the delegate, allowing only subscription and unsubscription.
- **Notification Mechanism**: They provide a way to notify multiple subscribers about an occurrence.

#### How to Use Events
1. Define a delegate type.
2. Define an event based on the delegate.
3. Raise the event when needed.
4. Subscribe to the event.

#### Code Example:
```csharp
// Step 1: Define a delegate type
public delegate void ProcessCompletedEventHandler(string message);

// Step 2: Define an event based on the delegate
public class Process
{
    public event ProcessCompletedEventHandler ProcessCompleted;

    public void StartProcess()
    {
        Console.WriteLine("Process started...");
        Thread.Sleep(2000); // Simulate work
        Console.WriteLine("Process completed.");

        // Step 3: Raise the event
        ProcessCompleted?.Invoke("Process finished successfully.");
    }
}

// Step 4: Subscribe to the event
public class Program
{
    public static void Main()
    {
        Process process = new Process();
        process.ProcessCompleted += NotifyUser;
        process.StartProcess();
    }

    public static void NotifyUser(string message)
    {
        Console.WriteLine($"Notification: {message}");
    }
}
```

### Callback Usage
Callbacks are a mechanism to execute a method after another method has completed its execution. Delegates are often used to implement callbacks.

#### Why Callbacks Exist
- **Asynchronous Programming**: Callbacks allow you to execute code after an asynchronous operation completes.
- **Custom Logic**: They enable the caller to specify custom logic to be executed.

#### Code Example:
```csharp
public class Calculator
{
    public void Add(int a, int b, Action<int> callback)
    {
        int result = a + b;
        callback(result); // Invoke the callback with the result
    }
}

public class Program
{
    public static void Main()
    {
        Calculator calculator = new Calculator();
        calculator.Add(5, 3, result => Console.WriteLine($"The result is: {result}"));
    }
}
```

### Event Example with Data Transfer
In this example, the event publisher sends a custom object containing detailed information to the subscribers. The subscribers then process this information in their own way.

#### Code Example:
```csharp
// Define a custom EventArgs class to hold event data
public class VideoEncodedEventArgs : EventArgs
{
    public string VideoTitle { get; set; }
    public DateTime EncodedAt { get; set; }
    public string EncoderName { get; set; }
}

// Publisher class
public class VideoEncoder
{
    // Define the event
    public event EventHandler<VideoEncodedEventArgs> VideoEncoded;

    public void Encode(string videoTitle, string encoderName)
    {
        Console.WriteLine($"Encoding video '{videoTitle}' using {encoderName}...");
        Thread.Sleep(2000); // Simulate encoding work
        Console.WriteLine("Finished encoding.");

        // Raise the event with detailed information
        OnVideoEncoded(new VideoEncodedEventArgs
        {
            VideoTitle = videoTitle,
            EncodedAt = DateTime.Now,
            EncoderName = encoderName
        });
    }

    protected virtual void OnVideoEncoded(VideoEncodedEventArgs e)
    {
        VideoEncoded?.Invoke(this, e);
    }
}

// Subscriber 1: Logs the encoding details
public class LoggingService
{
    public void OnVideoEncoded(object source, VideoEncodedEventArgs e)
    {
        Console.WriteLine($"[LOG] Video '{e.VideoTitle}' was encoded by {e.EncoderName} at {e.EncodedAt}.");
    }
}

// Subscriber 2: Sends a notification
public class NotificationService
{
    public void OnVideoEncoded(object source, VideoEncodedEventArgs e)
    {
        Console.WriteLine($"[NOTIFICATION] Sending notification: Video '{e.VideoTitle}' encoded successfully by {e.EncoderName}.");
    }
}

// Main program to demonstrate the event
public class Program
{
    public static void Main()
    {
        var videoEncoder = new VideoEncoder();
        var loggingService = new LoggingService();
        var notificationService = new NotificationService();

        // Subscribe to the event
        videoEncoder.VideoEncoded += loggingService.OnVideoEncoded;
        videoEncoder.VideoEncoded += notificationService.OnVideoEncoded;

        // Start encoding
        videoEncoder.Encode("NatureDocumentary.mp4", "HighQualityEncoder");
    }
}
```

### Summary
- **Delegates**: Enable dynamic method invocation and are the foundation of events.
- **Events**: Provide a notification mechanism for subscribers.
- **Callbacks**: Allow custom logic to be executed after a method completes.

These concepts are essential for building flexible, decoupled, and event-driven applications in C#.