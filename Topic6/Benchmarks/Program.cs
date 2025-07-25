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
                    var dataType = args.Length > 1 ? args[1].ToLower() : "";
                    RunBenchmarks(dataType);
                    break;
                    
                case "analyze":
                case "analysis":
                case "a":
                    var analyzeDataType = args.Length > 1 ? args[1].ToLower() : "int";
                    RunStatisticalAnalysis(analyzeDataType);
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
            Console.WriteLine("Usage: dotnet run -c Release <command> [datatype]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  benchmark, bench, b    - Run performance benchmarks (30 iterations each)");
            Console.WriteLine("  analyze, analysis, a   - Analyze existing results with statistics");
            Console.WriteLine("  help, h, ?            - Show this help message");
            Console.WriteLine();
            Console.WriteLine("Data Types:");
            Console.WriteLine("  int, integer          - Integer benchmarks (default)");
            Console.WriteLine("  string, str           - String benchmarks");
            Console.WriteLine("  all                   - All data types (benchmark only)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run -c Release benchmark int       # Run integer benchmarks");
            Console.WriteLine("  dotnet run -c Release benchmark string     # Run string benchmarks");
            Console.WriteLine("  dotnet run -c Release benchmark all        # Run all benchmarks");
            Console.WriteLine("  dotnet run -c Release analyze int          # Analyze integer results");
            Console.WriteLine("  dotnet run -c Release analyze string       # Analyze string results");
            Console.WriteLine("  dotnet run -c Release b int                # Short form");
            Console.WriteLine("  dotnet run -c Release a string             # Short form");
        }

        private static void RunBenchmarks(string dataType)
        {
            Console.WriteLine("=== Running Performance Benchmarks ===");
            Console.WriteLine("This will run collection benchmarks with 30 parallel tasks each.");
            Console.WriteLine("This may take several minutes to complete...");
            Console.WriteLine();

            switch (dataType)
            {
                case "int":
                case "integer":
                case "":
                    Console.WriteLine("Running INTEGER benchmarks...");
                    BenchmarkRunner.Run<ParallelizedIntegerBenchmarks>();
                    break;
                    
                case "string":
                case "str":
                    Console.WriteLine("Running STRING benchmarks...");
                    BenchmarkRunner.Run<ParallelizedStringBenchmarks>();
                    break;
                    
                case "all":
                    Console.WriteLine("Running ALL benchmark types...");
                    Console.WriteLine("\n1/2 Running INTEGER benchmarks...");
                    BenchmarkRunner.Run<ParallelizedIntegerBenchmarks>();
                    Console.WriteLine("\n2/2 Running STRING benchmarks...");
                    BenchmarkRunner.Run<ParallelizedStringBenchmarks>();
                    break;
                    
                default:
                    Console.WriteLine($"Unknown data type: {dataType}");
                    Console.WriteLine("Available types: int, string, all");
                    ShowHelp();
                    break;
            }
        }

        private static void RunStatisticalAnalysis(string dataType)
        {
            Console.WriteLine("=== Benchmark Result Statistical Analysis ===");
            
            var analyzer = new BenchmarkResultAnalyzer();
            
            // Determine file paths based on data type
            string resultsFileName, summaryFileName, reportFileName;
            switch (dataType)
            {
                case "string":
                case "str":
                    resultsFileName = "detailed_results_string.txt";
                    summaryFileName = "statistical_summary_string.csv";
                    reportFileName = "performance_report_string.md";
                    Console.WriteLine("Analyzing STRING benchmark results...");
                    break;
                    
                case "int":
                case "integer":
                default:
                    resultsFileName = "detailed_results.txt";
                    summaryFileName = "statistical_summary.csv";
                    reportFileName = "performance_report.md";
                    Console.WriteLine("Analyzing INTEGER benchmark results...");
                    break;
            }
            
            var resultsPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", resultsFileName);
            var summaryPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", summaryFileName);
            var reportPath = Path.Combine(Environment.CurrentDirectory, "BenchmarkDotNet.Artifacts", reportFileName);
            
            Console.WriteLine($"Reading results from: {resultsPath}");
            
            // Read the benchmark results
            var results = analyzer.ReadResults(resultsPath);
            
            if (results.Count == 0)
            {
                Console.WriteLine($"No {dataType} results found. Make sure the {resultsFileName} file exists and contains data.");
                Console.WriteLine($"Run 'dotnet run -c Release benchmark {dataType}' first to generate benchmark data.");
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
