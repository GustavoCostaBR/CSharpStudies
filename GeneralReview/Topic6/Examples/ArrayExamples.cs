using System;

public class ArrayExamples
{
    public static void Main(string[] args)
    {
        Console.WriteLine("--- One-Dimensional Array ---");
        // Create and initialize an array of strings
        string[] programmingLanguages = { "C#", "Python", "JavaScript", "Java" };

        // Access and print the first element
        Console.WriteLine($"First language: {programmingLanguages[0]}"); // Output: C#

        // Modify an element
        programmingLanguages[1] = "TypeScript";
        Console.WriteLine($"Modified second language: {programmingLanguages[1]}");

        // Iterate through the array
        Console.WriteLine("All languages:");
        foreach (string lang in programmingLanguages)
        {
            Console.WriteLine($"- {lang}");
        }

        Console.WriteLine("\n--- Two-Dimensional Array (Matrix) ---");
        // A 2x3 integer matrix (2 rows, 3 columns)
        int[,] matrix = 
        {
            { 10, 20, 30 },
            { 40, 50, 60 }
        };

        // Access an element (row 1, column 2)
        Console.WriteLine($"Element at [1, 2]: {matrix[1, 2]}"); // Output: 60

        // Get dimensions
        Console.WriteLine($"Matrix dimensions: {matrix.GetLength(0)}x{matrix.GetLength(1)}");


        Console.WriteLine("\n--- Jagged Array (Array of Arrays) ---");
        // A jagged array holding scores for different teams
        int[][] teamScores = new int[3][];
        teamScores[0] = new int[] { 95, 88 };
        teamScores[1] = new int[] { 75, 82, 91 };
        teamScores[2] = new int[] { 99 };

        // Print scores for the second team
        Console.WriteLine("Scores for Team 2:");
        foreach (int score in teamScores[1])
        {
            Console.WriteLine(score);
        }

        // Access a specific score
        Console.WriteLine($"First score of Team 1: {teamScores[0][0]}"); // Output: 95
    }
}
