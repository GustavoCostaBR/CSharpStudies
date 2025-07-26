using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Engines;

namespace Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, warmupCount: 5, iterationCount: 50)]
    [MemoryDiagnoser]
    public class StringBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [Params(10, 100, 1000, 10000)]
        public int LookupCount { get; set; }

        private string[] stringSourceData = null!;
        private string[] stringLookupItems = null!;
        private static readonly object _fileLock = new object();

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            
            // Generate random strings of varying lengths (5-15 characters)
            stringSourceData = Enumerable.Range(1, N)
                .Select(i => GenerateRandomString(random, random.Next(5, 16)))
                .Distinct()
                .Take(N)
                .ToArray();
                
            stringLookupItems = Enumerable.Range(1, LookupCount)
                .Select(i => GenerateRandomString(random, random.Next(5, 16)))
                .ToArray();
        }

        private string GenerateRandomString(Random random, int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void LogDetailedResults(string collectionType, int n, int lookupCount, TimeSpan creationTime, TimeSpan lookupTime, TimeSpan totalTime)
        {
            // Use a hardcoded absolute path to ensure we can find it
            var logPath = @"C:\Users\Gustavo\Documents\Projetos\Estudos_Csharp\CSharpStudies\Topic6\Benchmarks\BenchmarkDotNet.Artifacts\detailed_results_string.txt";
            
            // Ensure directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(logPath));
            
            lock (_fileLock)
            {
                try
                {
                    // Check if we need to write header BEFORE opening the file
                    bool needsHeader = !File.Exists(logPath) || new FileInfo(logPath).Length == 0;
                    
                    using var writer = new StreamWriter(logPath, true);
                    if (needsHeader)
                    {
                        // Write header if file doesn't exist or is empty
                        writer.WriteLine("CollectionType,N,LookupCount,CreationTime_μs,LookupTime_μs,TotalTime_μs");
                    }
                    writer.WriteLine($"{collectionType},{n},{lookupCount},{creationTime.TotalMicroseconds:F2},{lookupTime.TotalMicroseconds:F2},{totalTime.TotalMicroseconds:F2}");
                    
                    // Force write to disk
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    // If there's an issue, write to a backup location we can debug
                    var backupPath = @"C:\temp\benchmark_debug_string.txt";
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                    File.AppendAllText(backupPath, $"ERROR: {ex.Message}\n");
                }
            }
        }

        [IterationCleanup]
        public void IterationCleanup()
        {
            // Force garbage collection between iterations to prevent memory pressure interference
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Benchmark]
        public double StringList()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new List<string>(stringSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("List", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringHashSet()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new HashSet<string>(stringSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("HashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringSortedSet()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedSet<string>(stringSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("SortedSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringDictionary()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = stringSourceData.ToDictionary(x => x, x => true);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("Dictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringSortedDictionary()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("SortedDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringConcurrentDictionary()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("ConcurrentDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringImmutableList()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(stringSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("ImmutableList", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringImmutableHashSet()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(stringSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("ImmutableHashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double StringArray()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = stringSourceData.ToArray();
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            lookupSw.Stop();
            
            totalSw.Stop();
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("Array", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }
    }
}
