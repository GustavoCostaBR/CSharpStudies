using JsonStudies.Adapters;
using JsonStudies.Helpers;
using JsonStudies.Models;
using JsonStudies.SampleData;

namespace JsonStudies.Demo;

public static class HierarchyDemo
{
    public static void Run()
    {
        var page = SampleHierarchyFactory.Create(sectionsPerPage: 2, cardsPerSection: 2, fieldsPerContainer: 3, seed: 42);
        var jsonAdapter = new JsonHierarchyAdapter();

        Console.WriteLine("=== Demo: System.Text.Json ===");
        var jsonPayload = jsonAdapter.Serialize(page);
        Console.WriteLine($"JSON length: {jsonPayload.Length}");
        Console.WriteLine(jsonPayload); // Print JSON for inspection
        var rehydratedJsonPage = jsonAdapter.Deserialize(jsonPayload);
        var jsonTraversalCount = PageHelper.TraverseFields(rehydratedJsonPage).Count();
        Console.WriteLine($"Fields traversed: {jsonTraversalCount}");

        Console.WriteLine("\n=== Demo: In-memory index ===");
        var indexedPage = new IndexedPage(page);
        var targetFieldId = page.Sections.First().Cards.First().Fields.First().Id;
        var found = indexedPage.Fields.TryGetValue(targetFieldId, out var field);
        Console.WriteLine(!found
            ? "Field not found"
            : $"Field found: {field!.Name} -> {field.Value}");
    }
}

