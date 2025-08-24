# Topic 8: Basics of Object-Oriented Programming

Object-Oriented Programming (OOP) is a programming paradigm based on the concept of "objects", which can contain data in the form of fields (often known as attributes or properties), and code, in the form of procedures (often known as methods). This topic covers the fundamental building blocks of OOP in C#.

## Object Classes and Their Instances

A **class** is a blueprint for creating objects. It defines a set of properties and methods that are common to all objects of that type. An **object** is an instance of a class.

```csharp
// A simple class definition (the blueprint)
public class Car
{
    // Fields
    public string Model;
    public string Color;
    public int Year;

    // A method
    public void StartEngine()
    {
        Console.WriteLine($"The {Color} {Model}'s engine is starting.");
    }
}

// Creating instances (objects) of the Car class
var myCar = new Car();
myCar.Model = "Tesla Model 3";
myCar.Color = "Red";
myCar.Year = 2024;
myCar.StartEngine(); // Output: The Red Tesla Model 3's engine is starting.

var anotherCar = new Car
{
    Model = "Ford Mustang",
    Color = "Blue",
    Year = 2023
};
anotherCar.StartEngine(); // Output: The Blue Ford Mustang's engine is starting.
```

## Private and Public Members

Access modifiers control the visibility of class members. This is a core part of **encapsulation**, which bundles the data (fields) and the methods that operate on the data into a single unit (a class) and restricts access to some of the object's components.

-   `public`: The member is accessible from any other code.
-   `private`: The member is only accessible from within the same class. This is the default if no access modifier is specified.

It's a best practice to keep fields `private` and expose them through `public` methods (or properties, which are covered in Topic 9) to control how the data is accessed and modified.

```csharp
public class BankAccount
{
    // This field is private, it cannot be accessed directly from outside the class.
    private decimal _balance;

    // Public method to deposit money
    public void Deposit(decimal amount)
    {
        if (amount > 0)
        {
            _balance += amount;
        }
    }

    // Public method to get the current balance
    public decimal GetBalance()
    {
        return _balance;
    }
}

// Usage
var account = new BankAccount();
// account._balance = 1000000; // This will cause a compile error because _balance is private.
account.Deposit(500);
Console.WriteLine($"Current balance: {account.GetBalance()}"); // Output: Current balance: 500
```

## Using the `this` Keyword

The `this` keyword is a reference to the **current instance** of the class. It's most commonly used for two purposes:

1.  To distinguish between class members and local variables (like method parameters) that have the same name.
2.  To call another constructor from a constructor (constructor chaining).

```csharp
public class Employee
{
    private string _name;
    private string _employeeId;

    // 1. Using 'this' to distinguish field from parameter
    public Employee(string name, string employeeId)
    {
        this._name = name;
        this._employeeId = employeeId;
    }

    // 2. Using 'this' to chain constructors
    public Employee(string name) : this(name, "N/A")
    {
        // This constructor calls the one above, providing a default value for employeeId.
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Name: {this._name}, ID: {this._employeeId}");
    }
}
```

## Static Members

A `static` member (field, method, or property) belongs to the **class itself**, rather than to a specific instance (object). You access static members using the class name, not an instance variable.

-   **Static Fields**: A single copy of the data is shared among all instances of the class.
-   **Static Methods**: Can be called without creating an instance of the class. They can only access other static members.

```csharp
public class AppConfig
{
    // Static field: shared across the entire application
    public static string ApiUrl = "https://api.example.com";
    private static int _instanceCounter = 0;

    public AppConfig()
    {
        // Increment the static counter each time a new instance is created
        _instanceCounter++;
    }

    // Static method
    public static void DisplayApiUrl()
    {
        Console.WriteLine($"API URL is: {ApiUrl}");
    }

    // Instance method to show the count
    public void ShowInstanceCount()
    {
        // An instance method can access both static and instance members
        Console.WriteLine($"Total instances created: {_instanceCounter}");
    }
}

// Usage
AppConfig.ApiUrl = "https://api.new-example.com"; // Change the static field
AppConfig.DisplayApiUrl(); // Call the static method

var config1 = new AppConfig();
var config2 = new AppConfig();
config2.ShowInstanceCount(); // Output: Total instances created: 2
```

## Advanced Scenarios & Concepts

### Other Access Modifiers

Besides `public` and `private`, C# has other access modifiers:

-   `internal`: The member is accessible only within files in the same project (assembly). This is useful for creating helper classes that you don't want to expose to other projects that might reference yours.
-   `protected`: The member is accessible within its class and by derived class instances. This is fundamental to inheritance (covered in Topic 12).

### Readonly Fields

A `readonly` field can only be assigned a value either at its declaration or within a constructor of the same class. After the constructor finishes, its value cannot be changed, providing a level of immutability.

```csharp
public class LogEntry
{
    public readonly DateTime Timestamp; // Can only be set in the constructor
    public readonly string Message;

    public LogEntry(string message)
    {
        this.Timestamp = DateTime.UtcNow; // Set readonly field
        this.Message = message;
    }

    public void UpdateMessage(string newMessage)
    {
        // this.Message = newMessage; // Compile error: A readonly field cannot be assigned to (except in a constructor)
    }
}
```

### Static Classes

If a class is declared as `static`, it cannot be instantiated or inherited. It can only contain static members. This is ideal for creating utility or helper classes that do not need to maintain any instance-specific state. The `System.Math` class is a perfect example.

```csharp
public static class MathHelper
{
    public static double PI = 3.14159;

    public static int Square(int number)
    {
        return number * number;
    }
}

// Usage
int squared = MathHelper.Square(5); // Call method directly on the class
```
