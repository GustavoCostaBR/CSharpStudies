using System.Text.Json;
using JsonStudies.Models;

namespace JsonStudies.Adapters;

public class JsonHierarchyAdapter
{
    private static readonly JsonSerializerOptions _options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Serialize(Page page)
    {
        return JsonSerializer.Serialize(page, _options);
    }

    public Page Deserialize(string json)
    {
        var page = JsonSerializer.Deserialize<Page>(json, _options);
        if (page is null)
        {
            throw new InvalidOperationException("Failed to deserialize Page from JSON.");
        }
        return page;
    }
}
