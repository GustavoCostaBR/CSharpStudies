using BenchmarkDotNet.Running;
using Benchmarks;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var summary = BenchmarkRunner.Run<CreationAndLookupBenchmarks>();
