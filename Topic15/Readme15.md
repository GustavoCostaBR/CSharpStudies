# Topic 15: Generic Programming

## Overview of Generics in C#

Generics allow you to define type-safe data structures without committing to an actual data type. This feature was introduced in C# 2.0 and is a fundamental part of the language. Generics allow us to design classes and methods that defer the specification of one or more types until the class or method is declared and instantiated by client code.

The most common use of generics is to create collection classes. A generic collection class is type-safe because the type of the elements it contains is known at compile time.

## Motivation and Benefits of Generics

Before generics, the common way to create generalized code was to use `object` type. However, this approach has two major drawbacks:

1.  **Type Safety:** You lose compile-time type checking. You have to cast from `object` back to the desired type, which can lead to `InvalidCastException` at runtime.
2.  **Performance:** Using `object` often involves boxing and unboxing operations for value types, which can degrade performance.

Generics solve these problems by providing:

-   **Type Safety:** You get compile-time checking, which prevents runtime casting errors.
-   **Performance:** No boxing/unboxing is needed for value types, leading to better performance.
-   **Code Reusability:** Write code once and reuse it with different data types.

## Syntax of Generic Types and Methods

Generics use angle brackets (`<>`) to enclose a type parameter. The letter `T` is conventionally used as the name of the type parameter, but any valid identifier can be used.

-   **Generic Class:** `class MyClass<T> { ... }`
-   **Generic Method:** `public T MyMethod<T>(T value) { ... }`

## Built-in Generic Types

The .NET Framework provides a rich set of built-in generic types, primarily in the `System.Collections.Generic` namespace.

-   `List<T>`: A dynamic array.
-   `Dictionary<TKey, TValue>`: A collection of key-value pairs.
-   `Queue<T>`: A first-in, first-out (FIFO) collection.
-   `Stack<T>`: A last-in, first-out (LIFO) collection.
-   `HashSet<T>`: A collection of unique elements.

### Example: Using `List<T>`

```csharp
// A list that can only hold integers
List<int> numbers = new List<int>();
numbers.Add(1);
numbers.Add(2);
// numbers.Add("3"); // This would cause a compile-time error

// A list that can only hold strings
List<string> names = new List<string>();
names.Add("Alice");
names.Add("Bob");
```

## Creating Custom Generic Classes

You can create your own generic classes to encapsulate operations that are not specific to a particular data type.

### Example: A Generic `Box<T>` Class

This class can "hold" an item of any type.

```csharp
public class Box<T>
{
    public T Content { get; private set; }

    public Box(T content)
    {
        Content = content;
    }

    public void UpdateContent(T newContent)
    {
        Content = newContent;
    }
}
```

### Usage

```csharp
// A box for an integer
var intBox = new Box<int>(123);
Console.WriteLine($"Integer Box Content: {intBox.Content}");

// A box for a string
var stringBox = new Box<string>("Hello Generics");
Console.WriteLine($"String Box Content: {stringBox.Content}");

// intBox.UpdateContent("some string"); // Compile-time error
```

## Creating Custom Generic Methods

Methods can also be generic, even if they are part of a non-generic class.

### Example: A Generic `Swap<T>` Method

This method can swap two variables of any type.

```csharp
public class Swapper
{
    public static void Swap<T>(ref T a, ref T b)
    {
        T temp = a;
        a = b;
        b = temp;
    }
}
```

### Usage

```csharp
int x = 5;
int y = 10;
Console.WriteLine($"Before swap: x = {x}, y = {y}");
Swapper.Swap(ref x, ref y);
Console.WriteLine($"After swap: x = {x}, y = {y}");

string s1 = "Hello";
string s2 = "World";
Console.WriteLine($"Before swap: s1 = {s1}, s2 = {s2}");
Swapper.Swap(ref s1, ref s2);
Console.WriteLine($"After swap: s1 = {s1}, s2 = {s2}");
```

## Generic Interfaces

Interfaces can also be generic. This is very common for defining contracts for data structures and algorithms.

### Example: A Generic `IRepository<T>` Interface

```csharp
public interface IRepository<T>
{
    T GetById(int id);
    void Add(T entity);
    void Delete(T entity);
}

public class Product { /* ... */ }

public class ProductRepository : IRepository<Product>
{
    public Product GetById(int id) { /* implementation */ return null; }
    public void Add(Product entity) { /* implementation */ }
    public void Delete(Product entity) { /* implementation */ }
}
```

## Type Parameters and Type Safety

The primary benefit of generics is type safety. The compiler enforces that only the correct type is used with the generic class or method.

```csharp
List<int> numbers = new List<int>();
numbers.Add(1);
// numbers.Add("two"); // Compile-time error: cannot convert from 'string' to 'int'
```

## Constraints on Type Parameters (`where` clauses)

Constraints allow you to restrict the types that can be used as type arguments. This is useful when your generic code needs to call methods or access properties of the generic type.

The `where` clause is used to specify constraints.

| Constraint            | Description                                                              |
| --------------------- | ------------------------------------------------------------------------ |
| `where T : struct`    | The type argument must be a value type.                                  |
| `where T : class`     | The type argument must be a reference type.                              |
| `where T : new()`     | The type argument must have a public parameterless constructor.          |
| `where T : <base>`    | The type argument must be or derive from the specified base class.       |
| `where T : <interface>`| The type argument must be or implement the specified interface.          |
| `where T : U`         | The type argument `T` must be or derive from the type argument `U`.      |

