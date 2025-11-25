namespace JsonStudies.Models;

public sealed class Page
{
    public Page(Guid id, string name, List<Section> sections)
    {
        Id = id;
        Name = name;
        Sections = sections;
    }

    // Parameterless constructor for serializers
    public Page()
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Sections = [];
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Section> Sections { get; set; }
}
