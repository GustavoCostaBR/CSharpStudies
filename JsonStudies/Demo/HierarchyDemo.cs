using JsonStudies.Adapters;
using JsonStudies.Operations;
using JsonStudies.SampleData;

namespace JsonStudies.Demo;

public static class HierarchyDemo
{
    public static void Run()
    {
        var page = SampleHierarchyFactory.Create(sectionsPerPage: 2, cardsPerSection: 2, fieldsPerContainer: 3, seed: 42);
        var jsonAdapter = new JsonHierarchyAdapter();
        var yamlAdapter = new YamlHierarchyAdapter();
        var inMemoryAdapter = new InMemoryHierarchyAdapter();

        Console.WriteLine("=== Demo: System.Text.Json ===");
        var jsonPayload = jsonAdapter.Serialize(page);
        Console.WriteLine($"JSON length: {jsonPayload.Length}");
        var rehydratedJsonPage = jsonAdapter.Deserialize(jsonPayload);
        var jsonTraversalCount = HierarchyOperations.TraverseFields(rehydratedJsonPage).Count();
        Console.WriteLine($"Fields traversed: {jsonTraversalCount}");

        Console.WriteLine("\n=== Demo: YAML ===");
        var yamlPayload = yamlAdapter.Serialize(page);
        Console.WriteLine($"YAML length: {yamlPayload.Length}");
        var rehydratedYamlPage = yamlAdapter.Deserialize(yamlPayload);
        var yamlTraversalCount = HierarchyOperations.TraverseFields(rehydratedYamlPage).Count();
        Console.WriteLine($"Fields traversed: {yamlTraversalCount}");

        Console.WriteLine("\n=== Demo: In-memory index ===");
        var (_, fieldIndex) = inMemoryAdapter.BuildIndexedTree(page);
        var targetFieldId = page.Sections.First().Cards.First().Fields.First().Id;
        var field = inMemoryAdapter.TryGetField(fieldIndex, targetFieldId);
        Console.WriteLine(field is null
            ? "Field not found"
            : $"Field found: {field.Name} -> {field.Value}");
    }
}

