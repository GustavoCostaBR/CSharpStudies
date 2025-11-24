using JsonStudies.Benchmarks;
using JsonStudies.Demo;

// See https://aka.ms/new-console-template for more information

var mode = args.FirstOrDefault()?.ToLowerInvariant() ?? ProgramModes.Demo;

switch (mode)
{
    case ProgramModes.Benchmarks:
        HierarchyBenchmarks.Run();
        break;
    case ProgramModes.MassiveBenchmarks:
    case "massive-benchmarks":
        MassiveHierarchyBenchmarks.Run();
        break;
    default:
        HierarchyDemo.Run();
        break;
}
