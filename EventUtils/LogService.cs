namespace EventBusTest;

public class LogService : IDisposable
{
    public LogService()
    {
        InMemoryEventBus.Subscribe<EventPayload>(HandleTextReady);
    }

    private static async Task HandleTextReady(EventPayload eventPayload)
    {
        await Task.Delay(5000).ConfigureAwait(false);
        Console.WriteLine($"[LogService] Log entry: {eventPayload}");
    }

    public void Dispose()
    {
        InMemoryEventBus.Unsubscribe<EventPayload>(HandleTextReady);
        GC.SuppressFinalize(this);
    }
}

public record EventPayload(int IntegerValue, string StringValue);
