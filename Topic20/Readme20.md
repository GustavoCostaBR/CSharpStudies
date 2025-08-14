# Topic 20: Records in C#

## Introduction

Records are reference (or value, in the case of `record struct`) types introduced in C# 9 to model *immutable data with value-based equality* succinctly. They dramatically reduce boilerplate when defining data-centric objects (DTOs, messages, events, results, queries) by auto‑generating `Equals`, `GetHashCode`, `ToString`, *with*-style cloning, and deconstruction members. Records encourage functional, declarative styles and safer code by favoring immutability and explicit non-destructive mutation.

## Core Concepts

- **Record**: A type declared with the `record` keyword (`record class` by default) that gets built‑in value equality and boilerplate generation.
- **Primary Constructor / Positional Parameters**: Parameters in the record declaration header define public init-only properties automatically (`record Person(string FirstName, string LastName);`).
- **Value Equality**: Two record instances are considered equal (`==` / `Equals`) when *all nested component values* are equal (structural comparison), not when references are identical (unless using `record struct`).
- **Immutability by Convention**: Records favor `init` accessors and absence of setters to promote immutable state. (They are *not automatically immutable*—you can still introduce mutable fields/properties.)
- **With-Expressions**: Non-destructive copying: `var p2 = p1 with { LastName = "Smith" };` creates a shallow copy with modifications.
- **Deconstruction**: Records generate `Deconstruct` for primary constructor parameters enabling tuple-like unpacking: `var (first, last) = person;`.
- **Inheritance**: Records support inheritance, but *record inheritance should mirror data hierarchies carefully* because equality semantics incorporate the runtime type.
- **Record Structs / Record Classes**: `record struct` adds value-semantics container with similar niceties. 
- **Printing**: Auto-generated `ToString()` prints property names and values in a concise form.

## Syntax & Usage

### Basic Record Declaration (Introduction to Records)
```csharp
// Positional record (primary constructor defines properties)
public record Person(string FirstName, string LastName);

// Non-positional (explicit properties)
public record Address
{
    public string Street { get; init; } = string.Empty;
    public string City   { get; init; } = string.Empty;
};

var p1 = new Person("Ada", "Lovelace");
Console.WriteLine(p1); // Person { FirstName = Ada, LastName = Lovelace }

var a1 = new Address { Street = "42 Code Way", City = "London" };
Console.WriteLine(a1); // Address { Street = 42 Code Way, City = London }
```
Step-by-step:
1. `record Person(string FirstName, string LastName)` creates two `init` properties and value-based equality members.
2. `record Address` uses manual property declarations—still gains value equality & printing.
3. Instantiation looks like normal types; records are reference types (unless `record struct`).

### Mutability Example
```csharp
public record MutableUser
{
    public int Id { get; set; }          // Mutable (discouraged for core identity data)
    public string Name { get; init; } = string.Empty; // Init-only (settable only during construction)
}

var u = new MutableUser { Id = 1, Name = "Eve" };
// u.Name = "Changed"; // ERROR: init-only outside object creation
u.Id = 2;                // Allowed (mutable property)
Console.WriteLine(u);    // MutableUser { Id = 2, Name = Eve }
```
Key points:
- `init` preserves immutability post-construction.
- Introducing setters makes the record partially mutable, which still keeps value equality but can create logical pitfalls (object used as dictionary key mutated afterwards).

### Value Equality Example
```csharp
var pA = new Person("Ada", "Lovelace");
var pB = new Person("Ada", "Lovelace");
Console.WriteLine(ReferenceEquals(pA, pB)); // False (different references)
Console.WriteLine(pA == pB);                // True  (value equality)
Console.WriteLine(pA.Equals(pB));           // True

// With-expression produces a clone with one change
var pC = pA with { LastName = "Byron" };
Console.WriteLine(pC);                      // Person { FirstName = Ada, LastName = Byron }
Console.WriteLine(pA == pC);                // False (LastName differs)

// Deconstruction
var (first, last) = pA;                     // first = Ada, last = Lovelace
```
Explanation:
1. Two separate instances with identical component values are equal under `==` and `Equals`.
2. `ReferenceEquals` reveals they are distinct references.
3. `with` enables non-destructive updates.
4. `Deconstruct` supports tuple-like usage.

## Practical Example (Combining All)

