# Topic 3: Variables and data types

In C#, variables are used to store data that your program can manipulate. Every variable must have a specific **type**, which determines the size and layout of the variable's memory, the range of values that can be stored within that memory, and the set of operations that can be applied to the variable.

## General type system

C# has a **Unified Type System**, which means that all types—from simple numbers to complex user-defined classes—inherit from a single base type: `System.Object` (or its alias, `object`). This allows for consistent type handling and polymorphism across the entire system.

The types in C# are divided into two main categories:

### 1. Value Types
Value types directly contain their data. When you assign a value type variable to another, the value is copied. They are typically stored on the **stack**, which is a fast, last-in-first-out memory structure for storing local variables and managing method calls.

Key characteristics:
-   Cannot be `null` (unless they are made "nullable", e.g., `int?`).
-   Stored on the stack (generally).
-   Examples include:
    -   **Simple types**: `int`, `double`, `float`, `bool`, `char`.
    -   **`struct`**: User-defined value types.
    -   **`enum`**: A set of named constants.

```csharp
int a = 10;
int b = a; // The value 10 is copied from 'a' to 'b'.
b = 20;    // Changing 'b' does not affect 'a'.

Console.WriteLine($"a is {a}"); // Output: a is 10
Console.WriteLine($"b is {b}"); // Output: b is 20
```

### 2. Reference Types
Reference types do not store the actual data directly. Instead, they store a **reference** (or a memory address) to where the data is located. The actual data is stored on the **heap**, a larger memory area used for dynamic memory allocation.

Key characteristics:
-   Can be `null`, meaning they don't refer to any object.
-   Stored on the heap.
-   When you assign a reference type variable to another, only the reference is copied, not the actual object. Both variables will then point to the same object in memory.
-   Examples include:
    -   **`class`**: The primary way to define reference types.
    -   **`interface`**: Defines a contract.
    -   **`delegate`**: A type that represents references to methods.
    -   **`string`**: A special case. It's a reference type but behaves like a value type in many ways (it's immutable).
    -   **`record`**: A specialized class for immutable data models.

```csharp
// Assume we have a Person class with a Name property
var person1 = new Person { Name = "Alice" };
var person2 = person1; // The reference is copied. Both variables point to the same object.

person2.Name = "Bob"; // Changing the object through 'person2'...

Console.WriteLine(person1.Name); // Output: Bob ...affects what 'person1' sees.
```
I have created a [`Person.cs`](./Examples/Person.cs) class in the `Topic3/Examples` folder to illustrate this.

## Naming variables

Properly naming variables is crucial for writing readable and maintainable code. The naming conventions we discussed in Topic 2 apply here directly.

### Rules for Naming Variables:
-   A variable name can only contain letters (uppercase and lowercase), numbers, and the underscore character (`_`).
-   The first character of a variable name must be a letter or an underscore. It cannot be a number.
-   Variable names are case-sensitive (`myVariable` and `MyVariable` are two different variables).
-   You cannot use C# keywords (like `int`, `class`, `if`) as variable names. If you must use a keyword, you can prefix it with `@` (e.g., `@class`), but this is rare and should be avoided.

### Best Practices (Recap):
-   Use **camelCase** for local variables and method parameters (e.g., `string firstName`, `int totalScore`).
-   Choose descriptive names that indicate the variable's purpose. `remainingAttempts` is much better than `ra`.
-   Avoid single-letter variable names, except for loop counters (`i`, `j`, `k`) or in very short, simple methods.
-   For private class-level fields, use camelCase prefixed with an underscore (e.g., `_connectionString`).

```csharp
// Good variable names
string customerName = "John Smith";
int numberOfItemsInCart = 5;
bool isOrderShipped = false;

// Avoid these
// string n = "John Smith"; // Not descriptive
// int i = 5;              // Ambiguous outside a loop
// bool shipped = false;    // "isShipped" or "hasShipped" is clearer
```

## Use of basic data types

C# comes with a set of built-in data types that represent numbers, text, and boolean values. These are the fundamental building blocks for your data.

### Declaring and Initializing Variables
A variable is declared by specifying its type and name. You can initialize it at the same time.

```csharp
// Declaration
int myNumber;

// Initialization
myNumber = 100;

// Declaration and Initialization
string message = "Hello, C#!";
```

It's also very common practice in modern C# to use the `var` keyword to declare and initialize a variable in one step, letting the compiler infer the type. We will cover this in more detail shortly.

