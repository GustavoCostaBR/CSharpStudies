# Topic 9: Properties and Indexers

This topic builds upon the basics of classes by introducing more advanced and convenient ways to encapsulate and access data using properties and indexers.

## Properties

In Topic 8, we used `public` methods (`GetValue`, `SetValue`) to control access to `private` fields. C# provides a more elegant and powerful mechanism for this called **properties**. Properties expose fields in a safe and controlled way, but to the outside world, they look and feel just like public fields.

A property is composed of one or two code blocks called **accessors**: a `get` accessor and a `set` accessor.

```csharp
public class Employee
{
    private string _name; // The private "backing field"

    // The public property
    public string Name
    {
        // The 'get' accessor is executed when the property is read.
        get
        {
            return _name;
        }

        // The 'set' accessor is executed when the property is assigned a value.
        // The assigned value is available through the implicit 'value' keyword.
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Name cannot be null or empty.");
            }
            _name = value;
        }
    }
}

// Usage
var emp = new Employee();
emp.Name = "John Doe"; // This calls the 'set' accessor.
string employeeName = emp.Name; // This calls the 'get' accessor.
Console.WriteLine(employeeName); // Output: John Doe
```

### Auto-Implemented Properties

If your property does not require any custom logic in the accessors, C# provides a much shorter syntax called **auto-implemented properties**. The compiler automatically creates a private, anonymous backing field for you.

```csharp
// This single line...
public string Department { get; set; }

// ...is roughly equivalent to this:
private string _departmentBackingField;
public string Department
{
    get { return _departmentBackingField; }
    set { _departmentBackingField = value; }
}
```

### Why Use Auto-Implemented Properties Over Public Fields?

At first glance, an auto-implemented property like `public string Department { get; set; }` might seem identical to a public field `public string Department;`. However, using a property offers significant advantages and is considered a best practice in C#:

1.  **Future-Proofing (Binary Compatibility)**: This is the most critical reason. If you start with a public field and later need to add logic (like validation or notification), you must change it to a property. This change is a *breaking change* for any other code that uses your class and has already been compiled. If you start with a property, you can add a backing field and logic to the `get`/`set` accessors later without breaking any existing code.

2.  **Interface Implementation**: Interfaces can define properties, but not fields. If your class needs to implement an interface that requires a certain member, you must use a property.

3.  **Data Binding**: UI frameworks (WPF, Blazor, MAUI, etc.) and serialization libraries are built to work with properties. They often use reflection to discover and bind to an object's properties, and they will not work with public fields.

4.  **Finer Access Control**: Properties allow for more granular control. You can make a property read-only (`{ get; }`), init-only (`{ get; init; }`), or give the setter a more restrictive access level (`public string Name { get; private set; }`). This level of control is impossible with a simple public field.

5.  **Debugging**: You can set breakpoints on the `get` or `set` accessor of a property. This allows you to catch every time the property is read or changed, which is invaluable for debugging and impossible with a public field.

In short, while they may look similar in their simplest form, properties provide a layer of abstraction that is essential for writing robust, maintainable, and flexible C# code.

## Advanced Property Features

### `init` Accessor (Immutable Properties)

Introduced in C# 9, the `init` accessor allows you to create immutable properties. An `init`-only property can only be set during object initialization (i.e., in the constructor or using an object initializer). After that, it becomes read-only.

```csharp
public class Transaction
{
    // This property can only be set when a Transaction object is created.
    public Guid TransactionId { get; init; }
    public decimal Amount { get; set; }

    public Transaction()
    {
        TransactionId = Guid.NewGuid(); // Can be set in constructor
    }
}

// Usage
var tx = new Transaction
{
    Amount = 100.0m,
    // TransactionId = Guid.NewGuid() // This is also valid during initialization
};

// tx.TransactionId = Guid.NewGuid(); // Compile Error: The property is init-only.
```

### Properties with Different Access Modifiers

You can specify a different access modifier for one of the accessors to restrict its usage. A common pattern is to have a public `get` and a `private` or `protected` `set`.

```csharp
public class Order
{
    // The order status can be read by anyone, but only set from within the class.
    public string Status { get; private set; }

    public Order()
    {
        Status = "Pending"; // The private setter can be used here.
    }

    public void Ship()
    {
        // Logic to ship the order...
        Status = "Shipped";
    }
}
```

### Expression-Bodied Properties

For simple properties that just compute a value, you can use expression-body syntax for a more concise implementation.

```csharp
public class Person
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    // This read-only property is implemented as an expression.
    public string FullName => $"{FirstName} {LastName}";
}

// Usage
var person = new Person { FirstName = "Jane", LastName = "Doe" };
Console.WriteLine(person.FullName); // Output: Jane Doe
```

## Indexers

Indexers allow instances of a class or struct to be indexed just like arrays. This is particularly useful when creating custom collection classes. An indexer is defined similarly to a property but uses the `this` keyword and square brackets `[]` to declare the parameters.

```csharp
public class Scorecard
{
    private readonly int[] _scores = new int[10];

    // The indexer
    public int this[int round]
    {
        get
        {
            if (round < 0 || round >= _scores.Length)
            {
                throw new IndexOutOfRangeException("Invalid round number.");
            }
            return _scores[round];
        }
        set
        {
            if (round < 0 || round >= _scores.Length)
            {
                throw new IndexOutOfRangeException("Invalid round number.");
            }
            if (value < 0)
            {
                throw new ArgumentException("Score cannot be negative.");
            }
            _scores[round] = value;
        }
    }
}

// Usage
var scorecard = new Scorecard();
scorecard[0] = 95; // Calls the 'set' accessor of the indexer.
scorecard[1] = 100;
int firstRoundScore = scorecard[0]; // Calls the 'get' accessor.

Console.WriteLine($"Score for round 1: {firstRoundScore}"); // Output: Score for round 1: 95
```
