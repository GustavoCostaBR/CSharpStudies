using System;
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
}
