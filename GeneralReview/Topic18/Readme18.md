# Topic 18: Delegates, Lambda Expressions, and LINQ

## Introduction

Delegates, Lambda Expressions, and LINQ are three of the most powerful and interconnected features in modern C#. They form the foundation of functional-style programming in the language, enabling developers to write highly expressive, declarative, and efficient code for querying and manipulating data. Delegates act as type-safe function pointers, lambda expressions provide a concise syntax for creating anonymous functions, and LINQ (Language-Integrated Query) leverages both to offer a unified, readable syntax for working with any data source, from in-memory collections to databases.

## Core Concepts

-   **Delegates**: A delegate is a reference type that can hold a reference to a method. It's like a type-safe function pointer. Before a method can be assigned to a delegate, its signature must match the signature defined by the delegate.
-   **Generic Delegates**: To avoid creating custom delegates for every method signature, .NET provides a set of built-in generic delegates. The most common are:
    -   `Func<T, TResult>`: Represents a method that takes one or more parameters (`T`) and returns a value (`TResult`).
    -   `Action<T>`: Represents a method that takes one or more parameters (`T`) but does not return a value.
    -   `Predicate<T>`: Represents a method that takes a parameter (`T`) and returns a boolean value. It's often used for testing a condition.
-   **Lambda Expressions**: A lambda expression is an anonymous function—a block of code that can be treated like a value. It's a shorthand syntax for creating an instance of a delegate. They are the cornerstone of LINQ.
-   **Anonymous Types**: These are simple, read-only types that you can create on the fly without explicitly defining a class. They are typically used in LINQ `select` clauses to create a new object with a specific shape from a larger data source.
-   **LINQ (Language-Integrated Query)**: A set of technologies that integrates query capabilities directly into the C# language. LINQ provides a consistent model for working with data across various kinds of data sources and formats.
-   **`IEnumerable<T>` vs. `IQueryable<T>`**: These are the two core interfaces of LINQ.
    -   `IEnumerable<T>`: Represents a forward-only cursor of a collection that can be enumerated. LINQ queries on `IEnumerable<T>` are executed in-memory. It's best for local collections (lists, arrays).
    -   `IQueryable<T>`: Represents a query that can be executed against a specific data source (like a SQL database). The query is translated into the native query language of the provider (e.g., SQL). It's best for remote data sources.
-   **Closure**: A closure is the combination of a function and the lexical environment within which that function was declared. In C#, when a lambda expression accesses a variable from its containing method (an "outer" variable), it "closes over" that variable. This means it maintains a reference to the variable, even after the containing method has finished executing.

## Syntax & Usage

### Lambda Expressions

-   **Syntax**: `(input-parameters) => expression` or `(input-parameters) => { statement-block }`
-   **Usage**:
    ```csharp
    // A lambda that takes two integers and returns their sum.
    // The compiler infers the delegate type as Func<int, int, int>.
    Func<int, int, int> add = (x, y) => x + y;
    int result = add(5, 3); // result is 8

    // A lambda that takes a string and prints it to the console.
    // The compiler infers the delegate type as Action<string>.
    Action<string> print = message => Console.WriteLine(message);
    print("Hello, Lambda!");
    ```

### Anonymous Types

-   **Syntax**: `new { PropertyName1 = value1, PropertyName2 = value2 }`
-   **Usage**:
    ```csharp
    var person = new { Name = "John Doe", Age = 30 };
    Console.WriteLine($"{person.Name} is {person.Age} years old.");
    ```

### LINQ (Method Syntax)

-   **Syntax**: `collection.MethodName(lambdaExpression)`
-   **Usage**:
    ```csharp
    var numbers = new List<int> { 1, 2, 3, 4, 5, 6 };

    // Use Where (which takes a Predicate<T> or Func<T, bool>) to filter.
    var evenNumbers = numbers.Where(n => n % 2 == 0);

    // Use Select (which takes a Func<T, TResult>) to project into a new form.
    var squares = evenNumbers.Select(n => new { Original = n, Square = n * n });

    foreach (var item in squares)
    {
        Console.WriteLine($"The square of {item.Original} is {item.Square}");
    }
    ```

### Closures

A closure happens when a lambda expression accesses a variable from its containing scope (the "outer" method). The lambda "captures" this variable and can use it, even after the outer method has completed execution.

