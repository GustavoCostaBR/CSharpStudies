using System.Text.Json.Serialization;

namespace JsonStudies.Models;

public sealed record Field
{
    [JsonConstructor]
    public Field(Guid id, string name, string reference, string description, string value)
    {
        Id = id;
        Name = name;
        Reference = reference;
        Description = description;
        Value = value;
    }

    public Field()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Reference = string.Empty;
        Description = string.Empty;
        Value = string.Empty;
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Reference { get; init; }
    public string Description { get; init; }
    public string Value { get; init; }
}
