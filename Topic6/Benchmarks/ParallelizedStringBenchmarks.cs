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
    [SimpleJob(RunStrategy.Monitoring, warmupCount: 3, iterationCount: 10)]
    [MemoryDiagnoser]
    public class ParallelizedStringBenchmarks
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

        [Benchmark]
        public async Task<double> StringList_ParallelParams()
        {
            // Run 30 parallel instances for better statistical reliability
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            
            // Log detailed results for this specific N/LookupCount combination
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("List", N, LookupCount, avgCreation, avgLookup, avgTotal);
            
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringHashSet_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("HashSet", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringSortedSet_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("SortedSet", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringDictionary_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("Dictionary", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringSortedDictionary_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("SortedDictionary", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringConcurrentDictionary_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("ConcurrentDictionary", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringImmutableList_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("ImmutableList", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringImmutableHashSet_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("ImmutableHashSet", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }

        [Benchmark]
        public async Task<double> StringArray_ParallelParams()
        {
            var tasks = new Task<(TimeSpan creation, TimeSpan lookup, TimeSpan total)>[30];
            
            for (int i = 0; i < 30; i++)
            {
                tasks[i] = Task.Run(() =>
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
                    
                    return (creationSw.Elapsed, lookupSw.Elapsed, totalSw.Elapsed);
                });
            }
            
            var results = await Task.WhenAll(tasks);
            var avgCreation = TimeSpan.FromTicks((long)results.Average(r => r.creation.Ticks));
            var avgLookup = TimeSpan.FromTicks((long)results.Average(r => r.lookup.Ticks));
            var avgTotal = TimeSpan.FromTicks((long)results.Average(r => r.total.Ticks));
            
            LogDetailedResults("Array", N, LookupCount, avgCreation, avgLookup, avgTotal);
            return avgTotal.TotalMicroseconds;
        }
    }
}
