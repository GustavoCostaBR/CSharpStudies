using JsonStudies.Models;

namespace JsonStudies.Adapters;

public class InMemoryHierarchyAdapter
{
    public (Page Page, Dictionary<Guid, Field> Index) BuildIndexedTree(Page page)
    {
        var index = new Dictionary<Guid, Field>();

        // Traverse and index
        foreach (var section in page.Sections)
        {
            foreach (var field in section.Fields)
            {
                index[field.Id] = field;
            }

            foreach (var card in section.Cards)
            {
                foreach (var field in card.Fields)
                {
                    index[field.Id] = field;
                }
            }
        }

        return (page, index);
    }

    public Field? TryGetField(Dictionary<Guid, Field> index, Guid id)
    {
        return index.TryGetValue(id, out var field) ? field : null;
    }
}
