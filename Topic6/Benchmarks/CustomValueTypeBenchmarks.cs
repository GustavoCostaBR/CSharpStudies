using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    // Custom value type for testing
    public struct ProductId : IEquatable<ProductId>, IComparable<ProductId>
    {
        public int Value { get; }
        
        public ProductId(int value) => Value = value;
        
        public bool Equals(ProductId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is ProductId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();
        public static implicit operator ProductId(int value) => new(value);
        
        public int CompareTo(ProductId other) => Value.CompareTo(other.Value);
    }

    [MemoryDiagnoser]
    public class CustomValueTypeBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N;

        [Params(10, 100, 1000, 10000)]
        public int LookupCount;

        private ProductId[] productIdSourceData = null!;
        private ProductId[] productIdLookupItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            productIdSourceData = Enumerable.Range(1, N).Select(i => new ProductId(random.Next())).Distinct().Take(N).ToArray();
            productIdLookupItems = Enumerable.Range(1, LookupCount).Select(i => new ProductId(random.Next())).ToArray();
        }

        // Regular ProductId tests
        [Benchmark]
        public TimeSpan ProductIdList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<ProductId>(productIdSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdList_LookupTime()
        {
            var collection = new List<ProductId>(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<ProductId>(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        // ProductId with 'ref' parameter tests
        [Benchmark]
        public TimeSpan ProductIdList_LookupTime_Ref()
        {
            var collection = new List<ProductId>(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (ref readonly var item in productIdLookupItems.AsSpan())
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdList_TotalTime_Ref()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<ProductId>(productIdSourceData);
            foreach (ref readonly var item in productIdLookupItems.AsSpan())
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        // ProductId with 'in' parameter tests (using helper methods)
        [Benchmark]
        public TimeSpan ProductIdList_LookupTime_In()
        {
            var collection = new List<ProductId>(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = ContainsIn(collection, in item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdList_TotalTime_In()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<ProductId>(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = ContainsIn(collection, in item);
            sw.Stop();
            return sw.Elapsed;
        }

        private static bool ContainsIn(List<ProductId> collection, in ProductId item)
        {
            return collection.Contains(item);
        }

        [Benchmark]
        public TimeSpan ProductIdHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<ProductId>(productIdSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdHashSet_LookupTime()
        {
            var collection = new HashSet<ProductId>(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<ProductId>(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<ProductId>(productIdSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedSet_LookupTime()
        {
            var collection = new SortedSet<ProductId>(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<ProductId>(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = productIdSourceData.ToDictionary(x => x, x => true);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdDictionary_LookupTime()
        {
            var collection = productIdSourceData.ToDictionary(x => x, x => true);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = productIdSourceData.ToDictionary(x => x, x => true);
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedDictionary_LookupTime()
        {
            var collection = new SortedDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdSortedDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdConcurrentDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdConcurrentDictionary_LookupTime()
        {
            var collection = new ConcurrentDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdConcurrentDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<ProductId, bool>(productIdSourceData.ToDictionary(x => x, x => true));
            foreach (var item in productIdLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(productIdSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableList_LookupTime()
        {
            var collection = ImmutableList.CreateRange(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(productIdSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableHashSet_LookupTime()
        {
            var collection = ImmutableHashSet.CreateRange(productIdSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdImmutableHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(productIdSourceData);
            foreach (var item in productIdLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdArray_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = productIdSourceData.ToArray();
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdArray_LookupTime()
        {
            var collection = productIdSourceData.ToArray();
            var sw = Stopwatch.StartNew();
            foreach (var item in productIdLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan ProductIdArray_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = productIdSourceData.ToArray();
            foreach (var item in productIdLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
