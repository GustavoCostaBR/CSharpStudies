using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    // Custom reference type for testing
    public class Customer : IEquatable<Customer>, IComparable<Customer>
    {
        public string Id { get; }
        public string Name { get; }
        
        public Customer(string id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public bool Equals(Customer? other) => other != null && Id == other.Id;
        public override bool Equals(object? obj) => Equals(obj as Customer);
        public override int GetHashCode() => Id?.GetHashCode() ?? 0;
        
        public int CompareTo(Customer? other)
        {
            if (other == null) return 1;
            return string.Compare(Id, other.Id, StringComparison.Ordinal);
        }
    }

    [MemoryDiagnoser]
    public class CustomReferenceTypeBenchmarks
    {
        [Params(10, 100, 1000, 10000)]
        public int N;

        [Params(10, 100, 1000, 10000)]
        public int LookupCount;

        private Customer[] customerSourceData = null!;
        private Customer[] customerLookupItems = null!;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            customerSourceData = Enumerable.Range(1, N).Select(i => new Customer($"CUST_{random.Next()}", $"Customer {i}")).ToArray();
            customerLookupItems = Enumerable.Range(1, LookupCount).Select(i => new Customer($"CUST_{random.Next()}", $"Customer {i}")).ToArray();
        }

        [Benchmark]
        public TimeSpan CustomerList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<Customer>(customerSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerList_LookupTime()
        {
            var collection = new List<Customer>(customerSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new List<Customer>(customerSourceData);
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<Customer>(customerSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerHashSet_LookupTime()
        {
            var collection = new HashSet<Customer>(customerSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new HashSet<Customer>(customerSourceData);
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<Customer>(customerSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedSet_LookupTime()
        {
            var collection = new SortedSet<Customer>(customerSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedSet<Customer>(customerSourceData);
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = customerSourceData.ToDictionary(x => x, x => true);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerDictionary_LookupTime()
        {
            var collection = customerSourceData.ToDictionary(x => x, x => true);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = customerSourceData.ToDictionary(x => x, x => true);
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedDictionary_LookupTime()
        {
            var collection = new SortedDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerSortedDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new SortedDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerConcurrentDictionary_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerConcurrentDictionary_LookupTime()
        {
            var collection = new ConcurrentDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerConcurrentDictionary_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = new ConcurrentDictionary<Customer, bool>(customerSourceData.ToDictionary(x => x, x => true));
            foreach (var item in customerLookupItems)
                _ = collection.ContainsKey(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableList_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(customerSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableList_LookupTime()
        {
            var collection = ImmutableList.CreateRange(customerSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableList_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableList.CreateRange(customerSourceData);
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableHashSet_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(customerSourceData);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableHashSet_LookupTime()
        {
            var collection = ImmutableHashSet.CreateRange(customerSourceData);
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerImmutableHashSet_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = ImmutableHashSet.CreateRange(customerSourceData);
            foreach (var item in customerLookupItems)
                _ = collection.Contains(item);
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerArray_CreationTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = customerSourceData.ToArray();
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerArray_LookupTime()
        {
            var collection = customerSourceData.ToArray();
            var sw = Stopwatch.StartNew();
            foreach (var item in customerLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }

        [Benchmark]
        public TimeSpan CustomerArray_TotalTime()
        {
            var sw = Stopwatch.StartNew();
            var collection = customerSourceData.ToArray();
            foreach (var item in customerLookupItems)
                _ = Array.IndexOf(collection, item) >= 0;
            sw.Stop();
            return sw.Elapsed;
        }
    }
}
