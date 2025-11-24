using JsonStudies.Models;

namespace JsonStudies.SampleData;

public static class SampleHierarchyFactory
{
    public static Page Create(int sectionsPerPage, int cardsPerSection, int fieldsPerContainer, int seed = 42)
    {
        var random = new Random(seed);

        // Deterministic Guid generation helper
        Guid NextGuid()
        {
            var b = new byte[16];
            random.NextBytes(b);
            return new Guid(b);
        }

        // Helper to create fields
        List<Field> CreateFieldsDeterministic(int count)
        {
            var fields = new List<Field>(count);
            for (int i = 0; i < count; i++)
            {
                fields.Add(new Field(
                    id: NextGuid(),
                    name: $"Field {i}",
                    reference: $"REF-{random.Next(1000, 9999)}",
                    description: "Generated field description",
                    value: $"Value-{random.Next(100, 999)}"
                ));
            }
            return fields;
        }

        var sections = new List<Section>(sectionsPerPage);
        for (int s = 0; s < sectionsPerPage; s++)
        {
            var cards = new List<Card>(cardsPerSection);
            for (int c = 0; c < cardsPerSection; c++)
            {
                cards.Add(new Card(
                    id: NextGuid(),
                    name: $"Card {c}",
                    fields: CreateFieldsDeterministic(fieldsPerContainer)
                ));
            }

            sections.Add(new Section(
                id: NextGuid(),
                name: $"Section {s}",
                cards: cards,
                fields: CreateFieldsDeterministic(fieldsPerContainer)
            ));
        }

        return new Page(
            id: NextGuid(),
            name: "Generated Page",
            sections: sections
        );
    }
}
