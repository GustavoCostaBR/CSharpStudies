using JsonStudies.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace JsonStudies.Adapters;

public class YamlHierarchyAdapter
{
    private readonly ISerializer _serializer;
    private readonly IDeserializer _deserializer;

    public YamlHierarchyAdapter()
    {
        _serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        _deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();
    }

    public string Serialize(Page page)
    {
        return _serializer.Serialize(page);
    }

    public Page Deserialize(string yaml)
    {
        try
        {
            return _deserializer.Deserialize<Page>(yaml);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"YAML Deserialization Error: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"Inner: {ex.InnerException.Message}");
            }
            throw;
        }
    }
}
