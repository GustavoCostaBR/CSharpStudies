using EventBusTest;

using var email = new EmailService();
using var log = new LogService();

Console.WriteLine("Publishing events...");

// This will have subscribers
InMemoryEventBus.Publish("10");
InMemoryEventBus.Publish(new EventPayload(10, "10"));

// This will trigger the warning message (no subscribers for int)
InMemoryEventBus.Publish(42);

await Task.Delay(10000).ConfigureAwait(false);

Console.WriteLine("Done.");
