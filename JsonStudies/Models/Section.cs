namespace JsonStudies.Models;

public sealed record Section
{
    public Section(Guid id, string name, List<Card> cards, List<Field> fields)
    {
        Id = id;
        Name = name;
        Cards = cards;
        Fields = fields;
    }

    public Section()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Cards = [];
        Fields = [];
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public List<Card> Cards { get; init; }
    public List<Field> Fields { get; init; }
}