### Integer Types
These are used for whole numbers. The most common is `int`.

| Type    | Size (bits) | Range                               |
|---------|-------------|-------------------------------------|
| `sbyte` | 8           | -128 to 127                         |
| `byte`  | 8           | 0 to 255                            |
| `short` | 16          | -32,768 to 32,767                   |
| `ushort`| 16          | 0 to 65,535                         |
| `int`   | 32          | -2.1 billion to 2.1 billion         |
| `uint`  | 32          | 0 to 4.2 billion                    |
| `long`  | 64          | -9 quintillion to 9 quintillion     |
| `ulong` | 64          | 0 to 18 quintillion                 |

```csharp
int myAge = 30;
long worldPopulation = 8_000_000_000L; // Use 'L' suffix for long literals. Underscores can be used as digit separators.
```

### Floating-Point Types
These are for numbers with a fractional component. `double` is the most common choice for general-purpose calculations.

| Type      | Precision | Suffix |
|-----------|-----------|--------|
| `float`   | ~6-9 digits  | `F`    |
| `double`  | ~15-17 digits| `d` or none |
| `decimal` | 28-29 digits | `M`    |

Use `decimal` for financial or monetary calculations where high precision is critical and rounding errors must be avoided.

```csharp
double pi = 3.14159;
float gravity = 9.8F;
decimal accountBalance = 1500.99M;
```

### Boolean Type
The `bool` type represents a logical value: `true` or `false`. It's essential for controlling program flow.

```csharp
bool isUserLoggedIn = true;
if (isUserLoggedIn)
{
    Console.WriteLine("Welcome back!");
}
```

### Character and String Types
-   `char`: Represents a single 16-bit Unicode character. Literals are enclosed in single quotes (`'`).
-   `string`: Represents a sequence of characters. It's a reference type, but it's immutable. Literals are in double quotes (`"`).

```csharp
char firstInitial = 'G';
string fullName = "Gustavo";
```

### The `var` Keyword
You can use the `var` keyword to let the compiler infer the type of a local variable from the value it's initialized with. This can make code more concise.

```csharp
var age = 30; // Compiler infers 'int'
var name = "Jane Doe"; // Compiler infers 'string'
var price = 19.99; // Compiler infers 'double'
```

**Note:** The variable is still strongly typed. Once the compiler infers the type, it cannot be changed. `var` can only be used for local variables where the type is clear from the right side of the assignment.

## User data types

Beyond the built-in types, C# allows you to define your own custom types to model the concepts in your application's domain. The primary ways to do this are with `class`, `struct`, `enum`, and `record`.

### `class`
As we've seen, a `class` is a reference type. It's the most common way to define a complex object, especially one that has a distinct identity and behavior. Our `Person` class is a perfect example.

See [`Person.cs`](./Examples/Person.cs).

### `struct`
A `struct` (structure) is a value type. It's ideal for small, data-centric types that don't have a strong sense of identity and are conceptually similar to the built-in simple types.

**When to use a `struct`:**
- **For small, simple data aggregates:** Think of types that group a few related values, like `Point` (X, Y), `Color` (R, G, B), or `KeyValuePair`.
- **When you need value-type semantics:** You want copies of the object to be independent. Changing one copy should not affect the other.
- **For performance-critical code:** Because structs are typically allocated on the stack, they can be created and cleaned up much faster than class instances (which are allocated on the heap and managed by the Garbage Collector). This can reduce memory pressure in tight loops or large collections.

**When to prefer a `class`:**
- When the object has a distinct identity that needs to be preserved across different parts of your application (e.g., a `User` or `Order` object).
- When you need inheritance.
- When the object is large. Copying large structs can be less efficient than copying a small reference.

I have created a [`Point.cs`](./Examples/Point.cs) struct in the `Examples` folder.
```csharp
// In Point.cs
public struct Point
{
    public double X { get; }
    public double Y { get; }

    public Point(double x, double y)
    {
        X = x;
        Y = y;
    }
}

// Usage
var p1 = new Point(10, 20);
var p2 = p1; // p1 is copied to p2.
// Changing p2 would not affect p1.
```

### `enum`
An `enum` (enumeration) is a special value type that lets you define a set of named integral constants. It makes your code more readable and less error-prone than using "magic numbers".

