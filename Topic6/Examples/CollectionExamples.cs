using System;
using System.Collections.Generic;

public class CollectionExamples
{
    public static void DictionaryExample()
    {
        // Create a dictionary to store student grades
        var studentGrades = new Dictionary<string, double>();

        // Add students and their grades
        studentGrades.Add("Alice", 95.5);
        studentGrades.Add("Bob", 89.0);
        studentGrades["Charlie"] = 92.3; // Using the indexer

        // Access and print a grade
        Console.WriteLine($"Bob's grade: {studentGrades["Bob"]}");

        // Iterate over the dictionary
        foreach (var student in studentGrades)
        {
            Console.WriteLine($"{student.Key}: {student.Value}");
        }
    }

    public static void HashSetExample()
    {
        // Create a set of unique tags
        var tags = new HashSet<string>();

        // Add tags
        tags.Add("C#");
        tags.Add("Programming");
        tags.Add("Development");
        tags.Add("C#"); // This will not be added again

        Console.WriteLine(string.Join(", ", tags)); // Outputs: C#, Programming, Development

        // Check for the existence of a tag
        if (tags.Contains("Programming"))
        {
            Console.WriteLine("The 'Programming' tag exists.");
        }
    }
}
