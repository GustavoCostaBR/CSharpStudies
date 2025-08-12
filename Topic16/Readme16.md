# Topic 16: Operator Overloading

## Introduction

Operator overloading in C# allows you to redefine the way operators (like `+`, `-`, `*`, `/`, `==`, `!=`) work with user-defined types (classes or structs). By default, these operators work with built-in types like `int` and `string`. Operator overloading makes your custom types more intuitive and easier to work with, allowing them to be manipulated with familiar syntax, just like primitive types. This can lead to cleaner, more readable, and more expressive code.

## Core Concepts

-   **What can be overloaded?**: Most binary operators (`+`, `-`, `*`, `/`, `%`, `&`, `|`, `^`, `<<`, `>>`), unary operators (`+`, `-`, `!`, `~`, `++`, `--`), and comparison operators (`==`, `!=`, `<`, `>`, `<=`, `>=`) can be overloaded.
-   **What cannot be overloaded?**: Assignment operators (`=`), member access (`.`), method call (`()`), `new`, `typeof`, `sizeof`, `is`, `as`, and the conditional operator (`?:`) cannot be overloaded.
-   **Static Methods**: Overloaded operators must be declared as `public` and `static` methods within the type they operate on. They take one (for unary) or two (for binary) parameters of the type and return a result.
-   **Paired Operators**: Comparison operators must be overloaded in pairs. If you overload `==`, you must also overload `!=`. Similarly, `<` and `>`, and `<=` and `>=` must be paired.

## Syntax & Usage

The basic syntax for overloading an operator involves declaring a special method using the `operator` keyword.

### Syntax

```csharp
public static ReturnType operator <symbol>(ParameterType operand1, ParameterType operand2)
{
    // Logic for the operation
    return ...;
}
```

### Simple Example: A `Vector` class

Let's create a simple `Vector` class representing a 2D vector and overload the `+` operator to add two vectors together.

```csharp
public struct Vector
{
    public double X { get; }
    public double Y { get; }

    public Vector(double x, double y)
    {
        X = x;
        Y = y;
    }

    // Overloading the '+' operator
    public static Vector operator +(Vector v1, Vector v2)
    {
        return new Vector(v1.X + v2.X, v1.Y + v2.Y);
    }

    public override string ToString() => $"({X}, {Y})";
}
```

### Step-by-Step Explanation

1.  **`public struct Vector`**: We define a `Vector` struct to hold `X` and `Y` coordinates.
2.  **`public static Vector operator +(Vector v1, Vector v2)`**: This is the operator overload declaration.
    -   `public static`: It must be public and static.
    -   `Vector`: The method returns a new `Vector` instance, which is the result of the addition.
    -   `operator +`: This specifies that we are overloading the `+` operator.
    -   `(Vector v1, Vector v2)`: It takes two `Vector` objects as input operands.
3.  **`return new Vector(v1.X + v2.X, v1.Y + v2.Y);`**: The implementation adds the corresponding components of the two vectors and creates a new `Vector` with the result.

### Usage in Code

```csharp
var vector1 = new Vector(2, 3);
var vector2 = new Vector(4, 1);

// Use the overloaded '+' operator
Vector result = vector1 + vector2; 

Console.WriteLine(vector1);      // Output: (2, 3)
Console.WriteLine(vector2);      // Output: (4, 1)
Console.WriteLine(result);       // Output: (6, 4)
```

## Practical Example: `ComplexNumber` Class

A more practical scenario is creating a `ComplexNumber` class. Complex numbers have a real and an imaginary part, and their arithmetic rules are well-defined, making them a perfect candidate for operator overloading.

```csharp
public class ComplexNumber
{
    public double Real { get; }
    public double Imaginary { get; }

    public ComplexNumber(double real, double imaginary)
    {
        Real = real;
        Imaginary = imaginary;
    }

    // Overload the '+' operator for ComplexNumber + ComplexNumber
    public static ComplexNumber operator +(ComplexNumber c1, ComplexNumber c2)
    {
        return new ComplexNumber(c1.Real + c2.Real, c1.Imaginary + c2.Imaginary);
    }

    // Overload the '-' operator
    public static ComplexNumber operator -(ComplexNumber c1, ComplexNumber c2)
    {
        return new ComplexNumber(c1.Real - c2.Real, c1.Imaginary - c2.Imaginary);
    }

    // Overload the '==' operator
    public static bool operator ==(ComplexNumber c1, ComplexNumber c2)
    {
        // Handle nulls
        if (ReferenceEquals(c1, c2)) return true;
        if (c1 is null || c2 is null) return false;
        return c1.Real == c2.Real && c1.Imaginary == c2.Imaginary;
    }

    // Must overload '!=' if '==' is overloaded
    public static bool operator !=(ComplexNumber c1, ComplexNumber c2)
    {
        return !(c1 == c2);
    }

    // It's good practice to override Equals and GetHashCode when overloading '=='
    public override bool Equals(object obj)
    {
        return obj is ComplexNumber other && this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Real, Imaginary);
    }

    public override string ToString() => $"{Real} + {Imaginary}i";
}
```

### Why this code?

-   **Intuitive Arithmetic**: Overloading `+` and `-` allows developers to write `c1 + c2` instead of `c1.Add(c2)`, which is more natural and readable.
-   **Correct Equality**: Overloading `==` and `!=` provides a clear definition of equality for complex numbers (i.e., both real and imaginary parts must be equal). This is crucial for comparisons.
-   **Best Practices**: The example also shows the importance of overriding `Equals` and `GetHashCode` when you overload the equality operators to ensure consistent behavior across the .NET ecosystem (e.g., in collections like `Dictionary` or `HashSet`).

## Best Practices & Common Pitfalls

-   **Be Intuitive**: Only overload operators when the meaning is obvious and intuitive. Overloading `+` to perform subtraction would be a terrible idea.
-   **Overload in Pairs**: Always overload comparison operators in pairs (`==`/`!=`, `<`/`>`).
-   **Implement `IEquatable<T>`**: When overloading `==` and `!=`, it's a good practice to also implement the `IEquatable<T>` interface for type-safe equality checks.
-   **Return New Instances**: For operators that produce a new value (like `+` or `-`), your overload should return a new instance of the type rather than modifying one of the operands. This makes your types immutable and behavior predictable.
-   **Avoid Over-Engineering**: Don't overload operators just because you can. If a named method like `Multiply()` is clearer than `*`, use the method.

## Advanced Topics

-   **Implicit and Explicit Conversions**: Besides standard operators, you can define custom, user-defined type conversions using the `implicit` and `explicit` keywords. This allows you to, for example, convert a `ComplexNumber` to a `double` (explicitly, as it might lose data) or an `int` to a `ComplexNumber` (implicitly).
-   **True and False Operators**: You can overload the `true` and `false` operators, which is useful for creating types that can be used in boolean expressions (e.g., `if (myCustomObject)`). This is rare but powerful in specific scenarios.

## Summary

Operator overloading is a powerful C# feature that allows user-defined types to work with standard operators. When used correctly, it can significantly improve code readability and expressiveness by providing a more natural syntax. The key is to ensure that the overloaded operator's behavior is intuitive and consistent with its conventional meaning. Always follow best practices, such as overloading in pairs and ensuring your types behave predictably.
