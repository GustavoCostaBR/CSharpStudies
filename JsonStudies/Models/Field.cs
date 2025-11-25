using System.Text.Json.Serialization;

namespace JsonStudies.Models;

public sealed class Field
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

    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Reference { get; set; }
    public string Description { get; set; }
    public string Value { get; set; }
}
