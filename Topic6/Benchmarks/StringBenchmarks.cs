using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmarks
{
    public class StringBenchmarks
    {
        // Benchmark iteration constants
        public const int WarmupIterations = 5;
        public const int ActualIterations = 50;

        private int N { get; set; }
        private int LookupCount { get; set; }

        private string[] stringSourceData = null!;
        private string[] stringLookupItems = null!;
        private static readonly object _fileLock = new object();
        private int _iterationCount = 0;

        public void RunManual()
        {
            // Configure process for maximum consistency
            ConfigureProcessForBenchmarking();
            
            var nValues = new[] { 10, 100, 1000, 10000 };
            var lookupValues = new[] { 10, 100, 1000, 10000 };

            foreach (var n in nValues)
            {
                foreach (var lookupCount in lookupValues)
                {
                    N = n;
                    LookupCount = lookupCount;
                    
                    Console.WriteLine($"\nRunning for N={N}, LookupCount={LookupCount}");
                    
                    Setup();
                    
                    _iterationCount = 0;
                    
                    for (int i = 0; i < WarmupIterations + ActualIterations; i++)
                    {
                        IterationSetup();
                        
                        StringList();
                        IterationCleanup();
                        
                        StringHashSet();
                        IterationCleanup();
                        
                        StringSortedSet();
                        IterationCleanup();
                        
                        StringDictionary();
                        IterationCleanup();
                        
                        StringSortedDictionary();
                        IterationCleanup();
                        
                        StringConcurrentDictionary();
                        IterationCleanup();
                        
                        StringImmutableList();
                        IterationCleanup();
                        
                        StringImmutableHashSet();
                        IterationCleanup();
                        
                        StringArray();
                        IterationCleanup();
                    }
                }
            }
        }

        private void ConfigureProcessForBenchmarking()
        {
            try
            {
                var currentProcess = Process.GetCurrentProcess();
                
                // Set real-time priority for maximum consistency
                currentProcess.PriorityClass = ProcessPriorityClass.RealTime;
                
                Console.WriteLine("✓ Process configured for benchmarking:");
                Console.WriteLine($"  - Priority: {currentProcess.PriorityClass}");
                Console.WriteLine($"  - Process ID: {currentProcess.Id}");
                Console.WriteLine($"  - Processor affinity: All cores (natural scheduling)");
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"⚠ Warning: Could not configure process settings: {ex.Message}");
                Console.WriteLine("Benchmarks will continue but may have more variance.");
                Console.WriteLine();
            }
        }

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

        public void IterationSetup()
        {
            _iterationCount++;
        }

        private void LogDetailedResults(string collectionType, int n, int lookupCount, TimeSpan creationTime, TimeSpan lookupTime, TimeSpan totalTime)
        {
            // Only log results after warmup iterations are complete
            if (_iterationCount <= WarmupIterations)
            {
                return; // Skip logging during warmup
            }

            // Calculate processor overhead (time spent on other activities)
            var overheadTime = totalTime - (creationTime + lookupTime);

            // Get the project directory (4 levels up from bin/Release/net9.0)
            var projectDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)?.Parent?.Parent?.Parent?.FullName 
                                 ?? AppDomain.CurrentDomain.BaseDirectory;
            var logPath = Path.Combine(projectDirectory, "result", "detailed_results_string.txt");
            
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
                        writer.WriteLine("CollectionType,N,LookupCount,CreationTime_μs,LookupTime_μs,TotalTime_μs,OverheadTime_μs");
                    }
                    writer.WriteLine($"{collectionType},{n},{lookupCount},{creationTime.TotalMicroseconds:F2},{lookupTime.TotalMicroseconds:F2},{totalTime.TotalMicroseconds:F2},{overheadTime.TotalMicroseconds:F2}");
                    
                    // Force write to disk
                    writer.Flush();
                }
                catch (Exception ex)
                {
                    // If there's an issue, write to a backup location we can debug
                    var backupPath = Path.Combine(Path.GetTempPath(), "benchmark_debug_string.txt");
                    Directory.CreateDirectory(Path.GetDirectoryName(backupPath));
                    File.AppendAllText(backupPath, $"ERROR: {ex.Message}\n");
                }
            }
        }

        public void IterationCleanup()
        {
            // Force garbage collection between iterations to prevent memory pressure interference
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        public void StringList()
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
        }

        public void StringHashSet()
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
        }

        public void StringSortedSet()
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
        }

        public void StringDictionary()
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
        }

        public void StringSortedDictionary()
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
        }

        public void StringConcurrentDictionary()
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
        }

        public void StringImmutableList()
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
        }

        public void StringImmutableHashSet()
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
        }

        public void StringArray()
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
        }
    }
}