Scenario: Modeling an immutable order system with discounts and computing derived projections while preserving original objects.
```csharp
public enum OrderStatus { Draft, Submitted, Shipped, Cancelled }

public record Customer(string Id, string Name, string Email);

public record OrderLine(string Sku, int Quantity, decimal UnitPrice)
{
    public decimal LineTotal => Quantity * UnitPrice;
}

public record Order(
    string Id,
    Customer Customer,
    IReadOnlyList<OrderLine> Lines,
    OrderStatus Status,
    DateTime CreatedUtc,
    decimal DiscountPercent = 0m)
{
    public decimal Subtotal => Lines.Sum(l => l.LineTotal);
    public decimal Discount => Math.Round(Subtotal * DiscountPercent / 100m, 2);
    public decimal Total    => Subtotal - Discount;

    public Order ApplyDiscount(decimal percent) => this with { DiscountPercent = percent };

    public Order Submit() => Status switch
    {
        OrderStatus.Draft => this with { Status = OrderStatus.Submitted },
        _ => this // Idempotent for non-draft states
    };
}

// Usage
var customer = new Customer("C001", "Ada Lovelace", "ada@example.com");
var order = new Order(
    Id: "O1001",
    Customer: customer,
    Lines: new List<OrderLine>
    {
        new("BK-ALG", 1, 59.90m),
        new("BK-MATH", 2, 39.50m)
    },
    Status: OrderStatus.Draft,
    CreatedUtc: DateTime.UtcNow);

Console.WriteLine(order); // Auto ToString prints structure
Console.WriteLine($"Subtotal: {order.Subtotal} Total: {order.Total}");

var discounted = order.ApplyDiscount(10m); // Non-destructive clone
var submitted  = discounted.Submit();

Console.WriteLine($"Original Status: {order.Status}, After Submit: {submitted.Status}");
Console.WriteLine($"Original Total: {order.Total}, Discounted Total: {discounted.Total}");

// Value equality demonstration
var duplicate = order with { }; // clone without changes
Console.WriteLine(order == duplicate); // True
```
Why these choices:
- Records encapsulate *data + computed members* succinctly.
- Immutability avoids unintended side-effects (each transformation returns new instance).
- Value equality allows easy detection of meaningful changes.
- `with` usage clarifies non-destructive transformations (discount, submit).

## Best Practices & Common Pitfalls

### Best Practices
- Favor **init-only** properties (or `readonly` collections) to maintain logical immutability.
- Use **positional records** for simple DTO-like types; switch to *nominal* form when adding validation, extra logic, or non-constructor properties.
- Treat records as **data carriers**; keep heavy behavior in services/domain logic where appropriate.
- Leverage `with` for **non-destructive mutations** instead of manual copying.
- Use records for **value-centric identity** (messages, events, queries) rather than entity identity reliant on database-generated IDs that may change mid-lifecycle.
- When using in collections keyed by equality (e.g., `HashSet`, dictionary keys), keep them immutable to avoid hash/equality inconsistency.
- Prefer `record struct` only when *value-type semantics* (stack allocation, avoidance of GC) and small size make sense.

### Common Pitfalls
- Adding mutable setters then mutating after insertion into a hash-based set causing **stale hash buckets** and lookup anomalies.
- Overusing inheritance among records—value equality becomes complex and can surprise when mixing base/derived comparisons.
- Assuming deep copy: `with` produces a **shallow copy** (reference-typed properties are shared).
- Large record graphs can make `Equals` / `GetHashCode` **expensive**; consider custom implementations for hotspots.
- Using records where **object identity** (reference uniqueness) matters more than structural equality (e.g., UI components, long-lived stateful services).
- Forgetting that `record class` is a **reference type**—aliasing still occurs unless you copy.

### When to Use vs Alternatives
- Use **records** over classes for immutable data aggregates, event sourcing messages, configuration snapshots, discriminated unions (combined with inheritance or pattern matching).
- Use **class** when representing entities with evolving lifecycle & identity or when you need fine-grained mutation.
- Use **struct / record struct** for small, frequently instantiated value types (beware of copying costs when large).

## Advanced Topics

| Feature | Description | Example |
|---------|-------------|---------|
| Non-destructive mutation | `with` expression clones & overrides | `var p2 = p1 with { FirstName = "Grace" };` |
| Inheritance | Records can derive: `record SpecialPerson(...) : Person(...)` | Pattern matching benefits |
| Sealed ToString | Override or `sealed override` to control printing | `public sealed override string ToString() => ...;` |
| Custom Equality | Override `Equals`/`GetHashCode` if performance or semantics differ | Provide manual hashing |
| Record Struct | Value type variant | `public readonly record struct Point(int X, int Y);` |
| `with` on record struct | Produces value copy with modifications | `var p2 = p with { X = 5 };` |
| Deconstruction Control | Manually add extra `Deconstruct` overloads | `public void Deconstruct(out string f, out string l, out int len)` |
| Primary Constructors (C# 12) | Use in regular classes too | `public class Person(string name){ public string Name => name; }` |
| Required Members | Combine with records for invariants | `public required string Code { get; init; }` |
| Pattern Matching | Records pair naturally with positional/property patterns | `p is Person("Ada", _)` |
| Serialization | Slim DTO definitions | Works well with `System.Text.Json` |

### Example: Record Struct vs Record Class
```csharp
public record struct Pixel(int R, int G, int B)
{
    public override string ToString() => $"#{R:X2}{G:X2}{B:X2}";
}

var px1 = new Pixel(255, 128, 0);
var px2 = px1 with { G = 129 };
Console.WriteLine(px1); // #FF8000
Console.WriteLine(px2); // #FF8100
Console.WriteLine(px1 == px2); // False (value equality by fields)
```

### Example: Custom Equality Optimization
```csharp
public record LargePayload(string Id, string Json)
{
    // Cache hash for performance (immutability assumption)
    private int? _hash;
    public override int GetHashCode()
        => _hash ??= HashCode.Combine(Id, Json?.Length); // Avoid hashing entire string contents repeatedly
}
```

## Summary

Records provide concise, expressive definitions for data-centric types with built-in value equality, cloning, deconstruction, and printable representations. They encourage immutability, simplify transformation workflows via `with`, and integrate seamlessly with pattern matching and modern serialization. Use them when *what the data is* matters more than *which specific object instance* it is. Remain mindful of mutability pitfalls, shallow copying, and performance considerations for large graphs. Mastering records elevates code clarity, reduces boilerplate, and strengthens correctness in domain modeling.
