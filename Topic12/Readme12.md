# Topic 12: Inheritance, Polymorphism, and Interfaces

This topic covers the core pillars of object-oriented programming (OOP): inheritance, polymorphism, and interfaces. These concepts allow you to create flexible, reusable, and well-structured code by defining relationships between different types.

## Inheritance and Derivation

**Inheritance** is a mechanism where a new class—known as a **derived class** or **subclass**—acquires the properties and behaviors of an existing class—known as a **base class** or **superclass**. This models an "is-a" relationship (e.g., a `Car` *is a* `Vehicle`).

-   `public`: Members are accessible to everyone.
-   `protected`: Members are accessible only within the class itself and by any class that derives from it.

```csharp
// The base class
public class Vehicle
{
    public int Year { get; set; }
    protected int Mileage { get; set; } // Accessible to Vehicle and its children

    public Vehicle(int year)
    {
        this.Year = year;
        Console.WriteLine("Vehicle constructor called.");
    }

    public void Start()
    {
        Console.WriteLine("Vehicle started.");
    }
}

// The derived class
public class Car : Vehicle // The ':' syntax denotes inheritance
{
    public string Model { get; set; }

    // The 'base(year)' call ensures the base class constructor is executed first.
    public Car(string model, int year) : base(year)
    {
        this.Model = model;
        this.Mileage = 0; // Can access the protected member from Vehicle
        Console.WriteLine("Car constructor called.");
    }
}
```

## Polymorphism and Virtual Methods

**Polymorphism** (from Greek, meaning "many forms") allows objects of different derived classes to be treated as objects of a common base class. The specific version of a method that gets executed is determined at runtime based on the actual type of the object.

This is achieved using `virtual` and `override` methods.

-   `virtual`: A method in a base class that can be overridden by derived classes.
-   `override`: A method in a derived class that provides a new implementation for a `virtual` method from its base class.

A key related concept is the `abstract` method. While a `virtual` method *can* be overridden, an `abstract` method (which can only exist in an `abstract` class, as covered in the "Advanced Concepts" section) provides no implementation and *must* be overridden by any non-abstract derived class. It is another core part of enabling polymorphism.

```csharp
public class Shape
{
    // This method can be overridden by derived classes.
    public virtual void Draw()
    {
        Console.WriteLine("Drawing a generic shape.");
    }
}

public class Circle : Shape
{
    // This provides a specific implementation for Circle.
    public override void Draw()
    {
        Console.WriteLine("Drawing a circle.");
    }
}

public class Square : Shape
{
    public override void Draw()
    {
        Console.WriteLine("Drawing a square.");
    }
}

// --- Polymorphism in action ---
var shapes = new List<Shape>
{
    new Shape(),
    new Circle(),
    new Square()
};

foreach (var shape in shapes)
{
    shape.Draw(); // The correct Draw() method is called for each object's actual type.
}
// Output:
// Drawing a generic shape.
// Drawing a circle.
// Drawing a square.
```

## Data Type Conversion (Casting)

When working with inheritance hierarchies, you can convert between base and derived types.

-   **Upcasting (Implicit)**: Converting from a derived type to a base type. This is always safe and happens automatically.
-   **Downcasting (Explicit)**: Converting from a base type to a derived type. This is risky because it can fail at runtime if the object isn't actually of the target derived type.

```csharp
var myCircle = new Circle();

// 1. Upcasting (safe and implicit)
Shape shapeFromCircle = myCircle;

// 2. Downcasting (requires an explicit cast)
// Circle circleFromShape = (Circle)shapeFromCircle; // This works

// --- Safe Downcasting ---
Shape someShape = new Square();
// Circle riskyCircle = (Circle)someShape; // Throws InvalidCastException at runtime!

// Use the 'as' operator for safe downcasting. It returns null on failure.
Circle safeCircle = someShape as Circle;
if (safeCircle == null)
{
    Console.WriteLine("The shape is not a circle.");
}

// Use the 'is' operator to check the type before casting.
if (someShape is Square)
{
    Square mySquare = (Square)someShape;
    Console.WriteLine("Successfully cast to Square.");
}
```

## Boxing and Unboxing

Boxing is the process of converting a **value type** (like `int` or a `struct`) into a reference type object (`object` or an interface). Unboxing is the reverse process of extracting the value type from the object.

-   **Boxing**: A new object is allocated on the heap, and the value-type's data is copied into it.
-   **Unboxing**: The data is copied from the heap object back into a value-type variable on the stack. This requires an explicit cast and a runtime type check to ensure the conversion is valid.

