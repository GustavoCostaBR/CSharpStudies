# Topic 19: Pattern Matching in C#

## Introduction

Pattern Matching is a feature set in modern C# (enhanced from C# 7 onward) that lets you express conditional logic, decomposition, and type tests in a concise, readable, and *declarative* manner. Instead of writing verbose chains of `if` / `else` with casts, pattern matching allows you to match data *by shape, by type, by value, by relational condition,* or by *logical composition of patterns*. This improves clarity, reduces boilerplate, and encourages intent‑driven code—especially when working with heterogeneous object models, discriminated data (like records), or structured inputs (JSON, tuples, hierarchical data, etc.).

## Core Concepts

Key ideas and vocabulary:

- **Pattern**: A rule that an input value is tested against. If it *matches*, it may optionally *produce* variables (bindings) you can use.
- **Scrutinee**: The value being matched (the expression on the left of `is` or the value switched upon).
- **`is` Pattern Expression**: `expr is <pattern>` returns a boolean and may introduce a scoped variable.
- **`switch` Statement with Patterns**: Extends traditional `switch` with pattern arms and `when` guards.
- **`switch` Expression**: An expression form returning a value using pattern arms (concise, expression‑bodied, exhaustive or compile‑time error).
- **Common Pattern Categories**:
  - *Constant Pattern*: `x is 0`, `token is "OK"`.
  - *Type Pattern*: `obj is Customer c` binds `c` if `obj` is of type `Customer` (or derived / implements interface).
  - *Relational Pattern*: `n is > 0 and < 100`.
  - *Logical Patterns*: Combine with `and`, `or`, `not`.
  - *Property Pattern*: `cust is { Address.City: "Madrid", Age: > 18 }`.
  - *Positional Pattern*: Works with `Deconstruct` / records, e.g. `pt is (0, 0)` or `p is Point(> 0, > 0)`.
  - *List Pattern* (C# 11+): `items is [1, 2, ..]` (match sequence shape).
  - *Discard Pattern*: `_` matches anything (no binding).
  - *Var Pattern*: `var x` always matches and binds (often used for re‑naming or capturing expressions).
  - *Parenthesized Pattern*: `(pattern)` for grouping.
  - *Recursive Patterns*: Combining patterns inside patterns (property / positional nested usage).
- **Exhaustiveness**: For `switch` expressions the compiler requires all possible inputs are covered (via explicit patterns or a discard `_`).
- **Guards**: Additional boolean condition with `when` to refine a pattern match.

## Syntax & Usage

### Simple `is` Pattern Matching Example

```csharp
object value = GetSample(); // Could return different types

if (value is int i and >= 0 and < 10)
{
    Console.WriteLine($"Small int: {i}");
}
else if (value is string { Length: > 0 } s)
{
    Console.WriteLine($"Non-empty string of length {s.Length}");
}
else if (value is null)
{
    Console.WriteLine("Was null");
}
else
{
    Console.WriteLine("Other type or condition");
}

object GetSample() => 7; // Example provider
```

Step-by-step:
1. `value is int i and >= 0 and < 10` – type + relational + logical combination; binds `i` if all succeed.
2. `string { Length: > 0 } s` – property pattern ensures not null, of type `string`, length > 0 and binds as `s`.
3. `value is null` – explicit null pattern.
4. Fallback branch.

### Switch Pattern Matching (Statement) – Mini Example

```csharp
static string DescribeNumber(int n)
{
    switch (n)
    {
        case < 0:            return "Negative";
        case 0:              return "Zero";
        case > 0 and <= 10:  return "Small";
        case > 10 and < 100: return "Medium";
        default:             return "Large";
    }
}

Console.WriteLine(DescribeNumber(42)); // Medium
```

### Switch Expression – Mini Example

```csharp
static string Classify(object o) => o switch
{
    null                => "Null",
    int < 0             => "Negative int",
    int 0               => "Zero",
    int > 0 and < 10    => "Small int",
    string { Length: 0 } => "Empty string",
    string s            => $"String(len={s.Length})",
    _                   => "Something else"
};

Console.WriteLine(Classify("Hello"));
```

Explanation: The `switch` expression evaluates each pattern top‑to‑bottom; first match yields the result. The discard `_` covers remaining cases ensuring exhaustiveness.

## Practical Examples (Per Topic)

### 1. Pattern Matching Overview – Shape Hierarchy

We will use polymorphic shapes and pattern matching to compute areas and categorize shapes.

```csharp
abstract record Shape;
record Circle(double Radius) : Shape;
record Rectangle(double Width, double Height) : Shape;
record Triangle(double A, double B, double C) : Shape; // Sides
record Unknown(object Data) : Shape;

static double Area(Shape shape) => shape switch
{
    Circle { Radius: <= 0 }      => 0,
    Circle { Radius: var r }     => Math.PI * r * r,
    Rectangle { Width: > 0, Height: > 0 } r => r.Width * r.Height,
    Triangle { A: var a, B: var b, C: var c } t when IsValidTriangle(a,b,c)
        => Heron(a,b,c),
    _ => 0
};

static bool IsValidTriangle(double a, double b, double c) =>
    a > 0 && b > 0 && c > 0 && a + b > c && a + c > b && b + c > a;

static double Heron(double a, double b, double c)
{
    var s = (a + b + c) / 2.0;
    return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
}

var shapes = new Shape[]
{
    new Circle(2),
    new Rectangle(3,4),
    new Triangle(3,4,5),
    new Circle(-1),
    new Unknown("mystery")
};

foreach (var s in shapes)
    Console.WriteLine($"{s}: Area={Area(s):F2}");
```

Why: Demonstrates records (positional patterns), property patterns, guards (`when`), fallback discard, and domain logic in a declarative mapping.

### 2. Switch Pattern Matching – Command Router

Using a `switch` statement with patterns to process heterogeneous commands.

```csharp
interface ICommand { }
record CreateUser(string UserName) : ICommand;
record DeleteUser(int Id, bool Hard) : ICommand;
record GetUser(int Id) : ICommand;
record Ping(DateTime At) : ICommand;

static string Handle(ICommand cmd)
{
    switch (cmd)
    {
        case CreateUser { UserName: { Length: 0 } }: return "Invalid username";
        case CreateUser { UserName: var u }:        return $"Created user '{u}'";
        case DeleteUser ( <= 0, _ ):                return "Invalid id";
        case DeleteUser ( var id, true ):           return $"Hard-deleted user {id}";
        case DeleteUser ( var id, false ):          return $"Soft-deleted user {id}";
        case GetUser ( <= 0 ):                      return "Invalid id";
        case GetUser ( var id ):                    return $"Fetched user {id}";
        case Ping { At: var t } when t < DateTime.UtcNow.AddMinutes(-1):
            return "Ping expired";
        case Ping:                                  return "Pong";
        default:                                    return "Unknown command";
    }
}

var commands = new ICommand[]
{
    new CreateUser(""),
    new CreateUser("alice"),
    new DeleteUser(42, false),
    new DeleteUser(99, true),
    new GetUser(0),
    new GetUser(7),
    new Ping(DateTime.UtcNow.AddMinutes(-5)),
    new Ping(DateTime.UtcNow)
};

foreach (var c in commands)
    Console.WriteLine(Handle(c));
```

Why: Demonstrates `switch` statement pattern arms with property patterns, positional patterns (record positional decomposition), guards, and ordered specificity.

### 3. `is` Pattern Matching – Safe Downcasting & Validation

```csharp
object[] values = { 5, -3, 0, "hello", "", null, 9.5 }; // mixed data

foreach (var v in values)
{
    string classification = v switch
    {
        int > 0 and < 10 i        => $"Small int {i}",
        int 0                     => "Zero",
        int < 0                   => "Negative int",
        string { Length: > 0 } s  => $"Non-empty string '{s}'",
        string { Length: 0 }      => "Empty string",
        null                      => "Null literal",
        double d and >= 0         => $"Non-negative double {d:F1}",
        _                         => "Other"
    };
    Console.WriteLine(classification);
}
```

Why: Shows variety of patterns (`int > 0`, constant `0`, property pattern on string, null, relational + type) inside a switch expression used inline (readability, declarative classification).

### 4. Switch Expression & Expression-Bodied Members – HTTP Mapper

```csharp
static string ToStatusMessage(int code) => code switch
{
    >= 200 and < 300 => "Success",
    400               => "Bad Request",
    401               => "Unauthorized",
    404               => "Not Found",
    500 or 502 or 503 => "Server Error",
    _                 => "Other"
};

// Expression-bodied member returning a switch expression
static ConsoleColor ColorFor(int code) => code switch
{
    >= 200 and < 300 => ConsoleColor.Green,
    >= 400 and < 500 => ConsoleColor.Yellow,
    >= 500           => ConsoleColor.Red,
    _                => ConsoleColor.Gray
};

int[] testCodes = { 200, 201, 400, 404, 500, 418 };
foreach (var c in testCodes)
{
    var prev = Console.ForegroundColor;
    Console.ForegroundColor = ColorFor(c);
    Console.WriteLine($"{c}: {ToStatusMessage(c)}");
    Console.ForegroundColor = prev;
}
```

Why: Illustrates concise expression forms, relational & logical patterns, multi-value (`or`) patterns, and mapping integers to domain semantics and output formatting.

## Best Practices & Common Pitfalls

**Best Practices**:
- Prefer pattern matching over chains of `if (obj is Type)` followed by casts—improves readability and eliminates redundant casting.
- Order patterns from *most specific* to *most general* (especially in `switch` statements) to prevent unreachable code or unintended early matches.
- Use *property patterns* to keep validation logic declarative (e.g., `order is { Total: > 0, Status: OrderStatus.Open }`).
- Keep switch expressions *pure* (avoid side effects inside arms); compute and return values.
- For evolving domains (new derived types), consider using a discard `_` plus logging to detect unhandled future shapes.
- Use guards (`when`) sparingly; prefer embedding conditions as relational patterns when feasible (e.g., `case > 0 and < 100:` instead of `case int i when i > 0 && i < 100:`).

**Common Pitfalls**:
- Forgetting null cases when matching reference / nullable types (add explicit `null` when required or handle via fallback `_`).
- Overusing very complex single-line patterns hurting readability—extract sub-expressions or helper methods.
- Assuming order does not matter in `switch` statements—unlike switch expressions, statement arms are evaluated sequentially.
- Misunderstanding closure of pattern-scoped variables—bindings exist only within the matching arm or subsequent condition scope when using `is` inside an `if`.
- Using pattern matching where polymorphism (virtual dispatch) or dictionary lookup would be clearer—choose the tool that best matches the domain semantics.

**When to Use**:
- Classification of heterogeneous inputs.
- Data shape validation / destructuring (records, tuples, deconstruction-enabled types).
- Translating domain objects to DTOs / responses.
- Simplifying refactors from `if/else` ladders.

**When NOT to Use**:
- Performance‑critical inner loops with trivial logic (pattern matching can be equivalent, but unnecessary abstraction may obscure intent).
- Where future evolution strongly favors adding behavior to the types themselves (prefer polymorphism / visitor pattern).

## Advanced Topics

- **List Patterns**: Match sequence shapes: `nums is [1, _, 3, .. var rest]` capturing tail. Useful for parsing token streams.
- **Recursive Patterns**: Nesting property / positional patterns arbitrarily deep (e.g., `order is { Customer: { Address.Country: "US" }, Lines: [..] }`).
- **Span Pattern Matching (future improvements)**: Newer runtimes continue to improve matching performance over spans and slices.
- **Performance Considerations**: The compiler may lower switch expressions into jump tables or decision DAGs. Keep patterns distinct and minimal for optimal codegen.
- **Exhaustiveness in Discriminated Unions**: With `record` hierarchies and `sealed` base types, the compiler can warn on non-exhaustive switch expressions (C# 11+ improvements with `required` members also interact with property patterns).

## Summary

Pattern Matching enables declarative, type-safe, and concise handling of diverse data shapes in C#. You can classify, destructure, and validate using `is` expressions, enriched `switch` statements, and expressive `switch` expressions. Mastering pattern categories (constant, type, relational, property, positional, list, logical) and combining them effectively leads to cleaner code versus imperative branching. Use it to replace verbose casting, clarify intent, and centralize object shape logic—while staying mindful of readability, null handling, and appropriate alternative paradigms (polymorphism, lookup tables) when better suited.
