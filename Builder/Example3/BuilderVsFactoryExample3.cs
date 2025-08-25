using System;
using static System.Console;

namespace Builder.Example3;

/*
 Example3 Goal:
 Show a domain (House configuration) where:
  - Many optional features exist.
  - Some features depend on others (a pool requires a garden; solar panels require >= 2 floors).
  - We want readable, intention‑revealing construction + late validation.

 We implement:
 1. HouseBuilder (fluent, validates on Build). Demonstrates why Builder is clear.
 2. HouseFactory telescoping method with many optional parameters (harder to read / error prone).
 3. HouseFactory using an options record (improvement, but still lacks sequencing & invariant enforcement prior to build and can still be partially invalid until validation runs).

 Compare call sites in Example3Demo.Run().
*/

public class House
{
    public int Floors { get; }
    public int Bedrooms { get; }
    public bool HasGarage { get; }
    public bool HasGarden { get; }
    public bool HasPool { get; }
    public bool SolarPanels { get; }
    public string HeatingType { get; }

    internal House(int floors, int bedrooms, bool hasGarage, bool hasGarden, bool hasPool, bool solarPanels, string heatingType)
    {
        Floors = floors;
        Bedrooms = bedrooms;
        HasGarage = hasGarage;
        HasGarden = hasGarden;
        HasPool = hasPool;
        SolarPanels = solarPanels;
        HeatingType = heatingType;
    }

    public override string ToString() =>
        $"House: {Floors} floor(s), {Bedrooms} bedroom(s), Garage={HasGarage}, Garden={HasGarden}, Pool={HasPool}, SolarPanels={SolarPanels}, Heating={HeatingType}";
}

public class HouseBuilder
{
    private int? _floors;
    private int? _bedrooms;
    private bool _hasGarage;
    private bool _hasGarden;
    private bool _hasPool;
    private bool _solarPanels;
    private string? _heatingType;

    // Fluent API expresses intent clearly
    public HouseBuilder WithFloors(int floors) { _floors = floors; return this; }
    public HouseBuilder WithBedrooms(int bedrooms) { _bedrooms = bedrooms; return this; }
    public HouseBuilder AddGarage() { _hasGarage = true; return this; }
    public HouseBuilder AddGarden() { _hasGarden = true; return this; }
    public HouseBuilder AddPool() { _hasPool = true; return this; }
    public HouseBuilder AddSolarPanels() { _solarPanels = true; return this; }
    public HouseBuilder WithHeating(string heating) { _heatingType = heating; return this; }

    public House Build()
    {
        // Defer validation until all choices are made.
        if (_floors is null or <= 0)
            throw new InvalidOperationException("Floors must be > 0");
        if (_bedrooms is null or <= 0)
            throw new InvalidOperationException("Bedrooms must be > 0");

        // Domain invariants:
        if (_hasPool && !_hasGarden)
            throw new InvalidOperationException("Pool requires a garden");
        if (_solarPanels && _floors < 2)
            throw new InvalidOperationException("Solar panels require at least 2 floors (example rule)");

        var heating = _heatingType ?? "Gas"; // default
        return new House(_floors.Value, _bedrooms.Value, _hasGarage, _hasGarden, _hasPool, _solarPanels, heating);
    }
}

// ------------- Factory Alternatives -------------

public static class HouseFactory
{
    // 1. Telescoping / long parameter list variant.
    //    Call site must remember parameter order. Harder to read & maintain.
    public static House Create(
        int floors,
        int bedrooms,
        bool hasGarage = false,
        bool hasGarden = false,
        bool hasPool = false,
        bool solarPanels = false,
        string heatingType = "Gas")
    {
        Validate(floors, bedrooms, hasGarden, hasPool, solarPanels);
        return new House(floors, bedrooms, hasGarage, hasGarden, hasPool, solarPanels, heatingType);
    }

    // 2. Options record variant (improves readability somewhat, but still no staged guidance or partial validation until call time).
    public static House Create(HouseOptions options)
    {
        Validate(options.Floors, options.Bedrooms, options.HasGarden, options.HasPool, options.SolarPanels);
        var heating = string.IsNullOrWhiteSpace(options.HeatingType) ? "Gas" : options.HeatingType;
        return new House(options.Floors, options.Bedrooms, options.HasGarage, options.HasGarden, options.HasPool, options.SolarPanels, heating);
    }

    private static void Validate(int floors, int bedrooms, bool hasGarden, bool hasPool, bool solarPanels)
    {
        if (floors <= 0) throw new ArgumentException("Floors must be > 0", nameof(floors));
        if (bedrooms <= 0) throw new ArgumentException("Bedrooms must be > 0", nameof(bedrooms));
        if (hasPool && !hasGarden) throw new ArgumentException("Pool requires a garden", nameof(hasPool));
        if (solarPanels && floors < 2) throw new ArgumentException("Solar panels require >= 2 floors", nameof(solarPanels));
    }
}

public record HouseOptions(int Floors, int Bedrooms)
{
    public bool HasGarage { get; init; }
    public bool HasGarden { get; init; }
    public bool HasPool { get; init; }
    public bool SolarPanels { get; init; }
    public string? HeatingType { get; init; }
}

public static class Example3Demo
{
    public static void Run()
    {
        WriteLine("\n================ Example 3: Builder vs Factory ================\n");

        // --- Builder usage ---
        WriteLine("Builder: fluent, intention revealing, validates at the end:");
        var house = new HouseBuilder()
            .WithFloors(2)
            .WithBedrooms(3)
            .AddGarage()
            .AddGarden()
            .AddPool()
            .AddSolarPanels()
            .WithHeating("Electric")
            .Build();
        WriteLine(house);

        // Attempt invalid build to show guarded invariant
        WriteLine("\nBuilder: attempt invalid (pool without garden):");
        try
        {
            new HouseBuilder()
                .WithFloors(1)
                .WithBedrooms(2)
                .AddPool() // missing garden
                .Build();
        }
        catch (Exception ex)
        {
            WriteLine($"Caught expected builder validation error: {ex.Message}");
        }

        // --- Factory (telescoping) ---
        WriteLine("\nFactory (telescoping parameters):");
        var factoryHouse = HouseFactory.Create(2, 3, true, true, true, true, "Electric");
        WriteLine(factoryHouse + "   <-- harder to read arguments");

        // --- Factory with options object ---
        WriteLine("\nFactory (options object):");
        var optionsHouse = HouseFactory.Create(new HouseOptions(2, 3)
        {
            HasGarage = true,
            HasGarden = true,
            HasPool = true,
            SolarPanels = true,
            HeatingType = "Electric"
        });
        WriteLine(optionsHouse + "   <-- more readable but still no staged validation");

        WriteLine("\nKey Points: Builder offers (1) fluent readability, (2) deferred validation of cross-field rules, (3) discoverable steps via IntelliSense, (4) easy extension (just add a method). Factories excel when creation is a single atomic step without many optional combinations.");
    }
}

