using System;
using System.Threading.Tasks;

namespace Benchmarks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Parallel Benchmark Runner");
            Console.WriteLine("=========================");
            
            // Parse command line arguments for parallel instances count
            int parallelInstances = 8; // Default to 8 instances
            
            if (args.Length > 0 && int.TryParse(args[0], out int userInstances))
            {
                if (userInstances >= 2 && userInstances <= 16)
                {
                    parallelInstances = userInstances;
                }
                else
                {
                    Console.WriteLine("Warning: Parallel instances should be between 2 and 16. Using default of 8.");
                }
            }
            
            Console.WriteLine($"Running with {parallelInstances} parallel instances");
            Console.WriteLine($"Available CPU cores: {Environment.ProcessorCount}");
            Console.WriteLine();
            
            var runner = new ParallelBenchmarkRunner(parallelInstances);
            
            try
            {
                await runner.RunBenchmarksInParallel();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error running parallel benchmarks: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Environment.Exit(1);
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
