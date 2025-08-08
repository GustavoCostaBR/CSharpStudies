using System;
using System.Threading;

namespace CSharpStudies.Topic13.Examples
{
    //=========================================================================
    // Scenario: A video encoding service. After a video is encoded,
    // we need to notify other services (e.g., send an email, send a text).
    //=========================================================================


    //-------------------------------------------------------------------------
    // 1. The Tightly-Coupled Approach
    //-------------------------------------------------------------------------
    // In this design, the VideoEncoder class knows directly about the
    // notification services. It is "tightly coupled" to them.
    //-------------------------------------------------------------------------

    namespace TightlyCoupled
    {
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
            // The encoder has direct references to the services.
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
                Thread.Sleep(2000); // Simulate work
                Console.WriteLine("Finished encoding.");

                // After encoding, it directly calls the methods of the other services.
                _mailService.Send($"Video '{videoTitle}' has been encoded.");
                _messageService.Send($"Video '{videoTitle}' encoded.");
            }
        }
    }


    //-------------------------------------------------------------------------
    // 2. The Decoupled, Event-Driven Approach
    //-------------------------------------------------------------------------
    // In this design, the VideoEncoder knows nothing about the notification
    // services. It simply raises an event when its job is done.
    // Other classes can subscribe to this event if they are interested.
    //-------------------------------------------------------------------------

    namespace EventDriven
    {
        // Custom EventArgs to pass data with the event
        public class VideoEventArgs : EventArgs
        {
            public string VideoTitle { get; set; }
        }

        public class VideoEncoder
        {
            // 1. Define a delegate (or use the built-in EventHandler)
            // 2. Define an event based on that delegate
            public event EventHandler<VideoEventArgs> VideoEncoded;

            public void Encode(string videoTitle)
            {
                Console.WriteLine($"Encoding video '{videoTitle}'...");
                Thread.Sleep(2000); // Simulate work
                Console.WriteLine("Finished encoding.");

                // 3. Raise the event
                OnVideoEncoded(videoTitle);
            }

            // Protected virtual method to raise the event (best practice)
            protected virtual void OnVideoEncoded(string title)
            {
                // Check if there are any subscribers
                VideoEncoded?.Invoke(this, new VideoEventArgs() { VideoTitle = title });
            }
        }

        // Subscriber 1
        public class MailService
        {
            // Event handler method
            public void OnVideoEncoded(object source, VideoEventArgs args)
            {
                Console.WriteLine($"MAIL SERVICE: Sending email with message: 'Video ''{args.VideoTitle}'' has been encoded.'");
            }
        }

        // Subscriber 2
        public class MessageService
        {
            // Event handler method
            public void OnVideoEncoded(object source, VideoEventArgs args)
            {
                Console.WriteLine($"MESSAGE SERVICE: Sending text with message: 'Video ''{args.VideoTitle}'' encoded.'");
            }
        }
    }


    //-------------------------------------------------------------------------
    // Main Program to Run and Compare
    //-------------------------------------------------------------------------
    public class VideoEncoderProgram
    {
        public static void Run()
        {
            Console.WriteLine("--- Running Tightly-Coupled Example ---");
            var tightlyCoupledEncoder = new TightlyCoupled.VideoEncoder();
            tightlyCoupledEncoder.Encode("MyTripToTheAlps.mp4");

            Console.WriteLine("\n------------------------------------------\n");
            Console.WriteLine("What's the problem with the first approach?");
            Console.WriteLine("If we want to add a Push Notification service, we MUST modify the VideoEncoder class.");
            Console.WriteLine("This violates the Open/Closed Principle and makes the system rigid.\n");


            Console.WriteLine("--- Running Event-Driven Example ---");
            var eventDrivenEncoder = new EventDriven.VideoEncoder();
            var mailService = new EventDriven.MailService();
            var messageService = new EventDriven.MessageService();

            // The main application "wires up" the system by subscribing handlers to the event.
            eventDrivenEncoder.VideoEncoded += mailService.OnVideoEncoded;
            eventDrivenEncoder.VideoEncoded += messageService.OnVideoEncoded;

            eventDrivenEncoder.Encode("OurVacation.mov");

            Console.WriteLine("\n------------------------------------------\n");
            Console.WriteLine("Why is the second approach better?");
            Console.WriteLine("The VideoEncoder is completely decoupled. It doesn't know the MailService or MessageService exist.");
            Console.WriteLine("If we want to add a Push Notification service, we just create the new class and subscribe it to the event.");
            Console.WriteLine("The VideoEncoder class does not need to change. This is flexible, maintainable, and testable.");
        }
    }
}
