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
    public class StringCollectionBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N;

        [Params(10, 100, 1000, 10000)]
        public int LookupCount;

        private string[] stringSourceData = null!;
        private string[] stringLookupItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            stringSourceData = Enumerable.Range(1, N).Select(i => $"Item_{random.Next()}").Distinct().Take(N).ToArray();
            stringLookupItems = Enumerable.Range(1, LookupCount).Select(i => $"Item_{random.Next()}").ToArray();
        }

        [Benchmark]
        public TimeSpan StringList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<string>(stringSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringList_LookupTime()
        {
            var collection = new List<string>(stringSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<string>(stringSourceData);
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<string>(stringSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringHashSet_LookupTime()
        {
            var collection = new HashSet<string>(stringSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<string>(stringSourceData);
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<string>(stringSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedSet_LookupTime()
        {
            var collection = new SortedSet<string>(stringSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<string>(stringSourceData);
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = stringSourceData.ToDictionary(x => x, x => true);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringDictionary_LookupTime()
        {
            var collection = stringSourceData.ToDictionary(x => x, x => true);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = stringSourceData.ToDictionary(x => x, x => true);
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedDictionary_LookupTime()
        {
            var collection = new SortedDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringSortedDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringConcurrentDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringConcurrentDictionary_LookupTime()
        {
            var collection = new ConcurrentDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringConcurrentDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<string, bool>(stringSourceData.ToDictionary(x => x, x => true));
            foreach (var item in stringLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(stringSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableList_LookupTime()
        {
            var collection = ImmutableList.CreateRange(stringSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(stringSourceData);
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(stringSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableHashSet_LookupTime()
        {
            var collection = ImmutableHashSet.CreateRange(stringSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringImmutableHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(stringSourceData);
            foreach (var item in stringLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringArray_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = stringSourceData.ToArray();
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringArray_LookupTime()
        {
            var collection = stringSourceData.ToArray();
            var sw = Stopwatch.StartNew();
            foreach (var item in stringLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan StringArray_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = stringSourceData.ToArray();
            foreach (var item in stringLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