### Example: Using `where T : new()`

```csharp
public class Factory<T> where T : new()
{
    public T CreateInstance()
    {
        return new T();
    }
}

// Usage
var factory = new Factory<SomeClass>();
SomeClass instance = factory.CreateInstance();

// class NoDefaultConstructor { public NoDefaultConstructor(int x) {} }
// var factory2 = new Factory<NoDefaultConstructor>(); // Compile-time error
```

## Multiple Type Parameters

A generic type or method can have multiple type parameters.

### Example: A Generic `Pair<T, U>` Class

```csharp
public class Pair<T, U>
{
    public T First { get; set; }
    public U Second { get; set; }

    public Pair(T first, U second)
    {
        First = first;
        Second = second;
    }
}

// Usage
var pair = new Pair<int, string>(1, "one");
Console.WriteLine($"Pair: {pair.First}, {pair.Second}");
```

## Default Values for Type Parameters (`default(T)`)

Sometimes you need to assign a default value to a generic type parameter. You can use the `default(T)` keyword.

-   For reference types, it returns `null`.
-   For numeric value types, it returns `0`.
-   For `structs`, it returns an instance with all members initialized to their default values.

### Example

```csharp
public T GetDefault<T>()
{
    return default(T);
}

int defaultInt = GetDefault<int>(); // 0
string defaultString = GetDefault<string>(); // null
```

## Covariance and Contravariance in Generics

These are advanced topics that relate to the ability to use a more or less derived type than specified by the generic parameter.

-   **Covariance (`out`)**: Enables you to use a more derived type. It applies to return types.
    -   `IEnumerable<string>` can be assigned to `IEnumerable<object>`.
-   **Contravariance (`in`)**: Enables you to use a less derived type. It applies to method arguments.
    -   `Action<object>` can be assigned to `Action<string>`.

### Example

```csharp
// Covariance (out)
IEnumerable<string> strings = new List<string> { "a", "b" };
IEnumerable<object> objects = strings; // This is allowed due to covariance

// Contravariance (in)
Action<object> printObject = obj => Console.WriteLine(obj);
Action<string> printString = printObject; // This is allowed due to contravariance
printString("hello");
```

## Generic Delegates and Events

Delegates can also be generic. The .NET Framework provides built-in generic delegates like `Action<T>` and `Func<T, TResult>`.

-   `Action<T>`: Represents a method that takes one or more arguments and returns no value.
-   `Func<T, TResult>`: Represents a method that takes one or more arguments and returns a value.

### Example

```csharp
// Func<T, TResult>
Func<int, int, int> add = (a, b) => a + b;
int sum = add(3, 4); // 7

// Action<T>
Action<string> print = message => Console.WriteLine(message);
print("Hello Generic Delegates");
```

## Generic Collections in `System.Collections.Generic`

As mentioned earlier, this namespace is the cornerstone of using generics in C#. Always prefer these collections over the non-generic ones in `System.Collections` (like `ArrayList`).

## Using Generics with LINQ

Language-Integrated Query (LINQ) is heavily based on generics. The standard query operators (like `Select`, `Where`, `OrderBy`) are generic extension methods that operate on `IEnumerable<T>`.

### Example

```csharp
List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6 };

// Where is a generic extension method: Where<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
var evenNumbers = numbers.Where(n => n % 2 == 0);

// Select is a generic extension method: Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
var squares = numbers.Select(n => n * n);
```

## Examples: Implementing a Generic Stack and Queue

### Generic Stack

```csharp
public class MyStack<T>
{
    private readonly List<T> _items = new List<T>();

    public void Push(T item)
    {
        _items.Add(item);
    }

    public T Pop()
    {
        if (_items.Count == 0)
            throw new InvalidOperationException("The stack is empty.");

        T item = _items[_items.Count - 1];
        _items.RemoveAt(_items.Count - 1);
        return item;
    }

    public int Count => _items.Count;
}

// Usage
var stack = new MyStack<int>();
stack.Push(1);
stack.Push(2);
Console.WriteLine(stack.Pop()); // 2
```

## Practical Usage Scenarios for Generics

-   **Repository Pattern:** Create a generic repository for data access to avoid writing boilerplate code for each entity type.
-   **API Result Wrappers:** Create a generic `ApiResult<T>` class to standardize API responses.
-   **Caching:** A generic cache class can store any type of object.

## Common Pitfalls and Best Practices

-   **Avoid `object`:** Prefer generics over using the `object` type for generalization.
-   **Use Constraints:** Apply constraints when your generic code needs to access members of the type parameter.
-   **Follow Naming Conventions:** Use `T` as the type parameter name for simple generics. For multiple parameters, use `T`, `U`, `V`, or descriptive names like `TKey`, `TValue`.
-   **Understand `default(T)`:** Be aware of how `default(T)` behaves for different types.

## Summary and Further Reading

Generics are a powerful feature of C# that enable the creation of reusable, type-safe, and high-performance code. They are fundamental to modern C# programming, especially when working with collections and LINQ.

For more information, refer to the official Microsoft documentation:
-   [Generics (C# Programming Guide)](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/generics/)
-   [System.Collections.Generic Namespace](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic)
