namespace EventBusTest;

public static class InMemoryEventBus
{
    private static readonly HandlerCollection HandlerStore = new();

    public static void Subscribe<T>(Func<T, Task> handler)
    {
        HandlerStore.AtomicAddHandler(typeof(T).FullName!, handler);
    }

    public static void Subscribe<T>(Action<T> handler)
    {
        HandlerStore.AtomicAddHandler(typeof(T).FullName!, handler);
    }

    public static void Unsubscribe<T>(Func<T, Task> handler)
    {
        HandlerStore.AtomicRemoveHandler(typeof(T).FullName!, handler);
    }

    public static void Unsubscribe<T>(Action<T> handler)
    {
        HandlerStore.AtomicRemoveHandler(typeof(T).FullName!, handler);
    }

    public static void Publish<T>(T payload)
    {
        var eventKey = typeof(T).FullName!;
        var snapshot = HandlerStore.Snapshot(eventKey);
        
        if (snapshot.Length == 0)
        {
            Console.WriteLine($"[EventBus] Warning: No subscribers found for event type '{typeof(T).Name}' with payload: {payload}");
            return;
        }
        
        EventHelper.FireAndForgetHandlers(snapshot, payload);
    }
}