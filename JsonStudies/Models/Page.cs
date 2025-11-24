namespace JsonStudies.Models;

public sealed record Page
{
    public Page(Guid id, string name, List<Section> sections)
    {
        Id = id;
        Name = name;
        Sections = sections;
    }

    // Parameterless constructor for YamlDotNet
    public Page() 
    {
        Id = Guid.Empty;
        Name = string.Empty;
        Sections = new List<Section>();
    }

    public Guid Id { get; init; }
    public string Name { get; init; }
    public List<Section> Sections { get; init; }
}
