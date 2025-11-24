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

    public static Page ReplaceFieldValueFast(Page page, FieldLocation location, string newValue)
    {
        // 1. Get the Section
        var oldSection = page.Sections[location.SectionIndex];
        Section newSection;

        if (location.CardIndex.HasValue)
        {
            // It's in a Card
            int cIdx = location.CardIndex.Value;
            var oldCard = oldSection.Cards[cIdx];
            
            // Replace Field in Card
            var oldField = oldCard.Fields[location.FieldIndex];
            var newField = oldField with { Value = newValue };
            
            // Create new Card list
            var newCardFields = new List<Field>(oldCard.Fields);
            newCardFields[location.FieldIndex] = newField;
            var newCard = oldCard with { Fields = newCardFields };

            // Create new Section Card list
            var newSectionCards = new List<Card>(oldSection.Cards);
            newSectionCards[cIdx] = newCard;
            
            newSection = oldSection with { Cards = newSectionCards };
        }
        else
        {
            // It's directly in the Section
            var oldField = oldSection.Fields[location.FieldIndex];
            var newField = oldField with { Value = newValue };

            var newSectionFields = new List<Field>(oldSection.Fields);
            newSectionFields[location.FieldIndex] = newField;
            
            newSection = oldSection with { Fields = newSectionFields };
        }

        // Create new Page
        var newSections = new List<Section>(page.Sections);
        newSections[location.SectionIndex] = newSection;
        
        return page with { Sections = newSections };
    }

    public static void MutateFieldValue(Page page, Guid fieldId, string newValue)
    {
        foreach (var field in TraverseFields(page))
        {
            if (field.Id == fieldId)
            {
                field.Value = newValue;
                return;
            }
        }
    }

    public static void ReplaceSection(Page page, Guid sectionId, Section newSection)
    {
        var index = page.Sections.FindIndex(s => s.Id == sectionId);
        if (index != -1)
        {
            page.Sections[index] = newSection;
        }
    }
}
