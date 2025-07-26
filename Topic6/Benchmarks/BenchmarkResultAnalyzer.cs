using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Benchmarks
{
    public class BenchmarkResultAnalyzer
    {
        public class BenchmarkResult
        {
            public string CollectionType { get; set; } = string.Empty;
            public int N { get; set; }
            public int LookupCount { get; set; }
            public double CreationTime { get; set; }
            public double LookupTime { get; set; }
            public double TotalTime { get; set; }
            public double OverheadTime { get; set; }
        }

        public class StatisticalSummary
        {
            public string CollectionType { get; set; } = string.Empty;
            public int N { get; set; }
            public int LookupCount { get; set; }
            public int SampleCount { get; set; }
            public int OutliersRemoved { get; set; }
            
            // Creation Time Stats
            public double CreationMean { get; set; }
            public double CreationStdDev { get; set; }
            public double CreationMin { get; set; }
            public double CreationMax { get; set; }
            public double CreationMedian { get; set; }
            
            // Lookup Time Stats
            public double LookupMean { get; set; }
            public double LookupStdDev { get; set; }
            public double LookupMin { get; set; }
            public double LookupMax { get; set; }
            public double LookupMedian { get; set; }
            
            // Total Time Stats
            public double TotalMean { get; set; }
            public double TotalStdDev { get; set; }
            public double TotalMin { get; set; }
            public double TotalMax { get; set; }
            public double TotalMedian { get; set; }
            
            // Overhead Time Stats
            public double OverheadMean { get; set; }
            public double OverheadStdDev { get; set; }
            public double OverheadMin { get; set; }
            public double OverheadMax { get; set; }
            public double OverheadMedian { get; set; }
        }

        public List<BenchmarkResult> ReadResults(string filePath)
        {
            var results = new List<BenchmarkResult>();
            
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"File not found: {filePath}");
                return results;
            }

            var lines = File.ReadAllLines(filePath);
            bool isFirstLine = true;

            foreach (var line in lines)
            {
                if (isFirstLine && line.StartsWith("CollectionType"))
                {
                    isFirstLine = false;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(line)) continue;

                try
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 6)
                    {
                        var result = new BenchmarkResult
                        {
                            CollectionType = parts[0].Trim(),
                            N = int.Parse(parts[1]),
                            LookupCount = int.Parse(parts[2]),
                            CreationTime = double.Parse(parts[3]),
                            LookupTime = double.Parse(parts[4]),
                            TotalTime = double.Parse(parts[5])
                        };

                        // Handle OverheadTime if available (new format)
                        if (parts.Length >= 7)
                        {
                            result.OverheadTime = double.Parse(parts[6]);
                        }
                        else
                        {
                            // Calculate overhead for older data without this column
                            result.OverheadTime = result.TotalTime - (result.CreationTime + result.LookupTime);
                        }

                        results.Add(result);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error parsing line: {line}. Error: {ex.Message}");
                }
            }

            Console.WriteLine($"Successfully parsed {results.Count} benchmark results");
            return results;
        }

        public List<StatisticalSummary> AnalyzeResults(List<BenchmarkResult> results, double outlierThreshold = 2.0)
        {
            var summaries = new List<StatisticalSummary>();

            // Group by CollectionType, N, and LookupCount
            var groups = results
                .GroupBy(r => new { r.CollectionType, r.N, r.LookupCount })
                .Where(g => g.Count() > 1); // Only analyze groups with multiple samples

            foreach (var group in groups)
            {
                var samples = group.ToList();
                
                // Remove outliers using Z-score method
                var cleanedSamples = RemoveOutliers(samples, outlierThreshold);
                
                if (cleanedSamples.Count == 0) continue;

                var summary = new StatisticalSummary
                {
                    CollectionType = group.Key.CollectionType,
                    N = group.Key.N,
                    LookupCount = group.Key.LookupCount,
                    SampleCount = samples.Count,
                    OutliersRemoved = samples.Count - cleanedSamples.Count
                };

                // Calculate statistics for Creation Time
                var creationTimes = cleanedSamples.Select(s => s.CreationTime).ToList();
                summary.CreationMean = creationTimes.Average();
                summary.CreationStdDev = CalculateStandardDeviation(creationTimes);
                summary.CreationMin = creationTimes.Min();
                summary.CreationMax = creationTimes.Max();
                summary.CreationMedian = CalculateMedian(creationTimes);

                // Calculate statistics for Lookup Time
                var lookupTimes = cleanedSamples.Select(s => s.LookupTime).ToList();
                summary.LookupMean = lookupTimes.Average();
                summary.LookupStdDev = CalculateStandardDeviation(lookupTimes);
                summary.LookupMin = lookupTimes.Min();
                summary.LookupMax = lookupTimes.Max();
                summary.LookupMedian = CalculateMedian(lookupTimes);

                // Calculate statistics for Total Time
                var totalTimes = cleanedSamples.Select(s => s.TotalTime).ToList();
                summary.TotalMean = totalTimes.Average();
                summary.TotalStdDev = CalculateStandardDeviation(totalTimes);
                summary.TotalMin = totalTimes.Min();
                summary.TotalMax = totalTimes.Max();
                summary.TotalMedian = CalculateMedian(totalTimes);

                // Calculate statistics for Overhead Time
                var overheadTimes = cleanedSamples.Select(s => s.OverheadTime).ToList();
                summary.OverheadMean = overheadTimes.Average();
                summary.OverheadStdDev = CalculateStandardDeviation(overheadTimes);
                summary.OverheadMin = overheadTimes.Min();
                summary.OverheadMax = overheadTimes.Max();
                summary.OverheadMedian = CalculateMedian(overheadTimes);

                summaries.Add(summary);
            }

            return summaries;
        }

        private List<BenchmarkResult> RemoveOutliers(List<BenchmarkResult> samples, double threshold)
        {
            if (samples.Count <= 3) return samples; // Don't remove outliers from very small samples

            // Calculate Z-scores for total time (most representative metric)
            var totalTimes = samples.Select(s => s.TotalTime).ToList();
            var mean = totalTimes.Average();
            var stdDev = CalculateStandardDeviation(totalTimes);

            if (stdDev == 0) return samples; // All values are the same

            var cleaned = new List<BenchmarkResult>();
            foreach (var sample in samples)
            {
                var zScore = Math.Abs((sample.TotalTime - mean) / stdDev);
                if (zScore <= threshold)
                {
                    cleaned.Add(sample);
                }
            }

            return cleaned.Count > 0 ? cleaned : samples; // Return original if all were outliers
        }

        private double CalculateStandardDeviation(List<double> values)
        {
            if (values.Count <= 1) return 0;

            var mean = values.Average();
            var sumOfSquaredDifferences = values.Sum(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(sumOfSquaredDifferences / (values.Count - 1));
        }

        private double CalculateMedian(List<double> values)
        {
            var sorted = values.OrderBy(x => x).ToList();
            int count = sorted.Count;
            
            if (count == 0) return 0;
            if (count % 2 == 0)
            {
                return (sorted[count / 2 - 1] + sorted[count / 2]) / 2.0;
            }
            else
            {
                return sorted[count / 2];
            }
        }

        public void WriteSummaryToFile(List<StatisticalSummary> summaries, string outputPath)
        {
            using var writer = new StreamWriter(outputPath);
            
            // Write header
            writer.WriteLine("CollectionType,N,LookupCount,SampleCount,OutliersRemoved," +
                           "CreationMean,CreationStdDev,CreationMin,CreationMax,CreationMedian," +
                           "LookupMean,LookupStdDev,LookupMin,LookupMax,LookupMedian," +
                           "TotalMean,TotalStdDev,TotalMin,TotalMax,TotalMedian," +
                           "OverheadMean,OverheadStdDev,OverheadMin,OverheadMax,OverheadMedian");

            // Write data
            foreach (var summary in summaries.OrderBy(s => s.CollectionType)
                                             .ThenBy(s => s.N)
                                             .ThenBy(s => s.LookupCount))
            {
                writer.WriteLine($"{summary.CollectionType},{summary.N},{summary.LookupCount}," +
                               $"{summary.SampleCount},{summary.OutliersRemoved}," +
                               $"{summary.CreationMean:F3},{summary.CreationStdDev:F3},{summary.CreationMin:F3},{summary.CreationMax:F3},{summary.CreationMedian:F3}," +
                               $"{summary.LookupMean:F3},{summary.LookupStdDev:F3},{summary.LookupMin:F3},{summary.LookupMax:F3},{summary.LookupMedian:F3}," +
                               $"{summary.TotalMean:F3},{summary.TotalStdDev:F3},{summary.TotalMin:F3},{summary.TotalMax:F3},{summary.TotalMedian:F3}," +
                               $"{summary.OverheadMean:F3},{summary.OverheadStdDev:F3},{summary.OverheadMin:F3},{summary.OverheadMax:F3},{summary.OverheadMedian:F3}");
            }

            Console.WriteLine($"Statistical summary written to: {outputPath}");
        }

        public void GeneratePerformanceReport(List<StatisticalSummary> summaries, string reportPath)
        {
            using var writer = new StreamWriter(reportPath);
            
            writer.WriteLine("# Statistical Performance Analysis Report");
            writer.WriteLine($"Generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            writer.WriteLine();

            // Analyze creation time variance patterns
            writer.WriteLine("## Creation Time Variance Analysis");
            writer.WriteLine();
            writer.WriteLine("### Theory: Why Creation Time Varies with Lookup Count");
            writer.WriteLine("Even though creation should be independent of lookup count, several factors cause variance:");
            writer.WriteLine();
            writer.WriteLine("1. **Memory Layout Optimization**: JIT compiler may optimize differently based on expected usage patterns");
            writer.WriteLine("2. **CPU Cache State**: Previous iterations affect CPU cache state, influencing memory allocation");
            writer.WriteLine("3. **Garbage Collection Timing**: Different GC pressure from previous tests affects allocation patterns");
            writer.WriteLine("4. **Heap Fragmentation**: Memory fragmentation from previous iterations affects new allocations");
            writer.WriteLine("5. **Branch Prediction**: CPU branch predictor state varies based on previous operations");
            writer.WriteLine("6. **Memory Pre-allocation**: Some collections pre-allocate based on detected usage patterns");
            writer.WriteLine();

            // Analyze creation time patterns
            var creationVarianceAnalysis = AnalyzeCreationTimeVariance(summaries);
            writer.WriteLine("### Creation Time Variance by Collection Type:");
            foreach (var analysis in creationVarianceAnalysis)
            {
                writer.WriteLine($"- **{analysis.CollectionType}**: {analysis.VariancePattern} (CV: {analysis.CoefficientOfVariation:F1}%)");
            }
            writer.WriteLine();

            // Group by scenario for analysis
            var scenarios = summaries.GroupBy(s => new { s.N, s.LookupCount }).OrderBy(g => g.Key.N).ThenBy(g => g.Key.LookupCount);

            foreach (var scenario in scenarios)
            {
                writer.WriteLine($"## Scenario: N={scenario.Key.N}, Lookups={scenario.Key.LookupCount}");
                writer.WriteLine();

                var orderedByTotal = scenario.OrderBy(s => s.TotalMean).ToList();
                
                writer.WriteLine("### Total Time Performance Ranking:");
                for (int i = 0; i < orderedByTotal.Count; i++)
                {
                    var summary = orderedByTotal[i];
                    var emoji = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : "  ";
                    writer.WriteLine($"{emoji} **{summary.CollectionType}**: {summary.TotalMean:F1} μs " +
                                   $"(±{summary.TotalStdDev:F1}, {summary.OutliersRemoved} outliers removed)");
                }
                writer.WriteLine();

                // Show reliability metrics
                writer.WriteLine("### Reliability Analysis:");
                foreach (var summary in orderedByTotal.Take(3))
                {
                    var cv = summary.TotalStdDev / summary.TotalMean * 100; // Coefficient of variation
                    var reliability = cv < 5 ? "Very Reliable" : cv < 10 ? "Reliable" : cv < 20 ? "Moderate" : "Unreliable";
                    writer.WriteLine($"- **{summary.CollectionType}**: CV = {cv:F1}% ({reliability})");
                }
                writer.WriteLine();
            }

            Console.WriteLine($"Performance report written to: {reportPath}");
        }

        private List<CreationVarianceAnalysis> AnalyzeCreationTimeVariance(List<StatisticalSummary> summaries)
        {
            var analysis = new List<CreationVarianceAnalysis>();
            
            var collectionGroups = summaries.GroupBy(s => s.CollectionType);
            
            foreach (var group in collectionGroups)
            {
                var creationTimes = group.Select(s => s.CreationMean).ToList();
                var coefficientOfVariation = (creationTimes.StandardDeviation() / creationTimes.Average()) * 100;
                
                var pattern = coefficientOfVariation switch
                {
                    < 10 => "Very Stable",
                    < 25 => "Stable",
                    < 50 => "Moderate Variance",
                    < 100 => "High Variance",
                    _ => "Extremely Variable"
                };
                
                analysis.Add(new CreationVarianceAnalysis
                {
                    CollectionType = group.Key,
                    CoefficientOfVariation = coefficientOfVariation,
                    VariancePattern = pattern
                });
            }
            
            return analysis.OrderBy(a => a.CoefficientOfVariation).ToList();
        }

        private class CreationVarianceAnalysis
        {
            public string CollectionType { get; set; } = string.Empty;
            public double CoefficientOfVariation { get; set; }
            public string VariancePattern { get; set; } = string.Empty;
        }
    }

    // Extension method for standard deviation calculation
    public static class EnumerableExtensions
    {
        public static double StandardDeviation(this IEnumerable<double> values)
        {
            var valuesList = values.ToList();
            if (valuesList.Count <= 1) return 0;
            
            var mean = valuesList.Average();
            var sumOfSquaredDifferences = valuesList.Sum(v => Math.Pow(v - mean, 2));
            return Math.Sqrt(sumOfSquaredDifferences / (valuesList.Count - 1));
        }
    }
}
