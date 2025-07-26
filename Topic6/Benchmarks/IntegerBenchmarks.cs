using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Engines;

namespace Benchmarks
{
    [SimpleJob(RunStrategy.Monitoring, warmupCount: IntegerBenchmarks.WarmupIterations, iterationCount: IntegerBenchmarks.ActualIterations)]
    [MemoryDiagnoser]
    public class IntegerBenchmarks
    {
        // Benchmark iteration constants
        public const int WarmupIterations = 5;
        public const int ActualIterations = 50;

        [Params(10, 100, 1000, 10000)]
        public int N { get; set; }

        [Params(10, 100, 1000, 10000)]
        public int LookupCount { get; set; }

        private int[] intSourceData = null!;
        private int[] intLookupItems = null!;
        private static readonly object _fileLock = new object();
        private int _iterationCount = 0;
        private Process _currentProcess = null!;
        private ProcessorAffinity _originalAffinity;
        private ThreadPriority _originalPriority;

        private struct ProcessorAffinity
        {
            public IntPtr Mask;
            public ProcessPriorityClass Priority;
        }

        [GlobalSetup]
        public void Setup()
        {
            // Set processor affinity to isolate to a single core to reduce variance
            _currentProcess = Process.GetCurrentProcess();
            _originalAffinity = new ProcessorAffinity
            {
                Mask = _currentProcess.ProcessorAffinity,
                Priority = _currentProcess.PriorityClass
            };

            try
            {
                // Try to isolate to the first available processor core
                _currentProcess.ProcessorAffinity = new IntPtr(1);
                _currentProcess.PriorityClass = ProcessPriorityClass.High;
                
                // Set thread priority for better isolation
                _originalPriority = Thread.CurrentThread.Priority;
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
            }
            catch (Exception ex)
            {
                // If we can't set affinity (permissions), continue without it
                Console.WriteLine($"Warning: Could not set processor affinity: {ex.Message}");
            }

            var random = new Random(42);
            intSourceData = Enumerable.Range(1, N).Select(i => random.Next()).Distinct().Take(N).ToArray();
            intLookupItems = Enumerable.Range(1, LookupCount).Select(i => random.Next()).ToArray();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            try
            {
                // Restore original processor affinity and priority
                _currentProcess.ProcessorAffinity = _originalAffinity.Mask;
                _currentProcess.PriorityClass = _originalAffinity.Priority;
                Thread.CurrentThread.Priority = _originalPriority;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not restore processor affinity: {ex.Message}");
            }
        }

        [IterationSetup]
        public void IterationSetup()
        {
            _iterationCount++;
            
            // Force garbage collection and wait for finalizers to complete
            // This reduces GC interference during measurements
            GC.Collect(2, GCCollectionMode.Forced, true);
            GC.WaitForPendingFinalizers();
            GC.Collect(2, GCCollectionMode.Forced, true);
            
            // Small delay to let system settle
            Thread.Sleep(1);
        }

        private void LogDetailedResults(string collectionType, int n, int lookupCount, TimeSpan creationTime, TimeSpan lookupTime, TimeSpan totalTime)
        {
            // Only log results after warmup iterations are complete
            if (_iterationCount <= WarmupIterations)
            {
                return; // Skip logging during warmup
            }

            // Use a hardcoded absolute path to ensure we can find it
            var logPath = @"C:\Users\Gustavo\Documents\Projetos\Estudos_Csharp\CSharpStudies\Topic6\Benchmarks\BenchmarkDotNet.Artifacts\detailed_results_integer.txt";
            
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
                    var backupPath = @"C:\temp\benchmark_debug_integer.txt";
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

        // THEORY: Why Creation Time Varies with Lookup Count
        // =================================================
        // 1. **Memory Layout Preparation**: The JIT compiler may optimize memory layout 
        //    differently based on how the collection will be used (more lookups = different optimization)
        // 2. **CPU Cache State**: Previous benchmark iterations affect CPU cache state, 
        //    influencing memory allocation patterns
        // 3. **Garbage Collection Pressure**: Different lookup counts create different GC pressure,
        //    affecting when GC runs during creation phase
        // 4. **Memory Fragmentation**: Heap fragmentation from previous iterations affects 
        //    allocation patterns for new collections
        // 5. **Branch Prediction**: CPU branch predictor state varies based on previous operations
        // 6. **Memory Pre-allocation**: Some collections may pre-allocate based on expected usage patterns

        [Benchmark]
        public double IntList()
        {
            // Pre-warm CPU caches and branch predictor with a tiny operation
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = new List<int>(intSourceData);
            creationSw.Stop();
            
            // Memory barrier AFTER stopping creation timer to ensure proper ordering
            // without affecting the measurement
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            // Calculate total time as sum of measured components for consistency
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            // Clear reference to help GC
            collection = null;
            
            LogDetailedResults("List", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntHashSet()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = new HashSet<int>(intSourceData);
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("HashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntSortedSet()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedSet<int>(intSourceData);
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("SortedSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntDictionary()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = intSourceData.ToDictionary(x => x, x => true);
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("Dictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntSortedDictionary()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = new SortedDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("SortedDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntConcurrentDictionary()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("ConcurrentDictionary", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntImmutableList()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(intSourceData);
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("ImmutableList", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntImmutableHashSet()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(intSourceData);
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("ImmutableHashSet", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }

        [Benchmark]
        public double IntArray()
        {
            var dummy = intSourceData[0];
            
            var creationSw = Stopwatch.StartNew();
            var collection = intSourceData.ToArray();
            creationSw.Stop();
            
            Thread.MemoryBarrier();
            
            var lookupSw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            lookupSw.Stop();
            
            var totalTime = creationSw.Elapsed + lookupSw.Elapsed;
            
            collection = null;
            
            LogDetailedResults("Array", N, LookupCount, creationSw.Elapsed, lookupSw.Elapsed, totalTime);
            return totalTime.TotalMicroseconds;
        }
    }
}
