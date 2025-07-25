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
    [SimpleJob(RunStrategy.Monitoring, warmupCount: 3, iterationCount: 30)]
    [MemoryDiagnoser]
    public class ParallelizedIntegerBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [Params(10, 100, 1000, 10000)]
        public int LookupCount { get; set; }

        private int[] intSourceData = null!;
        private int[] intLookupItems = null!;
        private static readonly object _fileLock = new object();

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            intSourceData = Enumerable.Range(1, N).Select(i => random.Next()).Distinct().Take(N).ToArray();
            intLookupItems = Enumerable.Range(1, LookupCount).Select(i => random.Next()).ToArray();
        }

        private void LogDetailedResults(string collectionType, int n, int lookupCount, TimeSpan creationTime, TimeSpan lookupTime, TimeSpan totalTime)
        {
            // Use a hardcoded absolute path to ensure we can find it
            var logPath = @"C:\Users\Gustavo\Documents\Projetos\Estudos_Csharp\CSharpStudies\Topic6\Benchmarks\BenchmarkDotNet.Artifacts\detailed_results.txt";
            
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
                    var backupPath = @"C:\temp\benchmark_debug.txt";
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                    File.AppendAllText(backupPath, $"ERROR: {ex.Message}\n");
                }
            }
        }

        [Benchmark]
        public double IntList_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new List<int>(intSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("List", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntHashSet_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new HashSet<int>(intSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("HashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntSortedSet_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedSet<int>(intSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("SortedSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntDictionary_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = intSourceData.ToDictionary(x => x, x => true);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("Dictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntSortedDictionary_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("SortedDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntConcurrentDictionary_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("ConcurrentDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntImmutableList_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(intSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("ImmutableList", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntImmutableHashSet_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(intSourceData);
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("ImmutableHashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }

        [Benchmark]
        public double IntArray_ParallelParams()
        {
            var totalSw = Stopwatch.StartNew();
            
            var creationSw = Stopwatch.StartNew();
            var collection = intSourceData.ToArray();
            creationSw.Stop();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            lookupSw.Stop();
            
            totalSw.Stop();
            
            LogDetailedResults("Array", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
            return totalSw.Elapsed.TotalMicroseconds;
        }
    }
}
