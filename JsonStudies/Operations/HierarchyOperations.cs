using JsonStudies.Models;

namespace JsonStudies.Operations;

public static class HierarchyOperations
{
    public static IEnumerable<Field> TraverseFields(Page page)
    {
        foreach (var section in page.Sections)
        {
            foreach (var field in section.Fields)
            {
                yield return field;
            }

            foreach (var card in section.Cards)
            {
                foreach (var field in card.Fields)
                {
                    yield return field;
                }
            }
        }
    }

    public static Page ReplaceFieldValue(Page page, Guid fieldId, string newValue)
    {
        // Since records are immutable, we need to rebuild the tree path if we find the field.
        // We will iterate and if any section changes, we create a new Page.

        var newSections = new List<Section>(page.Sections.Count);
        bool pageChanged = false;

        foreach (var section in page.Sections)
        {
            var (newSection, sectionChanged) = ReplaceInSection(section, fieldId, newValue);
            newSections.Add(newSection);
            if (sectionChanged)
            {
                pageChanged = true;
            }
        }

        if (pageChanged)
        {
            return new Page(page.Id, page.Name, newSections);
        }

        return page;
    }

    private static (Section section, bool changed) ReplaceInSection(Section section, Guid fieldId, string newValue)
    {
        bool sectionChanged = false;
        
        // Check fields in Section
        var newFields = new List<Field>(section.Fields.Count);
        foreach (var field in section.Fields)
        {
            if (field.Id == fieldId)
            {
                newFields.Add(field with { Value = newValue });
                sectionChanged = true;
            }
            else
            {
                newFields.Add(field);
            }
        }

        // Check cards
        var newCards = new List<Card>(section.Cards.Count);
        foreach (var card in section.Cards)
        {
            var (newCard, cardChanged) = ReplaceInCard(card, fieldId, newValue);
            newCards.Add(newCard);
            if (cardChanged)
            {
                sectionChanged = true;
            }
        }

        if (sectionChanged)
        {
            return (new Section(section.Id, section.Name, newCards, newFields), true);
        }

        return (section, false);
    }

    private static (Card card, bool changed) ReplaceInCard(Card card, Guid fieldId, string newValue)
    {
        bool cardChanged = false;
        var newFields = new List<Field>(card.Fields.Count);

        foreach (var field in card.Fields)
        {
            if (field.Id == fieldId)
            {
                newFields.Add(field with { Value = newValue });
                cardChanged = true;
            }
            else
            {
                newFields.Add(field);
            }
        }

        if (cardChanged)
        {
            return (new Card(card.Id, card.Name, newFields), true);
        }

        return (card, false);
    }
}