```csharp
public enum ShippingMethod
{
    Standard, // 0
    Express,  // 1
    Overnight // 2
}

var method = ShippingMethod.Express;
Console.WriteLine($"Selected method: {method}"); // Output: Selected method: Express
```

**Enums as Flags:**
A powerful feature of enums is the ability to use them as bit flags, allowing an enum instance to store any combination of values. This is done by assigning values that are powers of two and decorating the enum with the `[Flags]` attribute.

I have created a [`Permissions.cs`](./Examples/Permissions.cs) enum in the `Examples` folder to demonstrate this.
```csharp
// In Permissions.cs
[Flags]
public enum Permissions
{
    None = 0,      // 0000
    Read = 1,      // 0001
    Write = 2,     // 0010
    Execute = 4,   // 0100
    All = Read | Write | Execute
}

// Usage
var userPermissions = Permissions.Read | Permissions.Write; // Combine flags

// Check for a specific permission
if (userPermissions.HasFlag(Permissions.Read))
{
    Console.WriteLine("User has Read permission.");
}

// Check for multiple permissions
if ((userPermissions & Permissions.All) == userPermissions)
{
    Console.WriteLine("User has all permissions.");
}
```

### Records: `record class` and `record struct`
Introduced in C# 9 and enhanced in C# 10, records are specialized types designed primarily for encapsulating **immutable data**. They provide concise syntax and built-in behaviors for value-based equality.

When you write `record`, it's a shortcut for `record class`, which is a reference type. You can also define a `record struct`, which is a value type.

**Key Features & Benefits:**
- **Immutability by Default:** Records use `init`-only properties, meaning they can only be set during object initialization. This prevents the state of an object from changing after it's created, which helps to reduce bugs.
- **Non-Destructive Mutation:** While records are immutable, you can create a modified copy using a `with` expression. This is a clean and efficient way to create a new instance based on an existing one.
- **Value-Based Equality:** Two record instances are considered equal if they have the same type and store the same values. For classes, two instances are only equal if they refer to the exact same object in memory.
- **Concise Syntax:** Positional records allow you to declare the type and its properties in a single line.
- **Built-in Formatting:** The compiler generates a clean `ToString()` override for you.

**Example: `record class`**
Use a `record class` when you want reference-type semantics (e.g., you can have a `null` reference) but with the benefits of immutability and value-based equality. This is perfect for Data Transfer Objects (DTOs).

I have updated [`Product.cs`](./Examples/Product.cs) in the `Examples` folder to be a positional record.
```csharp
// In Product.cs
public record Product(int Id, string Name, decimal Price);

// Usage
var product1 = new Product(1, "Laptop", 1200.00M);
var product2 = new Product(1, "Laptop", 1200.00M);

Console.WriteLine(product1); // Output: Product { Id = 1, Name = Laptop, Price = 1200.00 }
Console.WriteLine(product1 == product2); // Output: True (value-based equality)

// Non-destructive mutation
var discountedProduct = product1 with { Price = 1000.00M };
Console.WriteLine(discountedProduct); // Output: Product { Id = 1, Name = Laptop, Price = 1000.00 }
```

**Example: `record struct`**
Use a `record struct` when you want the benefits of a record (immutability, value equality) but also the performance characteristics of a value type (stack allocation). This is a great choice for small, immutable data structures that you might put in a large collection.

I have created a [`Color.cs`](./Examples/Color.cs) record struct in the `Examples` folder.
```csharp
// In Color.cs
public readonly record struct Color(byte R, byte G, byte B);

// Usage
var red = new Color(255, 0, 0);
var alsoRed = new Color(255, 0, 0);

Console.WriteLine(red == alsoRed); // Output: True
```

**Which to Use for DTOs: `record class` or `record struct`?**

For Data Transfer Objects (DTOs), especially those used in APIs (like ASP.NET Core Web API), the general recommendation is to use a **`record class`**.

Here’s why:
1.  **Nullability:** Web APIs and databases often work with data that can be `null`. A `record class` is a reference type, so it can be `null`, which maps naturally to concepts like a missing JSON object. A `record struct` is a value type and cannot be `null`, which can complicate serialization and model binding in web frameworks.
2.  **Compatibility with Frameworks:** Most .NET frameworks are designed and optimized to work with classes. While they can handle structs, the ecosystem is more mature around reference types for things like dependency injection, ORMs (like Entity Framework Core), and model binding.
3.  **Cost of Copying:** When you pass a DTO through different layers of your application, a `record class` only copies a small reference. A `record struct` copies the entire object. If your DTO is large, this copying can become less efficient than the cost of garbage collecting a class instance.

