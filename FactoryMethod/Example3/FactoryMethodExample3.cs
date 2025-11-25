using System.Text.Json;
using static System.Console;

namespace FactoryMethod.Example3;

/*
Example3 Goal: Show a realistic Factory Method use case.
Scenario: A generic DataImporter defines a template Import pipeline (read -> parse -> post-process)
          but delegates creation of the concrete parser to subclasses through a Factory Method.
Why Factory Method here? We want to keep the import algorithm stable (Template Method) while
          letting subclasses decide which concrete parser (product) to use.
Benefits:
 - Avoids giant switch on file type in base class.
 - Easy to add new formats: implement IParser + subclass DataImporter.
 - Common error handling / metrics remain in base Import method.
*/

public interface IParser
{
    ParsedData Parse(string rawContent);
}

public sealed class ParsedData
{
    public IReadOnlyList<IDictionary<string, string>> Rows { get; }
    public ParsedData(IReadOnlyList<IDictionary<string, string>> rows) => Rows = rows;
}

// ----- Concrete Products -----

public sealed class CsvParser : IParser
{
    private readonly char _sep;
    public CsvParser(char sep = ',') => _sep = sep;
    public ParsedData Parse(string rawContent)
    {
        var lines = rawContent.Split(['\n'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (lines.Length == 0) return new ParsedData([]);
        var header = lines[0].Split(_sep, StringSplitOptions.TrimEntries);
        var list = new List<IDictionary<string, string>>();
        for (var i = 1; i < lines.Length; i++)
        {
            var cols = lines[i].Split(_sep, StringSplitOptions.TrimEntries);
            var dict = new Dictionary<string, string>();
            for (var c = 0; c < header.Length && c < cols.Length; c++) dict[header[c]] = cols[c];
            list.Add(dict);
        }
        return new ParsedData(list);
    }
}

public sealed class JsonArrayParser : IParser
{
    public ParsedData Parse(string rawContent)
    {
        var list = new List<IDictionary<string, string>>();
        using var doc = JsonDocument.Parse(rawContent);
        if (doc.RootElement.ValueKind != JsonValueKind.Array) return new ParsedData(list);
        foreach (var element in doc.RootElement.EnumerateArray())
        {
            var dict = new Dictionary<string, string>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = prop.Value.ToString();
            }
            list.Add(dict);
        }
        return new ParsedData(list);
    }
}

// ----- Creator (Template + Factory Method) -----

public abstract class DataImporter
{
    // Template Method (fixed high-level algorithm)
    public ParsedData Import(string source)
    {
        var raw = Read(source); // Could be IO, here simplified.
        var parser = CreateParser(source); // Factory Method
        var data = parser.Parse(raw);
        return PostProcess(data);
    }

    protected abstract IParser CreateParser(string source); // Factory Method

    protected virtual string Read(string source) => source; // In real case: read file / http

    protected virtual ParsedData PostProcess(ParsedData data) => data; // Hook
}

// ----- Concrete Creators -----

public sealed class CsvDataImporter : DataImporter
{
    private readonly char _sep;
    public CsvDataImporter(char sep = ',') => _sep = sep;
    protected override IParser CreateParser(string _) => new CsvParser(_sep);
}

public sealed class JsonDataImporter : DataImporter
{
    protected override IParser CreateParser(string _) => new JsonArrayParser();

    // Example of overriding a hook
    protected override ParsedData PostProcess(ParsedData data)
    {
        // Add a synthetic row count row (demo only)
        var meta = new Dictionary<string, string> { ["_meta"] = $"rows={data.Rows.Count}" };
        var extended = data.Rows.ToList();
        extended.Add(meta);
        return new ParsedData(extended);
    }
}

public static class Example3Demo
{
    public static void Run()
    {
        WriteLine("\n================ Factory Method Example3 ================\n");

        // Sample CSV content
        var csv = "id,name\n1,Ana\n2,Bruno";
        // Sample JSON content
        var json = "[ { \"id\": 1, \"name\": \"Ana\" }, { \"id\": 2, \"name\": \"Bruno\" } ]";

        DataImporter csvImporter = new CsvDataImporter();
        DataImporter jsonImporter = new JsonDataImporter();

        var csvData = csvImporter.Import(csv);
        var jsonData = jsonImporter.Import(json);

        WriteLine("CSV Parsed Rows:");
        Print(csvData);
        WriteLine("JSON Parsed Rows (with meta row):");
        Print(jsonData);

        WriteLine("\nExplanation: DataImporter.Import() is the template. It calls CreateParser() (Factory Method) so each subclass supplies its parser without changing the algorithm. Adding XML later only requires an XmlDataImporter + XmlParser.");
    }

    private static void Print(ParsedData data)
    {
        var i = 0;
        foreach (var row in data.Rows)
        {
            Write($"Row {i++}: ");
            WriteLine(string.Join(", ", row.Select(kv => kv.Key + '=' + kv.Value)));
        }
        WriteLine();
    }
}

