namespace EventBusTest;

public static class EventHelper
{
    public static void FireAndForgetHandlers<T>(Delegate[] snapshot, T payload)
    {
        if (snapshot == null || snapshot.Length == 0) return;
        foreach (var d in snapshot)
        {
            switch (d)
            {
                case Func<T, Task> asyncHandler:
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await asyncHandler(payload).ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EventHelper] Handler error (background): {ex.Message}");
                        }
                    });
                    break;
                case Action<T> syncHandler:
                    _ = Task.Run(() =>
                    {
                        try
                        {
                            syncHandler(payload);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[EventHelper] Handler error (background): {ex.Message}");
                        }
                    });
                    break;
            }
        }
    }
}