using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.CsProj;
using BenchmarkDotNet.Toolchains.DotNetCli;

namespace Benchmarks
{
    public class ParallelBenchmarkRunner
    {
        private readonly int _parallelInstances;
        private readonly ManualConfig _releaseConfig;

        public ParallelBenchmarkRunner(int parallelInstances = 8)
        {
            _parallelInstances = parallelInstances;
            
            // Create optimized release configuration
            _releaseConfig = ManualConfig.Create(DefaultConfig.Instance)
                .WithOptions(ConfigOptions.DisableOptimizationsValidator)
                .AddJob(Job.Default
                    .WithToolchain(CsProjCoreToolchain.NetCoreApp90)
                    .WithId("ReleaseJob"))
                .WithOption(ConfigOptions.JoinSummary, true);
        }

        public async Task RunBenchmarksInParallel()
        {
            Console.WriteLine($"Starting parallel benchmark execution with {_parallelInstances} instances...");
            Console.WriteLine($"CPU Core Count: {Environment.ProcessorCount}");
            Console.WriteLine(new string('=', 80));

            var benchmarkTasks = new List<Task>();
            var results = new ConcurrentBag<BenchmarkResult>();
            var stopwatch = Stopwatch.StartNew();

            // Define different benchmark scenarios to run in parallel
            var benchmarkScenarios = new List<BenchmarkScenario>
            {
                new("IntegerCollections", () => RunBenchmarkSafely<IntegerCollectionBenchmarks>()),
                new("StringCollections", () => RunBenchmarkSafely<StringCollectionBenchmarks>()),
                new("GuidCollections", () => RunBenchmarkSafely<GuidCollectionBenchmarks>()),
                new("CustomValueTypes", () => RunBenchmarkSafely<CustomValueTypeBenchmarks>()),
                new("CustomReferenceTypes", () => RunBenchmarkSafely<CustomReferenceTypeBenchmarks>())
            };

            // Take only the number of scenarios we want to run in parallel
            var scenariosToRun = benchmarkScenarios.Take(_parallelInstances);

            foreach (var scenario in scenariosToRun)
            {
                var task = Task.Run(async () =>
                {
                    var taskStopwatch = Stopwatch.StartNew();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Starting {scenario.Name}...");
                    
                    try
                    {
                        var result = await scenario.ExecuteAsync();
                        taskStopwatch.Stop();
                        
                        var benchmarkResult = new BenchmarkResult
                        {
                            ScenarioName = scenario.Name,
                            Success = result != null,
                            Duration = taskStopwatch.Elapsed,
                            ErrorMessage = result == null ? "Benchmark returned null" : null
                        };
                        
                        results.Add(benchmarkResult);
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Completed {scenario.Name} in {taskStopwatch.Elapsed:mm\\:ss}");
                    }
                    catch (Exception ex)
                    {
                        taskStopwatch.Stop();
                        var benchmarkResult = new BenchmarkResult
                        {
                            ScenarioName = scenario.Name,
                            Success = false,
                            Duration = taskStopwatch.Elapsed,
                            ErrorMessage = ex.Message
                        };
                        
                        results.Add(benchmarkResult);
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Failed {scenario.Name}: {ex.Message}");
                    }
                });
                
                benchmarkTasks.Add(task);
            }

            // Wait for all benchmarks to complete
            await Task.WhenAll(benchmarkTasks);
            stopwatch.Stop();

            // Print summary
            Console.WriteLine("\n" + new string('=', 80));
            Console.WriteLine("PARALLEL BENCHMARK EXECUTION SUMMARY");
            Console.WriteLine(new string('=', 80));
            Console.WriteLine($"Total Execution Time: {stopwatch.Elapsed:mm\\:ss}");
            Console.WriteLine($"Parallel Instances: {_parallelInstances}");
            Console.WriteLine();

            foreach (var result in results)
            {
                var status = result.Success ? "✓ SUCCESS" : "✗ FAILED";
                Console.WriteLine($"{status,-12} {result.ScenarioName,-25} Duration: {result.Duration:mm\\:ss}");
                if (!result.Success && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    Console.WriteLine($"             Error: {result.ErrorMessage}");
                }
            }

            var successCount = results.Count(r => r.Success);
            var failCount = results.Count(r => !r.Success);
            
            Console.WriteLine();
            Console.WriteLine($"Results: {successCount} successful, {failCount} failed");
            Console.WriteLine(new string('=', 80));
        }

        private async Task<object> RunBenchmarkSafely<T>() where T : class
        {
            return await Task.Run(() =>
            {
                try
                {
                    return BenchmarkRunner.Run<T>(_releaseConfig);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error running benchmark {typeof(T).Name}: {ex.Message}");
                    throw;
                }
            });
        }
    }

    public record BenchmarkScenario(string Name, Func<Task<object>> ExecuteAsync);

    public class BenchmarkResult
    {
        public string ScenarioName { get; set; } = string.Empty;
        public bool Success { get; set; }
        public TimeSpan Duration { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