### Why is This Powerful?

The primary power of boxing and unboxing lies in C#'s **unified type system**. It allows value types, which are normally lightweight and live on the stack, to be treated as objects when necessary. This was particularly crucial for:

1.  **Non-Generic Collections**: Before generics were introduced in C# 2.0, collections like `ArrayList` could only store `object`. Boxing allowed them to store any type, including `int`, `double`, or custom `structs`.
2.  **Interoperability**: When working with APIs or older libraries that expect `object` parameters, boxing provides a seamless bridge to pass value types.
3.  **Reflection**: When you inspect and manipulate types at runtime, you often work with them as `object` instances.

While modern C# with generics (`List<T>`, `Dictionary<TKey, TValue>`) has greatly reduced the need for boxing, understanding it is still fundamental to grasping the C# type system.

### Examples of Boxing and Unboxing

```csharp
// --- Example 1: Basic Boxing ---
int myNumber = 42;

// Boxing: The integer value is "boxed" into an object on the heap.
object boxedNumber = myNumber;

// Unboxing: The value is extracted back into an integer.
int unboxedNumber = (int)boxedNumber;
Console.WriteLine($"Unboxed number: {unboxedNumber}");


// --- Example 2: Boxing with non-generic collections (the old way) ---
var mixedList = new System.Collections.ArrayList();
mixedList.Add(10); // int is boxed to object
mixedList.Add("hello"); // string is already a reference type
mixedList.Add(new Point(5, 5)); // struct is boxed to object

// To use the value, it must be unboxed with a cast
int firstNumber = (int)mixedList[0];


// --- Example 3: Unboxing to the wrong type ---
object anotherBoxedNumber = 99;
try
{
    // This will throw an InvalidCastException because the object
    // contains an 'int', not a 'long'.
    long willFail = (long)anotherBoxedNumber;
}
catch (InvalidCastException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
```

While powerful, boxing and unboxing have a performance cost due to heap allocation and type checking. They should be used judiciously, especially in performance-critical code. Modern code should always prefer generic collections (`List<int>` instead of `ArrayList`) to avoid unnecessary boxing.

## Interface Implementation

An **interface** is a contract that defines a set of related functionalities that a non-abstract `class` or `struct` *must* implement. It's a powerful tool for achieving abstraction and polymorphism across unrelated types.

-   An interface is a blueprint for a "can-do" relationship (e.g., a `Car` *can be* `IDisposable`, a `Customer` *can be* `ILoggable`).
-   A class can inherit from only one base class, but it can implement **multiple interfaces**.
-   Interfaces themselves cannot be instantiated.

### What Can an Interface Contain?

An interface can define the signatures for:
-   **Methods**: `void MyMethod();`
-   **Properties**: `int MyProperty { get; set; }`
-   **Events**: `event EventHandler MyEvent;`
-   **Indexers**: `T this[int index] { get; }`

### Example: Properties and Multiple Interfaces

Let's model a system where items can be sold and shipped. These are separate concerns, so we can define them in separate interfaces.

```csharp
// Defines what is required for an item to be sold
public interface ISellable
{
    decimal Price { get; } // A read-only property
    string Sku { get; }     // Stock Keeping Unit

    void AddToCart();
}

// Defines what is required for an item to be shipped
public interface IShippable
{
    double WeightKg { get; }
    double VolumeCm3 { get; }
}

// This class implements BOTH interfaces
public class PhysicalProduct : ISellable, IShippable
{
    public string Name { get; set; }
    public decimal Price { get; private set; }
    public string Sku { get; private set; }
    public double WeightKg { get; private set; }
    public double VolumeCm3 { get; private set; }

    public PhysicalProduct(string name, decimal price, string sku, double weight, double volume)
    {
        Name = name; Price = price; Sku = sku; WeightKg = weight; VolumeCm3 = volume;
    }

    public void AddToCart()
    {
        Console.WriteLine($"Adding {Name} to cart at ${Price}.");
    }
}

// This class only implements one interface
public class DigitalSubscription : ISellable
{
    public string ServiceName { get; set; }
    public decimal Price { get; private set; }
    public string Sku { get; private set; }

    public DigitalSubscription(string name, decimal price, string sku)
    {
        ServiceName = name; Price = price; Sku = sku;
    }

    public void AddToCart()
    {
        Console.WriteLine($"Adding subscription to {ServiceName} to cart.");
    }
}
```

This design allows us to write methods that work with any `IShippable` or any `ISellable` object, decoupling the logic from the concrete types.

