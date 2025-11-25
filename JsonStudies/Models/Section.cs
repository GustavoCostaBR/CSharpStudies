namespace JsonStudies.Models;

public sealed class Section
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

    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Card> Cards { get; set; }
    public List<Field> Fields { get; set; }
}
