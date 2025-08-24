namespace EventBusTest;

public class EmailService : IDisposable
{
    public EmailService()
    {
        InMemoryEventBus.Subscribe<string>(HandleTextReady);
    }

    private static async Task HandleTextReady(string text)
    {
        await Task.Delay(50).ConfigureAwait(false);
        Console.WriteLine($"[EmailService] Sending email with text: {text}");
        throw new Exception("Simulated exception in EmailService");
    }

    public void Dispose()
    {
        InMemoryEventBus.Unsubscribe<string>(HandleTextReady);
        GC.SuppressFinalize(this);
    }
}
