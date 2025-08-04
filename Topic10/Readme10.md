# Topic 10: Reference Types and Object Hierarchies

This topic delves into one of the most fundamental concepts in C#: **reference types**. Understanding how they work is crucial for managing memory and building complex, interconnected applications.

## Use of References

In C#, types are divided into two main categories: value types and reference types.

-   **Value Types** (like `int`, `double`, `bool`, and `struct`): A variable of a value type directly contains its data. When you copy it, you create a full, independent duplicate.
-   **Reference Types** (like `class`, `record`, `string`, `object`, and arrays): A variable of a reference type does not store the object directly. Instead, it stores a **reference** (similar to an address or a pointer) to the location in memory where the object lives.

This distinction has significant implications.

### Memory Allocation: Stack vs. Heap

-   **The Stack**: A highly efficient, last-in-first-out (LIFO) region of memory. It's used for static memory allocation, which includes tracking method calls and storing value types. Memory management is simple and fast.
-   **The Heap**: A large region of memory used for dynamic allocation. All reference type objects are created on the heap. The .NET Garbage Collector is responsible for cleaning up objects on the heap that are no longer referenced.

When you declare a reference type variable, the *reference* itself lives on the stack, but the *object* it points to is on the heap.

### Behavior of References

#### 1. Assignment Copies the Reference

When you assign one reference variable to another, you are only copying the reference, not the object itself. Both variables will now point to the **same object** in memory.

```csharp
public class Pen
{
    public string InkColor { get; set; }
}

// --- Usage ---
var pen1 = new Pen { InkColor = "Blue" };
var pen2 = pen1; // Copy the reference, NOT the object.

// Now, modify the object through the second reference.
pen2.InkColor = "Red";

// The change is visible through the first reference because it's the same object.
Console.WriteLine(pen1.InkColor); // Output: Red
```

#### 2. Passing to Methods

When you pass a reference type to a method, a copy of the reference is passed. This means the method parameter also points to the original object, allowing the method to modify it.

```csharp
public void ChangeColor(Pen pen)
{
    // This 'pen' parameter points to the same object passed in.
    pen.InkColor = "Black";
}

// --- Usage ---
var myPen = new Pen { InkColor = "Green" };
Console.WriteLine(myPen.InkColor); // Output: Green

ChangeColor(myPen); // Pass the reference to the method.

Console.WriteLine(myPen.InkColor); // Output: Black (The original object was modified)
```

## Object Hierarchies (Composition)

While "hierarchy" often makes us think of inheritance (covered in Topic 12), a more common way to build object hierarchies is through **composition**. This is where an object is "composed" of other objects, meaning it holds references to them as members.

This allows you to model complex "has-a" relationships. For example, a `Car` *has an* `Engine`.

```csharp
// The Engine is a component part.
public class Engine
{
    public int Horsepower { get; set; }
    public bool IsRunning { get; private set; }

    public void Start() => IsRunning = true;
    public void Stop() => IsRunning = false;
}

// The Car class is composed of an Engine.
public class Car
{
    // The Car "has-a" reference to an Engine object.
    public Engine Engine { get; }
    public string Model { get; set; }

    public Car(string model, int horsepower)
    {
        this.Model = model;
        // This creates a hierarchy: the Car object now "owns" an Engine object.
        this.Engine = new Engine { Horsepower = horsepower };
    }

    public void StartCar()
    {
        Console.WriteLine($"Starting the {Model}...");
        Engine.Start();
        Console.WriteLine("Engine is running.");
    }
}

// --- Usage ---
var myCar = new Car("Tesla Model S", 778);
myCar.StartCar();
Console.WriteLine($"Is the car's engine running? {myCar.Engine.IsRunning}"); // Output: True
```

In this hierarchy, the `Car` is the parent object, and the `Engine` is a child object. The `Car`'s lifetime often controls the `Engine`'s lifetime. This is a powerful and flexible way to structure complex systems without relying on inheritance.
