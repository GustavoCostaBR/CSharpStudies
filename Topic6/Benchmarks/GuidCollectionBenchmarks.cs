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
    public class GuidCollectionBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N;

        [Params(10, 100, 1000, 10000)]
        public int LookupCount;

        private Guid[] guidSourceData = null!;
        private Guid[] guidLookupItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            guidSourceData = Enumerable.Range(1, N).Select(i => Guid.NewGuid()).ToArray();
            guidLookupItems = Enumerable.Range(1, LookupCount).Select(i => Guid.NewGuid()).ToArray();
        }

        [Benchmark]
        public TimeSpan GuidList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<Guid>(guidSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidList_LookupTime()
        {
            var collection = new List<Guid>(guidSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<Guid>(guidSourceData);
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<Guid>(guidSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidHashSet_LookupTime()
        {
            var collection = new HashSet<Guid>(guidSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<Guid>(guidSourceData);
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<Guid>(guidSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedSet_LookupTime()
        {
            var collection = new SortedSet<Guid>(guidSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<Guid>(guidSourceData);
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = guidSourceData.ToDictionary(x => x, x => true);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidDictionary_LookupTime()
        {
            var collection = guidSourceData.ToDictionary(x => x, x => true);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = guidSourceData.ToDictionary(x => x, x => true);
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedDictionary_LookupTime()
        {
            var collection = new SortedDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidSortedDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidConcurrentDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidConcurrentDictionary_LookupTime()
        {
            var collection = new ConcurrentDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidConcurrentDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<Guid, bool>(guidSourceData.ToDictionary(x => x, x => true));
            foreach (var item in guidLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(guidSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableList_LookupTime()
        {
            var collection = ImmutableList.CreateRange(guidSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(guidSourceData);
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(guidSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableHashSet_LookupTime()
        {
            var collection = ImmutableHashSet.CreateRange(guidSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidImmutableHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(guidSourceData);
            foreach (var item in guidLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidArray_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = guidSourceData.ToArray();
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidArray_LookupTime()
        {
            var collection = guidSourceData.ToArray();
            var sw = Stopwatch.StartNew();
            foreach (var item in guidLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan GuidArray_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = guidSourceData.ToArray();
            foreach (var item in guidLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
