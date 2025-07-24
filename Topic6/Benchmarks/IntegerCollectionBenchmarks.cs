using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class IntegerCollectionBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N;

        [Params(10, 100, 1000, 10000)]
        public int LookupCount;

        private int[] intSourceData = null!;
        private int[] intLookupItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            intSourceData = Enumerable.Range(1, N).Select(i => random.Next()).Distinct().Take(N).ToArray();
            intLookupItems = Enumerable.Range(1, LookupCount).Select(i => random.Next()).ToArray();
        }

        [Benchmark]
        public TimeSpan IntList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<int>(intSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntList_LookupTime()
        {
            var collection = new List<int>(intSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<int>(intSourceData);
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<int>(intSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntHashSet_LookupTime()
        {
            var collection = new HashSet<int>(intSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<int>(intSourceData);
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<int>(intSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedSet_LookupTime()
        {
            var collection = new SortedSet<int>(intSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<int>(intSourceData);
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = intSourceData.ToDictionary(x => x, x => true);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntDictionary_LookupTime()
        {
            var collection = intSourceData.ToDictionary(x => x, x => true);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = intSourceData.ToDictionary(x => x, x => true);
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedDictionary_LookupTime()
        {
            var collection = new SortedDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntSortedDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntConcurrentDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntConcurrentDictionary_LookupTime()
        {
            var collection = new ConcurrentDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntConcurrentDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<int, bool>(intSourceData.ToDictionary(x => x, x => true));
            foreach (var item in intLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(intSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableList_LookupTime()
        {
            var collection = ImmutableList.CreateRange(intSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(intSourceData);
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(intSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableHashSet_LookupTime()
        {
            var collection = ImmutableHashSet.CreateRange(intSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntImmutableHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(intSourceData);
            foreach (var item in intLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntArray_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = intSourceData.ToArray();
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntArray_LookupTime()
        {
            var collection = intSourceData.ToArray();
            var sw = Stopwatch.StartNew();
            foreach (var item in intLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan IntArray_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = intSourceData.ToArray();
            foreach (var item in intLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
