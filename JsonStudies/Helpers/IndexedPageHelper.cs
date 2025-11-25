using System;
using JsonStudies.Models;

namespace JsonStudies.Helpers;

/// <summary>
/// Provides indexed lookup and mutation helpers for <see cref="IndexedPage"/>.
/// </summary>
public static class IndexedPageHelper
{
    public static bool UpdateFieldValue(IndexedPage indexedPage, Guid fieldId, string newValue)
    {
        if (!indexedPage.Fields.TryGetValue(fieldId, out var field))
        {
            return false;
        }

        field.Value = newValue;
        return true;
    }

    public static bool ReplaceSection(IndexedPage indexedPage, Guid sectionId, Section newSection)
    {
        if (!indexedPage.Sections.TryGetValue(sectionId, out var oldSection))
        {
            return false;
        }

        var sectionIndex = indexedPage.Root.Sections.IndexOf(oldSection);
        if (sectionIndex == -1)
        {
            return false;
        }

        indexedPage.Root.Sections[sectionIndex] = newSection;
        indexedPage.Sections[sectionId] = newSection;

        RemoveSectionEntries(indexedPage, oldSection);
        AddSectionEntries(indexedPage, newSection);
        return true;
    }

    private static void RemoveSectionEntries(IndexedPage indexedPage, Section section)
    {
        foreach (var field in section.Fields)
        {
            indexedPage.Fields.Remove(field.Id);
        }

        foreach (var card in section.Cards)
        {
            indexedPage.Cards.Remove(card.Id);
            foreach (var field in card.Fields)
            {
                indexedPage.Fields.Remove(field.Id);
            }
        }
    }

    private static void AddSectionEntries(IndexedPage indexedPage, Section section)
    {
        foreach (var field in section.Fields)
        {
            indexedPage.Fields[field.Id] = field;
        }

        foreach (var card in section.Cards)
        {
            indexedPage.Cards[card.Id] = card;
            foreach (var field in card.Fields)
            {
                indexedPage.Fields[field.Id] = field;
            }
        }
    }
}
