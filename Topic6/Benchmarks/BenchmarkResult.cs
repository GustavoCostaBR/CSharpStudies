using System;

namespace Benchmarks
{
    public struct BenchmarkResult
    {
        public int N { get; set; }
        public int LookupCount { get; set; }
        public TimeSpan CreationTime { get; set; }
        public TimeSpan LookupTime { get; set; }
        public TimeSpan TotalTime { get; set; }
        
        public override string ToString()
        {
            return $"N={N}, LookupCount={LookupCount} | Creation: {CreationTime.TotalMilliseconds:F2}ms, Lookup: {LookupTime.TotalMilliseconds:F2}ms, Total: {TotalTime.TotalMilliseconds:F2}ms";
        }
    }
}