-   **Example**: Creating a function factory.

    ```csharp
    public static class FunctionFactory
    {
        public static Func<int, int> CreateAdder(int amountToAdd)
        {
            // This lambda expression "closes over" the amountToAdd variable.
            // It captures the variable's value at this moment.
            Func<int, int> adder = (number) => number + amountToAdd;
            return adder;
        }
    }

    // --- Usage ---
    // Create a function that will add 5 to any number.
    var add5 = FunctionFactory.CreateAdder(5);

    // Create another function that will add 10.
    var add10 = FunctionFactory.CreateAdder(10);

    // Even though the CreateAdder method has finished, the 'add5' delegate
    // still has access to its captured 'amountToAdd' variable (which is 5).
    int result1 = add5(100); // result1 is 105

    // The same is true for 'add10'.
    int result2 = add10(100); // result2 is 110

    Console.WriteLine($"Result 1: {result1}");
    Console.WriteLine($"Result 2: {result2}");
    ```

## Practical Example: Querying a Product Catalog

Let's create a real-world scenario where we query a list of products. We'll filter for electronics, sort them by price, and project the results into a new, simplified form.

```csharp
using System;
using System.Collections.Generic;
using System.Linq;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
}

public class LinqExample
{
    public static void Main()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Category = "Electronics", Price = 1200.50m },
            new Product { Id = 2, Name = "T-Shirt", Category = "Apparel", Price = 25.00m },
            new Product { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 75.99m },
            new Product { Id = 4, Name = "Jeans", Category = "Apparel", Price = 90.25m },
            new Product { Id = 5, Name = "Monitor", Category = "Electronics", Price = 350.00m }
        };

        // Define a filter using a generic delegate
        Func<Product, bool> isElectronic = p => p.Category == "Electronics";

        // Build the LINQ query
        var expensiveElectronics = products
            .Where(isElectronic) // Filter for electronics
            .Where(p => p.Price > 100) // Further filter for price
            .OrderByDescending(p => p.Price) // Sort by price
            .Select(p => new { p.Name, p.Price }); // Project to an anonymous type

        Console.WriteLine("Expensive Electronics (sorted by price):");
        foreach (var product in expensiveElectronics)
        {
            Console.WriteLine($"- {product.Name}: ${product.Price}");
        }
    }
}
```

### Why this code?

1.  **Declarative & Readable**: The LINQ query clearly states *what* we want, not *how* to get it. The chain of `Where`, `OrderByDescending`, and `Select` is easy to follow.
2.  **Composition**: We can chain multiple LINQ methods together to build complex queries from simple parts. We even used a pre-defined `Func` delegate (`isElectronic`) in our `Where` clause.
3.  **Efficiency**: The `Select` projection creates anonymous types containing only the `Name` and `Price`. This is memory-efficient as we are not carrying around the entire `Product` object if we only need a subset of its data for display.
4.  **Deferred Execution**: The query does not execute until the `foreach` loop starts enumerating it. This allows LINQ to potentially optimize the execution.

## Best Practices & Common Pitfalls

-   **Understand Deferred Execution**: LINQ queries are not executed when they are defined. They run when you iterate over the results (e.g., with `foreach`, `.ToList()`, `.First()`). This can cause issues if the underlying data source changes between query definition and execution.
-   **Avoid Multiple Enumerations**: Calling `ToList()` or `ToArray()` on a query will execute it and store the results in a collection. If you need to iterate over the results multiple times, do this once to avoid re-executing the entire query each time.
-   **Closures and Loop Variables**: Be careful when using loop variables inside a lambda. In older versions of C#, the lambda would capture the variable itself, not its value at each iteration. Modern C# has fixed this for `foreach` loops, but it's a classic pitfall to be aware of.
-   **`IEnumerable` vs. `IQueryable`**: Use `IQueryable` when querying databases (e.g., with Entity Framework). This ensures your C# lambda expressions are converted to efficient SQL. Using `IEnumerable` here would fetch the entire table into memory and then filter it, which is highly inefficient.
-   **Keep Lambdas Simple**: For complex logic, it's better to define a separate, named method and pass it to the LINQ operator. This improves readability and testability.

## Advanced Topics

-   **Expression Trees**: When you use a lambda with `IQueryable`, the compiler doesn't convert it into executable code. Instead, it creates an **Expression Tree** (`Expression<Func<T, bool>>`). This is a data structure that represents the code in your lambda. The LINQ provider (e.g., Entity Framework) can then inspect this tree and translate it into another language, like SQL.
-   **Custom LINQ Providers**: For advanced scenarios, you can implement your own LINQ provider by implementing the `IQueryable` and `IQueryProvider` interfaces. This would allow you to use LINQ syntax to query a custom data source, like a web API.

## Summary

Delegates, lambda expressions, and LINQ are a trio of features that fundamentally changed C# programming. Delegates provide the type-safe foundation for passing methods as arguments. Lambda expressions offer a concise and powerful syntax for creating these methods on the fly. LINQ leverages both to provide a rich, declarative, and unified query language for any data source. Mastering these concepts is essential for writing modern, clean, and efficient C# code.
