namespace JsonStudies.Models;

public sealed record Card
{
    public Card(Guid id, string name, List<Field> fields)
    {
        Id = id;
        Name = name;
        Fields = fields;
    }

    public Card()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Fields = new List<Field>();
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public List<Field> Fields { get; init; }
}
