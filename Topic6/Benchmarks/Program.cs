using System;
using System.IO;
using System.Linq;

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
                    var dataType = args.Length > 1 ? args[1].ToLower() : "int";
                    RunBenchmarks(dataType);
                    break;
                    
                case "analyze":
                case "analysis":
                case "a":
                case "summary":
                case "sum":
                case "s":
                    var analyzeDataType = args.Length > 1 ? args[1].ToLower() : "int";
                    RunStatisticalAnalysis(analyzeDataType);
                    break;
                    
                case "full":
                case "f":
                    var fullDataType = args.Length > 1 ? args[1].ToLower() : "int";
                    RunFullBenchmarkAndAnalysis(fullDataType);
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
            Console.WriteLine("  benchmark, bench, b    - Run collection benchmarks (50 iterations each)");
            Console.WriteLine("  analyze, analysis, a   - Analyze existing results with statistics");
            Console.WriteLine("  summary, sum, s        - Same as analyze (alias)");
            Console.WriteLine("  full, f               - Run benchmarks AND analysis in sequence");
            Console.WriteLine("  help, h, ?            - Show this help message");
            Console.WriteLine();
            Console.WriteLine("Data Types:");
            Console.WriteLine("  int, integer          - Integer benchmarks (default)");
            Console.WriteLine("  string, str           - String benchmarks");
            Console.WriteLine("  guid                  - GUID benchmarks");
            Console.WriteLine("  all                   - All data types");
            Console.WriteLine();
            Console.WriteLine("Features:");
            Console.WriteLine("  • Single-core execution for maximum consistency");
            Console.WriteLine("  • High process priority to minimize interference");
            Console.WriteLine("  • Comprehensive variance analysis");
            Console.WriteLine("  • Outlier detection and removal");
            Console.WriteLine("  • Statistical summaries and performance reports");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run -c Release benchmark int       # Run integer benchmarks");
            Console.WriteLine("  dotnet run -c Release benchmark string     # Run string benchmarks");
            Console.WriteLine("  dotnet run -c Release benchmark guid       # Run GUID benchmarks");
            Console.WriteLine("  dotnet run -c Release benchmark all        # Run all benchmarks");
            Console.WriteLine("  dotnet run -c Release analyze string       # Analyze string results");
            Console.WriteLine("  dotnet run -c Release full guid            # Run GUID benchmarks + analysis");
            Console.WriteLine("  dotnet run -c Release b                    # Short form integer benchmark");
            Console.WriteLine("  dotnet run -c Release s guid               # Short form GUID analysis");
            Console.WriteLine();
            Console.WriteLine("Note: All benchmark implementations use the same high-precision pattern");
            Console.WriteLine("      with single-core execution and statistical variance analysis.");
        }

        private static void RunBenchmarks(string dataType)
        {
            Console.WriteLine("=== Running Collection Performance Benchmarks ===");
            Console.WriteLine("The process will be configured for maximum consistency:");
            Console.WriteLine("• Real-time priority execution");
            Console.WriteLine("• Natural multi-core scheduling");
            Console.WriteLine("• Multiple iterations with statistical analysis");
            Console.WriteLine();
            Console.WriteLine("This may take several minutes to complete...");
            Console.WriteLine();

            switch (dataType)
            {
                case "int":
                case "integer":
                case "":
                    Console.WriteLine("Running INTEGER benchmarks...");
                    var intBenchmarks = new IntegerBenchmarks();
                    intBenchmarks.RunManual();
                    Console.WriteLine("✓ Integer benchmarks completed!");
                    Console.WriteLine("Results saved to: result/detailed_results_integer.txt");
                    break;
                    
                case "string":
                case "str":
                    Console.WriteLine("Running STRING benchmarks...");
                    var stringBenchmarks = new StringBenchmarks();
                    stringBenchmarks.RunManual();
                    Console.WriteLine("✓ String benchmarks completed!");
                    Console.WriteLine("Results saved to: result/detailed_results_string.txt");
                    break;

                case "guid":
                    Console.WriteLine("Running GUID benchmarks...");
                    var guidBenchmarks = new GuidBenchmarks();
                    guidBenchmarks.RunManual();
                    Console.WriteLine("✓ GUID benchmarks completed!");
                    Console.WriteLine("Results saved to: result/detailed_results_guid.txt");
                    break;
                    
                case "all":
                    Console.WriteLine("Running ALL benchmark types...");
                    Console.WriteLine("\n1/3 Running INTEGER benchmarks...");
                    var allIntBenchmarks = new IntegerBenchmarks();
                    allIntBenchmarks.RunManual();
                    Console.WriteLine("✓ Integer benchmarks completed!");
                    
                    Console.WriteLine("\n2/3 Running STRING benchmarks...");
                    var allStringBenchmarks = new StringBenchmarks();
                    allStringBenchmarks.RunManual();
                    Console.WriteLine("✓ String benchmarks completed!");
                    
                    Console.WriteLine("\n3/3 Running GUID benchmarks...");
                    var allGuidBenchmarks = new GuidBenchmarks();
                    allGuidBenchmarks.RunManual();
                    Console.WriteLine("✓ GUID benchmarks completed!");
                    
                    Console.WriteLine("\n✓ All benchmarks completed successfully!");
                    Console.WriteLine("Results saved to result/ directory");
                    break;
                    
                default:
                    Console.WriteLine($"Unknown data type: {dataType}");
                    Console.WriteLine("Available types: int, string, guid, all");
                    ShowHelp();
                    return;
            }
            
            Console.WriteLine();
            Console.WriteLine($"Run 'dotnet run -c Release analyze {dataType}' to generate statistical analysis.");
        }

        private static void RunStatisticalAnalysis(string dataType)
        {
            Console.WriteLine("=== Benchmark Result Statistical Analysis ===");
            
            var analyzer = new BenchmarkResultAnalyzer();
            
            // Determine file paths based on data type
            string resultsFileName, summaryFileName, reportFileName, dataTypeName;
            switch (dataType)
            {
                case "string":
                case "str":
                    resultsFileName = "detailed_results_string.txt";
                    summaryFileName = "statistical_summary_string.csv";
                    reportFileName = "performance_report_string.md";
                    dataTypeName = "STRING";
                    break;

                case "guid":
                    resultsFileName = "detailed_results_guid.txt";
                    summaryFileName = "statistical_summary_guid.csv";
                    reportFileName = "performance_report_guid.md";
                    dataTypeName = "GUID";
                    break;
                    
                case "int":
                case "integer":
                default:
                    resultsFileName = "detailed_results_integer.txt";
                    summaryFileName = "statistical_summary_integer.csv";
                    reportFileName = "performance_report_integer.md";
                    dataTypeName = "INTEGER";
                    break;
            }
            
            // Get the project directory (4 levels up from bin/Release/net9.0)
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName 
                                 ?? AppDomain.CurrentDomain.BaseDirectory;
            var baseDirectory = Path.Combine(projectDirectory, "result");
            var resultsPath = Path.Combine(baseDirectory, resultsFileName);
            var summaryPath = Path.Combine(baseDirectory, summaryFileName);
            var reportPath = Path.Combine(baseDirectory, reportFileName);
            
            Console.WriteLine($"Analyzing {dataTypeName} benchmark results...");
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
        
        private static void RunFullBenchmarkAndAnalysis(string dataType)
        {
            Console.WriteLine("=== Full Benchmark Execution: Benchmarks + Analysis ===");
            Console.WriteLine();
            
            // Run benchmarks first
            Console.WriteLine("Step 1/2: Running benchmarks...");
            RunBenchmarks(dataType);
            
            Console.WriteLine();
            Console.WriteLine("Step 2/2: Analyzing results...");
            RunStatisticalAnalysis(dataType);
            
            Console.WriteLine();
            Console.WriteLine("✓ Full benchmark and analysis completed!");
        }
    }
}