```csharp
public void ProcessShoppingCart(List<ISellable> items)
{
    foreach (var item in items)
    {
        item.AddToCart(); // We only care that the item is sellable.
    }
}
```

### Advanced Feature: Default Interface Methods (C# 8.0+)

Since C# 8.0, interfaces can provide a default implementation for a member. This is useful for adding new members to an existing public interface without breaking all the classes that already implement it.

```csharp
public interface ILoggable
{
    string GetLogMessage();

    // Default implementation. Existing classes don't have to implement this.
    void LogToConsole()
    {
        Console.WriteLine(GetLogMessage());
    }
}

public class Customer : ILoggable
{
    public string Name { get; set; }
    public string GetLogMessage() => $"Customer: {Name}";
}

// Usage
var customer = new Customer { Name = "ACME Corp" };
customer.LogToConsole(); // We can call the default method.
```

### Advanced Feature: Explicit Interface Implementation

Sometimes a class may need to implement two interfaces that have a member with the same name, or you may want to hide an interface's method from the class's public API. **Explicit implementation** solves this. The member is only accessible when the object is cast to the interface type.

```csharp
interface IEnglishSpeaker { string Greet(); }
interface IFrenchSpeaker { string Greet(); }

public class MultilingualPerson : IEnglishSpeaker, IFrenchSpeaker
{
    // Explicit implementation for English
    string IEnglishSpeaker.Greet() => "Hello!";

    // Explicit implementation for French
    string IFrenchSpeaker.Greet() => "Bonjour!";

    // A public method for the class itself
    public string GreetInGerman() => "Guten Tag!";
}

// Usage
var person = new MultilingualPerson();
// person.Greet(); // Compile Error: No Greet method on the class itself.

// To call a specific version, cast to the interface
IEnglishSpeaker englishSpeaker = person;
Console.WriteLine(englishSpeaker.Greet()); // Output: Hello!

IFrenchSpeaker frenchSpeaker = person;
Console.WriteLine(frenchSpeaker.Greet()); // Output: Bonjour!
```

### Why Interfaces Are So Useful

1.  **Decoupling**: Interfaces allow you to separate the "what" (the contract) from the "how" (the implementation). Code that depends on an interface doesn't need to know about the concrete classes, making your system easier to change and maintain.
2.  **Testability (Mocking)**: When writing unit tests, you can easily create "mock" or "fake" implementations of an interface. This allows you to test a piece of code in isolation without depending on external systems like a real database or web service.
3.  **Polymorphism**: They enable polymorphism for completely unrelated classes that share a common capability.
4.  **Multiple Inheritance of Behavior**: While C# only allows single inheritance for classes, a class can implement any number of interfaces, allowing it to "inherit" and express multiple behaviors.

## Advanced Concepts & Best Practices

### Abstract Classes

An `abstract` class is a special base class that cannot be instantiated on its own. It's intended only to be inherited from. It can contain `abstract` members, which are methods without a body that *must* be overridden by derived classes.

-   **Use an `abstract` class** when you want to provide some common, shared implementation but also force derived classes to provide their own specific implementation for certain methods.

```csharp
public abstract class DatabaseProvider
{
    public string ConnectionString { get; protected set; }

    // A concrete method with shared logic
    public void Connect()
    {
        Console.WriteLine("Connecting to database...");
    }

    // An abstract method that derived classes MUST implement
    public abstract void ExecuteQuery(string query);
}

public class SqlProvider : DatabaseProvider
{
    public override void ExecuteQuery(string query)
    {
        Console.WriteLine($"Executing SQL query: {query}");
    }
}
```

### Sealed Classes

A `sealed` class cannot be inherited from. It marks the end of an inheritance chain.

-   **Use `sealed`** when you want to prevent further derivation of your class, either for design reasons or for performance optimizations (the compiler can make better decisions with sealed types). `string` is a well-known sealed class in .NET.

```csharp
public sealed class MyImmutableConfig
{
    // ...
}
```

### Reflection on Best Practices

-   **Favor Composition Over Inheritance**: Inheritance creates a strong, sometimes rigid, coupling. Before using inheritance, ask if a "has-a" relationship (composition) is more appropriate than an "is-a" relationship.
-   **Use Interfaces for Capabilities**: Define what an object *can do* with an interface. This is more flexible than base-class inheritance.
-   **Use Abstract Classes for Shared Core Logic**: Use an abstract class when you have a group of related classes that share a significant amount of common code and a core identity.
