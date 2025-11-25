using JsonStudies.Benchmarks;
using JsonStudies.Demo;

// See https://aka.ms/new-console-template for more information

var normalizedArgs = args.Select(arg => arg.ToLowerInvariant()).ToArray();
var mode = normalizedArgs.ElementAtOrDefault(0) ?? ProgramModes.Demo;
var benchmarkSelection = normalizedArgs.ElementAtOrDefault(1);

switch (mode)
{
    case ProgramModes.Benchmarks:
        RunBenchmarks(benchmarkSelection);
        break;
    case ProgramModes.MassiveBenchmarks:
    case "massive-benchmarks":
        MassiveHierarchyBenchmarks.Run();
        break;
    default:
        HierarchyDemo.Run();
        break;
}

static void RunBenchmarks(string? selection)
{
    switch (selection)
    {
        case null:
        case "":
        case ProgramModes.HierarchyBenchmarks:
            HierarchyBenchmarks.Run();
            break;
        case ProgramModes.MassiveBenchmarks:
        case "massive-benchmarks":
            MassiveHierarchyBenchmarks.Run();
            break;
        case ProgramModes.AllBenchmarks:
            HierarchyBenchmarks.Run();
            MassiveHierarchyBenchmarks.Run();
            break;
        default:
            Console.WriteLine($"Unknown benchmark selection '{selection}'. Use '{ProgramModes.HierarchyBenchmarks}', '{ProgramModes.MassiveBenchmarks}', or '{ProgramModes.AllBenchmarks}'.");
            break;
    }
}
