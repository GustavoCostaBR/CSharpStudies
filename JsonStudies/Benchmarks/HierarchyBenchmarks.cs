using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using JsonStudies.Adapters;
using JsonStudies.Models;
using JsonStudies.Operations;
using JsonStudies.SampleData;

namespace JsonStudies.Benchmarks;

[MemoryDiagnoser]
public class HierarchyBenchmarks
{
    private readonly JsonHierarchyAdapter _jsonAdapter = new();
    private readonly YamlHierarchyAdapter _yamlAdapter = new();
    private readonly InMemoryHierarchyAdapter _inMemoryAdapter = new();

    private Page _page = null!;
    private string _jsonPayload = string.Empty;
    private string _yamlPayload = string.Empty;
    private Guid _existingFieldId;

    [Params(3)] public int Sections { get; set; }
    [Params(3)] public int CardsPerSection { get; set; }
    [Params(5)] public int FieldsPerContainer { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        _page = SampleHierarchyFactory.Create(Sections, CardsPerSection, FieldsPerContainer);
        _jsonPayload = _jsonAdapter.Serialize(_page);
        _yamlPayload = _yamlAdapter.Serialize(_page);
        _existingFieldId = _page.Sections.First().Cards.First().Fields.First().Id;
    }

    [Benchmark]
    public int TraverseFieldsJson()
    {
        var page = _jsonAdapter.Deserialize(_jsonPayload);
        return HierarchyOperations.TraverseFields(page).Count();
    }

    [Benchmark]
    public int TraverseFieldsYaml()
    {
        var page = _yamlAdapter.Deserialize(_yamlPayload);
        return HierarchyOperations.TraverseFields(page).Count();
    }

    [Benchmark]
    public int TraverseFieldsInMemory()
    {
        var (page, _) = _inMemoryAdapter.BuildIndexedTree(_page);
        return HierarchyOperations.TraverseFields(page).Count();
    }

    [Benchmark]
    public Page ReplaceFieldJson()
    {
        var page = _jsonAdapter.Deserialize(_jsonPayload);
        return HierarchyOperations.ReplaceFieldValue(page, _existingFieldId, "new-value");
    }

    [Benchmark]
    public Page ReplaceFieldYaml()
    {
        var page = _yamlAdapter.Deserialize(_yamlPayload);
        return HierarchyOperations.ReplaceFieldValue(page, _existingFieldId, "new-value");
    }

    [Benchmark]
    public Field? LookupInMemory()
    {
        var (_, index) = _inMemoryAdapter.BuildIndexedTree(_page);
        return _inMemoryAdapter.TryGetField(index, _existingFieldId);
    }

    public static void Run() => BenchmarkRunner.Run<HierarchyBenchmarks>();
}

