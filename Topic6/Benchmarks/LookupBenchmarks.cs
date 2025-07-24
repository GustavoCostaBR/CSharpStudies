using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarks
{
    [MemoryDiagnoser]
    public class LookupBenchmarks
    {
        [Params(100, 1000, 10000)]
        public int N;

        private List<int> list;
        private HashSet<int> hashSet;
        private int[] itemsToLookup;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            var data = Enumerable.Range(1, N).Select(i => random.Next()).Distinct().ToArray();
            list = new List<int>(data);
            hashSet = new HashSet<int>(data);
            itemsToLookup = Enumerable.Range(1, 100).Select(i => random.Next()).ToArray();
        }

        [Benchmark]
        public void ListContains()
        {
            foreach (var item in itemsToLookup)
            {
                list.Contains(item);
            }
        }

        [Benchmark]
        public void HashSetContains()
        {
            foreach (var item in itemsToLookup)
            {
                hashSet.Contains(item);
            }
        }
    }

    [MemoryDiagnoser]
    public class CreationAndLookupBenchmarks
    {
        [Params(100, 1000, 10000, 100000)]
        public int N;

        [Params(10, 100, 1000)]
        public int LookupCount;

        private int[] data;
        private int[] itemsToLookup;

        [GlobalSetup]
        public void Setup()
        {
            var random = new Random(42);
            data = Enumerable.Range(1, N).Select(i => random.Next()).Distinct().ToArray();
            itemsToLookup = Enumerable.Range(1, LookupCount).Select(i => random.Next()).ToArray();
        }

        [Benchmark]
        public void ListCreationAndLookup()
        {
            var list = new List<int>(data);
            foreach (var item in itemsToLookup)
            {
                list.Contains(item);
            }
        }

        [Benchmark]
        public void HashSetCreationAndLookup()
        {
            var hashSet = new HashSet<int>(data);
            foreach (var item in itemsToLookup)
            {
                hashSet.Contains(item);
            }
        }
    }
}
