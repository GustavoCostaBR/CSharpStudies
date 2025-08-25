using static System.Console;

namespace Builder.Example2;

/*
 Refactored fluent-style builder for vehicles (contrast with classic Director + abstract builder pattern).
 Goals:
  - Intention-revealing chained API.
  - Validation deferred until Build().
  - Removes explicit Director; caller sequences steps.
  - Provides convenience static starters per VehicleType.
  - Encapsulates cross-field rules (example rules for demo purposes).
*/

public sealed class VehicleFluentBuilder
{
    private readonly VehicleType _type;
    private string? _frame;
    private string? _engine;
    private int? _wheels;
    private int? _doors;

    private VehicleFluentBuilder(VehicleType type) => _type = type;

    // Entry points (discoverable via IntelliSense)
    public static VehicleFluentBuilder For(VehicleType type) => new(type);
    public static VehicleFluentBuilder Car() => new(VehicleType.Car);
    public static VehicleFluentBuilder MotorCycle() => new(VehicleType.MotorCycle);
    public static VehicleFluentBuilder Scooter() => new(VehicleType.Scooter);

    // Fluent configuration
    public VehicleFluentBuilder WithFrame(string frame)
    {
        _frame = frame;
        return this;
    }

    public VehicleFluentBuilder WithEngine(string engine)
    {
        _engine = engine;
        return this;
    }

    public VehicleFluentBuilder WithWheels(int wheels)
    {
        _wheels = wheels;
        return this;
    }

    public VehicleFluentBuilder WithDoors(int doors)
    {
        _doors = doors;
        return this;
    }

    // Convenience presets (optional)
    public VehicleFluentBuilder StandardDefaults()
    {
        return _type switch
        {
            VehicleType.Car => WithFrame(_frame ?? "Car Frame").WithEngine(_engine ?? "2500 cc")
                .WithWheels(_wheels ?? 4).WithDoors(_doors ?? 4),
            VehicleType.MotorCycle => WithFrame(_frame ?? "MotorCycle Frame").WithEngine(_engine ?? "500 cc")
                .WithWheels(_wheels ?? 2).WithDoors(_doors ?? 0),
            VehicleType.Scooter => WithFrame(_frame ?? "Scooter Frame").WithEngine(_engine ?? "50 cc")
                .WithWheels(_wheels ?? 2).WithDoors(_doors ?? 0),
            _ => this
        };
    }

    public Vehicle Build()
    {
        // Basic required fields
        if (string.IsNullOrWhiteSpace(_frame)) throw new InvalidOperationException("Frame not specified");
        if (string.IsNullOrWhiteSpace(_engine)) throw new InvalidOperationException("Engine not specified");
        if (_wheels is null) throw new InvalidOperationException("Wheels not specified");
        if (_doors is null) throw new InvalidOperationException("Doors not specified");

        // Example domain constraints (illustrative only):
        if (_type == VehicleType.MotorCycle && _wheels != 2)
            throw new InvalidOperationException("MotorCycle must have exactly 2 wheels");
        if (_type == VehicleType.MotorCycle && _doors != 0)
            throw new InvalidOperationException("MotorCycle must have 0 doors");
        if (_type == VehicleType.Scooter && _wheels != 2)
            throw new InvalidOperationException("Scooter must have exactly 2 wheels");
        if (_type == VehicleType.Car && _wheels is < 3 or > 6)
            throw new InvalidOperationException("Car wheels must be between 3 and 6 for this demo");
        if (_doors < 0 || _doors > 6)
            throw new InvalidOperationException("Door count out of supported range");

        var vehicle = new Vehicle(_type);
        vehicle[PartType.Frame] = _frame!;
        vehicle[PartType.Engine] = _engine!;
        vehicle[PartType.Wheel] = _wheels!.ToString();
        vehicle[PartType.Door] = _doors!.ToString();
        return vehicle;
    }
}

public static class FluentVehicleExample2Demo
{
    public static void Run()
    {
        WriteLine("\n===== Example2 Fluent Refactor =====");

        var car = VehicleFluentBuilder.Car()
            .StandardDefaults() // fills defaults
            .WithEngine("3000 cc") // override just one aspect
            .Build();
        car.Show();

        var bike = VehicleFluentBuilder.MotorCycle()
            .WithFrame("Custom Steel Frame")
            .WithEngine("650 cc")
            .WithWheels(2)
            .WithDoors(0)
            .Build();
        bike.Show();

        WriteLine("\n(Attempt invalid scooter with 3 wheels -> expect validation error)");
        try
        {
            VehicleFluentBuilder.Scooter()
                .StandardDefaults()
                .WithWheels(3) // invalid
                .Build();
        }
        catch (Exception ex)
        {
            WriteLine($"Validation caught: {ex.Message}");
        }

        WriteLine(
            "\nComparison Notes: Classic pattern separated construction steps via Director; fluent version inlines sequencing, improves readability, centralizes validation, and supports partial overrides of sensible defaults.");
    }
}