A **`record struct`** is the better choice for DTOs only when they are very small and you are operating in a performance-critical context where you need to minimize heap allocations and GC pressure (e.g., high-frequency trading, game development, or image processing).

### Other Modern C# Types to Know: Inline Arrays

Introduced in C# 12, **inline arrays** are a new `struct` type that allows you to create a fixed-size array directly within another `struct`. This is an advanced feature primarily used for high-performance scenarios and interoperability with native code, where you need a contiguous block of memory of a specific size without the overhead of heap allocation.

It helps to improve performance by avoiding "unsafe" fixed-size buffers while providing similar benefits of direct memory access.

```csharp
// Declaration in a struct
[System.Runtime.CompilerServices.InlineArray(10)]
public struct Buffer
{
    private int _element0;
}

// Usage
var buffer = new Buffer();
for (int i = 0; i < 10; i++)
{
    buffer[i] = i * i; // Can be indexed like an array
}
```
You likely won't use this feature in everyday business application development, but it's a powerful tool to be aware of for specialized, performance-sensitive work.

## Data type conversion

Often, you'll need to convert a variable from one type to another. C# provides several ways to do this, which can be categorized as either implicit or explicit conversions.

### Implicit Conversion
An implicit conversion is performed automatically by the compiler when it's safe to do so, meaning no data will be lost. This typically happens when you convert from a smaller integral type to a larger one, or from a derived class to a base class.

```csharp
int myInt = 123;
long myLong = myInt; // Implicit conversion from int to long, no data loss.

double myDouble = myInt; // Implicit conversion from int to double.
Console.WriteLine(myDouble); // Output: 123
```

### Explicit Conversion (Casting)
An explicit conversion, or a **cast**, is required when information might be lost in the conversion, or when the conversion might not succeed for other reasons. You must explicitly tell the compiler you want to perform the conversion using the cast operator `(typeName)`.

If you cast a larger type to a smaller one, you risk losing data (a "narrowing" conversion).

```csharp
double pi = 3.14159;
// int integerPi = pi; // This would cause a compile error.

// You must use an explicit cast.
int integerPi = (int)pi;
Console.WriteLine(integerPi); // Output: 3 (The fractional part is truncated, data is lost)

long bigNumber = 3000000000L;
// int smallNumber = (int)bigNumber; // This will overflow and result in an incorrect value without throwing an exception.
```

### Using Helper Methods
For converting between non-compatible types (like a `string` to an `int`), you must use helper methods.

#### The `Convert` Class
The `System.Convert` class provides a variety of methods to convert between the base types. It's useful because it can handle `null` values (returning 0) and performs rounding when converting floating-point numbers to integers, rather than just truncating.

```csharp
string numberString = "123";
int convertedInt = Convert.ToInt32(numberString);
Console.WriteLine(convertedInt); // Output: 123

double d = 3.7;
int i = Convert.ToInt32(d); // Rounds to the nearest even number (in this case, 4)
Console.WriteLine(i); // Output: 4
```

#### `Parse()` and `TryParse()`
These methods are available on most numeric and date/time types to parse a string into that type.
-   `Parse()`: Converts a string to its corresponding type. It will throw a `FormatException` if the string is not in a valid format, or an `OverflowException` if the number is too large for the target type.
-   `TryParse()`: This is a safer way to parse. It tries to convert the string, but instead of throwing an exception on failure, it returns a `bool` indicating whether the conversion succeeded. The converted value is returned via an `out` parameter. This is often the preferred method for handling user input.

```csharp
string userInput = "25";
string invalidInput = "hello";

// Using Parse() - can throw an exception
try
{
    int age = int.Parse(userInput);
    Console.WriteLine($"Age is: {age}");
}
catch (FormatException)
{
    Console.WriteLine("Invalid input format.");
}

// Using TryParse() - safer
if (int.TryParse(userInput, out int parsedAge))
{
    Console.WriteLine($"Successfully parsed age: {parsedAge}");
}
else
{
    Console.WriteLine("Could not parse age.");
}

if (!int.TryParse(invalidInput, out int failedParse))
{
    Console.WriteLine("Failed to parse 'hello' as expected.");
}
```