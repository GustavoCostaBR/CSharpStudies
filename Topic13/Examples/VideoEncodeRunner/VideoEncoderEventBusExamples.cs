using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;

namespace CSharpStudies.Topic13.Examples.EventBus
{
    //==============================================================
    // 1. Lightweight In-Memory Event Bus (Publish/Subscribe pattern)
    //==============================================================
    // Synchronous vs Asynchronous: Handlers run sequentially (awaited) in-process.
    // No persistence / no retries / no isolation between publisher & subscribers beyond interface contracts.

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
                // Execute handlers sequentially; could be parallelized if desired.
                foreach (var h in handlers.ToList())
                {
                    await h(@event, ct);
                }
            }
        }
    }

    public sealed class VideoEncodedEvent : IDomainEvent
    {
        public required string Title { get; init; }
        public DateTime OccurredAtUtc { get; init; } = DateTime.UtcNow;
    }

    public sealed class VideoEncoderUsingBus
    {
        private readonly InMemoryEventBus _bus;
        public VideoEncoderUsingBus(InMemoryEventBus bus) => _bus = bus;

        public async Task EncodeAsync(string title, CancellationToken ct = default)
        {
            Console.WriteLine($"[InMemoryBus] Encoding '{title}'...");
            await Task.Delay(500, ct); // simulate work
            Console.WriteLine("[InMemoryBus] Finished encoding.");
            await _bus.PublishAsync(new VideoEncodedEvent { Title = title }, ct);
        }
    }

    public sealed class MailServiceHandler : IEventHandler<VideoEncodedEvent>
    {
        public Task HandleAsync(VideoEncodedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[InMemoryBus][MAIL] Video '{@event.Title}' encoded at {@event.OccurredAtUtc:O}");
            return Task.CompletedTask;
        }
    }

    public sealed class MessageServiceHandler : IEventHandler<VideoEncodedEvent>
    {
        public Task HandleAsync(VideoEncodedEvent @event, CancellationToken ct = default)
        {
            Console.WriteLine($"[InMemoryBus][MSG ] Video '{@event.Title}' encoded.");
            return Task.CompletedTask;
        }
    }

    public static class InMemoryEventBusExample
    {
        public static async Task RunAsync()
        {
            Console.WriteLine("\n--- In-Memory Event Bus Example ---");
            var bus = new InMemoryEventBus();
            bus.Subscribe(new MailServiceHandler());
            bus.Subscribe(new MessageServiceHandler());
            var encoder = new VideoEncoderUsingBus(bus);
            await encoder.EncodeAsync("CityDroneFootage.mp4");
        }
    }


    //==============================================================
    // 2. MassTransit (Enterprise Message Bus abstraction)
    //==============================================================
    // Here we use the built-in InMemory transport for simplicity.
    // MassTransit is fully asynchronous, supports middleware, retries, sagas, observability, etc.

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
            Console.WriteLine("\n--- MassTransit In-Memory Transport Example ---");

            var busControl = Bus.Factory.CreateUsingInMemory(cfg =>
            {
                cfg.ReceiveEndpoint("video-encoded-consumers", e =>
                {
                    e.Consumer<MailServiceConsumer>();
                    e.Consumer<MessageServiceConsumer>();
                });
            });

            // Start bus
            await busControl.StartAsync();
            try
            {
                Console.WriteLine("[MassTransit] Encoding 'WeddingHighlights.mov' ...");
                await Task.Delay(500);
                Console.WriteLine("[MassTransit] Finished encoding.");
                await busControl.Publish(new VideoEncoded("WeddingHighlights.mov", DateTime.UtcNow));
                await Task.Delay(250); // allow consumers to process before stopping
            }
            finally
            {
                await busControl.StopAsync();
            }
        }
    }
}
