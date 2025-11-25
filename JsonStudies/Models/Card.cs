namespace JsonStudies.Models;

public sealed class Card
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
        Fields = [];
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Field> Fields { get; set; }
}
