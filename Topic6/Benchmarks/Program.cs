using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            var command = args[0].ToLower();

            switch (command)
            {
                case "benchmark":
                case "bench":
                case "b":
                    RunBenchmarks();
                    break;
                    
                case "analyze":
                case "analysis":
                case "a":
                    RunStatisticalAnalysis();
                    break;
                    
                case "help":
                case "h":
                case "?":
                    ShowHelp();
                    break;
                    
                default:
                    Console.WriteLine($"Unknown command: {command}");
                    ShowHelp();
                    break;
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("=== C# Collection Performance Benchmarks ===");
            Console.WriteLine();
            Console.WriteLine("Usage: dotnet run -c Release <command>");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  benchmark, bench, b    - Run performance benchmarks (30 iterations each)");
            Console.WriteLine("  analyze, analysis, a   - Analyze existing results with statistics");
            Console.WriteLine("  help, h, ?            - Show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run -c Release benchmark   # Run new benchmarks");
            Console.WriteLine("  dotnet run -c Release analyze     # Analyze existing data");
            Console.WriteLine("  dotnet run -c Release b           # Short form for benchmark");
            Console.WriteLine("  dotnet run -c Release a           # Short form for analyze");
        }

        private static void RunBenchmarks()
        {
            Console.WriteLine("=== Running Performance Benchmarks ===");
            Console.WriteLine("This will run all collection benchmarks with 30 parallel tasks each.");
            Console.WriteLine("This may take several minutes to complete...");
            Console.WriteLine();
            
            BenchmarkRunner.Run<ParallelizedIntegerBenchmarks>();
        }

        private static void RunStatisticalAnalysis()
        {
            Console.WriteLine("=== Benchmark Result Statistical Analysis ===");
            
            var analyzer = new BenchmarkResultAnalyzer();
            
            // Path to the detailed results file
            var resultsPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", "detailed_results.txt");
            var summaryPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", "statistical_summary.csv");
            var reportPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", "performance_report.md");
            
            Console.WriteLine($"Reading results from: {resultsPath}");
            
            // Read the benchmark results
            var results = analyzer.ReadResults(resultsPath);
            
            if (results.Count == 0)
            {
                Console.WriteLine("No results found. Make sure the detailed_results.txt file exists and contains data.");
                Console.WriteLine("Run 'dotnet run -c Release benchmark' first to generate benchmark data.");
                return;
            }
            
            Console.WriteLine($"Loaded {results.Count} benchmark results");
            
            // Analyze with outlier removal (Z-score threshold of 2.0)
            Console.WriteLine("Performing statistical analysis with outlier detection...");
            var summaries = analyzer.AnalyzeResults(results, outlierThreshold: 2.0);
            
            Console.WriteLine($"Generated {summaries.Count} statistical summaries");
            
            // Write statistical summary to CSV
            analyzer.WriteSummaryToFile(summaries, summaryPath);
            
            // Generate performance report
            analyzer.GeneratePerformanceReport(summaries, reportPath);
            
            // Display some quick stats
            Console.WriteLine("\n=== Quick Overview ===");
            var totalOutliers = summaries.Sum(s => s.OutliersRemoved);
            var totalSamples = summaries.Sum(s => s.SampleCount);
            Console.WriteLine($"Total samples analyzed: {totalSamples}");
            Console.WriteLine($"Total outliers removed: {totalOutliers} ({(double)totalOutliers/totalSamples*100:F1}%)");
            
            // Show top performers for largest scenario
            var largestScenario = summaries.Where(s => s.N == 10000 && s.LookupCount == 10000).OrderBy(s => s.TotalMean).Take(3).ToList();
            if (largestScenario.Any())
            {
                Console.WriteLine("\nTop 3 performers for N=10,000, Lookups=10,000:");
                for (int i = 0; i < largestScenario.Count; i++)
                {
                    var s = largestScenario[i];
                    Console.WriteLine($"{i+1}. {s.CollectionType}: {s.TotalMean:F1} μs (±{s.TotalStdDev:F1})");
                }
            }
            
            Console.WriteLine($"\nFiles generated:");
            Console.WriteLine($"- Statistical Summary: {summaryPath}");
            Console.WriteLine($"- Performance Report: {reportPath}");
        }
    }
}
