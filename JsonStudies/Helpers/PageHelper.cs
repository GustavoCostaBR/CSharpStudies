using System;
using System.Collections.Generic;
using JsonStudies.Models;

namespace JsonStudies.Helpers;

/// <summary>
/// Provides traversal and mutation helpers for a <see cref="Page"/> instance.
/// </summary>
public static class PageHelper
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
        foreach (var section in page.Sections)
        {
            foreach (var field in section.Fields)
            {
                if (field.Id == fieldId)
                {
                    field.Value = newValue;
                    return page;
                }
            }

            foreach (var card in section.Cards)
            {
                foreach (var field in card.Fields)
                {
                    if (field.Id == fieldId)
                    {
                        field.Value = newValue;
                        return page;
                    }
                }
            }
        }

        return page;
    }

    public static Page ReplaceFieldValueFast(Page page, FieldLocation location, string newValue)
    {
        var section = page.Sections[location.SectionIndex];

        if (location.CardIndex.HasValue)
        {
            var card = section.Cards[location.CardIndex.Value];
            card.Fields[location.FieldIndex].Value = newValue;
        }
        else
        {
            section.Fields[location.FieldIndex].Value = newValue;
        }

        return page;
    }

    public static void MutateFieldValue(Page page, Guid fieldId, string newValue)
    {
        ReplaceFieldValue(page, fieldId, newValue);
    }

    public static void ReplaceSection(Page page, Guid sectionId, Section newSection)
    {
        var index = page.Sections.FindIndex(section => section.Id == sectionId);
        if (index == -1)
        {
            return;
        }

        page.Sections[index] = newSection;
    }
}
