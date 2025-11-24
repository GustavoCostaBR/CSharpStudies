using System.Collections.Generic;

namespace JsonStudies.Models;

public class IndexedPage
{
    public Page Root { get; }
    public Dictionary<Guid, Section> Sections { get; } = new();
    public Dictionary<Guid, Card> Cards { get; } = new();
    public Dictionary<Guid, Field> Fields { get; } = new();

    public IndexedPage(Page root)
    {
        Root = root;
        BuildIndex();
    }

    private void BuildIndex()
    {
        foreach (var section in Root.Sections)
        {
            Sections[section.Id] = section;
            foreach (var field in section.Fields)
            {
                Fields[field.Id] = field;
            }

            foreach (var card in section.Cards)
            {
                Cards[card.Id] = card;
                foreach (var field in card.Fields)
                {
                    Fields[field.Id] = field;
                }
            }
        }
    }

    public void UpdateField(Guid fieldId, string newValue)
    {
        if (Fields.TryGetValue(fieldId, out var field))
        {
            field.Value = newValue;
        }
    }

    public void ReplaceSection(Guid sectionId, Section newSection)
    {
        if (Sections.TryGetValue(sectionId, out var oldSection))
        {
            // 1. Remove old section from Root
            var index = Root.Sections.IndexOf(oldSection);
            if (index != -1)
            {
                Root.Sections[index] = newSection;
            }

            // 2. Remove old section from Index
            Sections.Remove(sectionId);
            foreach (var field in oldSection.Fields)
            {
                Fields.Remove(field.Id);
            }
            foreach (var card in oldSection.Cards)
            {
                Cards.Remove(card.Id);
                foreach (var field in card.Fields)
                {
                    Fields.Remove(field.Id);
                }
            }

            // 3. Add new section to Index
            Sections[newSection.Id] = newSection;
            foreach (var field in newSection.Fields)
            {
                Fields[field.Id] = field;
            }
            foreach (var card in newSection.Cards)
            {
                Cards[card.Id] = card;
                foreach (var field in card.Fields)
                {
                    Fields[field.Id] = field;
                }
            }
        }
    }
}
