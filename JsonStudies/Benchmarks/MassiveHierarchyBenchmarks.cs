using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using JsonStudies.Adapters;
using JsonStudies.Helpers;
using JsonStudies.Models;
using JsonStudies.SampleData;
using System.Text.Json.Nodes;

namespace JsonStudies.Benchmarks;

[MemoryDiagnoser]
public class MassiveHierarchyBenchmarks
{
    private readonly JsonHierarchyAdapter _jsonAdapter = new();
    
    private Page _page = null!;
    private string _jsonPayload = string.Empty;
    
    [Params(10)] public int Sections { get; set; }
    [Params(5)] public int CardsPerSection { get; set; }
    [Params(10)] public int FieldsPerContainer { get; set; }

    private Guid _targetFieldId;
    private Guid _targetSectionId;
    private Section _newSection = null!;
    
    // Cached State
    private JsonNode _cachedJsonNode = null!;
    private Page _cachedPoco_NoIndex = null!;
    private IndexedPage _cachedPoco_Indexed = null!;

    [GlobalSetup]
    public void Setup()
    {
        // 1. Create Data
        _page = SampleHierarchyFactory.Create(Sections, CardsPerSection, FieldsPerContainer);
        _jsonPayload = _jsonAdapter.Serialize(_page);

        // Target the last field to force worst-case traversal
        _targetFieldId = _page.Sections.Last().Cards.Last().Fields.Last().Id;
        _targetSectionId = _page.Sections.Last().Id;
        
        // Create a replacement section (simple empty one for benchmark)
        // We use the same ID so that repeated benchmark iterations can find it again.
        _newSection = new Section(_targetSectionId, "Replacement Section", new List<Card>(), new List<Field>());

        // 2. Setup Cached State
        _cachedJsonNode = JsonNode.Parse(_jsonPayload)!;
        _cachedPoco_NoIndex = _jsonAdapter.Deserialize(_jsonPayload);
        _cachedPoco_Indexed = new IndexedPage(_jsonAdapter.Deserialize(_jsonPayload));
    }

    // =================================================================================================
    // GROUP 1: FULL WORKFLOW (Parse -> Modify Field -> Serialize)
    // =================================================================================================

    [Benchmark(Description = "1. Workflow: JSON DOM")]
    public string Workflow_JsonDOM()
    {
        var root = JsonNode.Parse(_jsonPayload)!;
        var targetNode = FindFieldInJsonNode(root, _targetFieldId);
        if (targetNode != null) targetNode["Value"] = "UPDATED_VALUE";
        return root.ToJsonString();
    }

    [Benchmark(Description = "2. Workflow: POCO (No Index)")]
    public string Workflow_Poco_NoIndex()
    {
        var page = _jsonAdapter.Deserialize(_jsonPayload);
        PageHelper.MutateFieldValue(page, _targetFieldId, "UPDATED_VALUE");
        return _jsonAdapter.Serialize(page);
    }

    [Benchmark(Description = "3. Workflow: POCO (Indexed)")]
    public string Workflow_Poco_Indexed()
    {
        var page = _jsonAdapter.Deserialize(_jsonPayload);
        var indexedPage = new IndexedPage(page);
        IndexedPageHelper.UpdateFieldValue(indexedPage, _targetFieldId, "UPDATED_VALUE");
        return _jsonAdapter.Serialize(indexedPage.Root);
    }

    // =================================================================================================
    // GROUP 2: PARSING (From JSON String)
    // =================================================================================================

    [Benchmark(Description = "1. Parse: JSON DOM")]
    public JsonNode Parse_JsonDOM() => JsonNode.Parse(_jsonPayload)!;

    [Benchmark(Description = "2. Parse: POCO (No Index)")]
    public Page Parse_Poco_NoIndex() => _jsonAdapter.Deserialize(_jsonPayload);

    [Benchmark(Description = "3. Parse: POCO (Indexed)")]
    public IndexedPage Parse_Poco_Indexed() => new IndexedPage(_jsonAdapter.Deserialize(_jsonPayload));

    // =================================================================================================
    // GROUP 3: FIELD REPLACEMENT (In-Memory)
    // =================================================================================================

    [Benchmark(Description = "1. Modify Field: JSON DOM")]
    public void ModifyField_JsonDOM()
    {
        var targetNode = FindFieldInJsonNode(_cachedJsonNode, _targetFieldId);
        if (targetNode != null) targetNode["Value"] = "UPDATED_VALUE";
    }

    [Benchmark(Description = "2. Modify Field: POCO (No Index)")]
    public void ModifyField_Poco_NoIndex()
    {
        PageHelper.MutateFieldValue(_cachedPoco_NoIndex, _targetFieldId, "UPDATED_VALUE");
    }

    [Benchmark(Description = "3. Modify Field: POCO (Indexed)")]
    public void ModifyField_Poco_Indexed()
    {
        IndexedPageHelper.UpdateFieldValue(_cachedPoco_Indexed, _targetFieldId, "UPDATED_VALUE");
    }

    // =================================================================================================
    // GROUP 4: SECTION REPLACEMENT (In-Memory)
    // =================================================================================================

    [Benchmark(Description = "1. Modify Section: JSON DOM")]
    public void ModifySection_JsonDOM()
    {
        var sectionsArray = _cachedJsonNode["Sections"] as JsonArray;
        if (sectionsArray != null)
        {
            for (int i = 0; i < sectionsArray.Count; i++)
            {
                if (sectionsArray[i]?["Id"]?.GetValue<Guid>() == _targetSectionId)
                {
                    sectionsArray[i] = System.Text.Json.JsonSerializer.SerializeToNode(_newSection);
                    break;
                }
            }
        }
    }

    [Benchmark(Description = "2. Modify Section: POCO (No Index)")]
    public void ModifySection_Poco_NoIndex()
    {
        PageHelper.ReplaceSection(_cachedPoco_NoIndex, _targetSectionId, _newSection);
    }

    [Benchmark(Description = "3. Modify Section: POCO (Indexed)")]
    public void ModifySection_Poco_Indexed()
    {
        IndexedPageHelper.ReplaceSection(_cachedPoco_Indexed, _targetSectionId, _newSection);
    }

    // =================================================================================================
    // GROUP 5: SERIALIZATION (To JSON String)
    // =================================================================================================

    [Benchmark(Description = "1. Serialize: JSON DOM")]
    public string Serialize_JsonDOM() => _cachedJsonNode.ToJsonString();

    [Benchmark(Description = "2. Serialize: POCO (No Index)")]
    public string Serialize_Poco_NoIndex() => _jsonAdapter.Serialize(_cachedPoco_NoIndex);

    [Benchmark(Description = "3. Serialize: POCO (Indexed)")]
    public string Serialize_Poco_Indexed() => _jsonAdapter.Serialize(_cachedPoco_Indexed.Root);

    // =================================================================================================
    // Helpers
    // =================================================================================================

    private JsonNode? FindFieldInJsonNode(JsonNode node, Guid targetId)
    {
        if (node is JsonObject obj)
        {
            // Check if this is a Field and has the matching ID
            if (obj.ContainsKey("Id") && obj["Id"]?.GetValue<Guid>() == targetId)
            {
                return obj;
            }

            foreach (var property in obj)
            {
                if (property.Value != null)
                {
                    var result = FindFieldInJsonNode(property.Value, targetId);
                    if (result != null) return result;
                }
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
            {
                if (item != null)
                {
                    var result = FindFieldInJsonNode(item, targetId);
                    if (result != null) return result;
                }
            }
        }
        return null;
    }


    public static void Run() => BenchmarkRunner.Run<MassiveHierarchyBenchmarks>();
}
