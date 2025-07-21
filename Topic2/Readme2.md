# Topic 2: C# Overview

This topic covers the basic structure of a C# program, input/output operations, best practices, and how to compile and debug your code.

## Program structure in C#

A C# program consists of one or more files. Each file can contain namespaces, and each namespace can contain types like classes, structs, interfaces, enums, and delegates. A basic C# program must have a `Main` method, which is the entry point for the application.

### The `Main` Method
The `Main` method is where your program starts execution. It's a `static` method that can be declared in a few ways, but it's most commonly found inside a `class` or a `struct`.

Here is a traditional example of a "Hello, World!" program showing the basic structure:
```csharp
// A 'using' directive to import types from a namespace
using System;

// A namespace to organize your code
namespace MyFirstApp
{
    // A class to contain your program's logic
    class Program
    {
        // The Main method, the application's entry point
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }
    }
}
```

### Top-Level Statements
As we saw in Topic 1, modern C# (since .NET 5) allows for a simpler program structure using **top-level statements**. This feature lets you write your program's entry point code directly in a file, without explicitly declaring a `Program` class or a `Main` method. The compiler generates the class and `Main` method for you.

The "Hello, World!" program can be simplified to a single line:
```csharp
// This is a complete, valid C# program.
Console.WriteLine("Hello, World!");
```
This is the default for new console applications and is perfect for beginners and small utilities, as it removes unnecessary ceremony. Only one file in your project can have top-level statements.

## Basic I/O operations

I/O (Input/Output) operations are fundamental for any application that needs to interact with a user. In console applications, this is primarily done through the `Console` class from the `System` namespace.

### Writing Output
The most common way to display text to the console is using `Console.WriteLine()`. This method prints a string of text to the console, followed by a line terminator.

```csharp
Console.WriteLine("This will be on the first line.");
Console.WriteLine("This will be on the second line.");
```

You can also use `Console.Write()` to print text without adding a new line at the end.

```csharp
Console.Write("Hello, ");
Console.Write("World!");
// Output: Hello, World!
```

You can use string interpolation to easily embed variable values within a string:
```csharp
string name = "Gustavo";
int age = 30;
Console.WriteLine($"My name is {name} and I am {age} years old.");
```

### Reading Input
To get input from the user, you can use the `Console.ReadLine()` method. This method reads a line of text from the console and returns it as a `string`.

Here's an example of a program that asks for the user's name and then greets them:
```csharp
Console.Write("Please enter your name: ");
string userName = Console.ReadLine();
Console.WriteLine($"Hello, {userName}!");
```

It's important to remember that `Console.ReadLine()` always returns a string. If you need to read a number or another type of data, you'll have to convert (or *parse*) the string into the desired type.

```csharp
Console.Write("How old are you? ");
string input = Console.ReadLine();
int userAge = int.Parse(input); // Parsing the string to an integer
Console.WriteLine($"Next year, you will be {userAge + 1} years old.");
```
**Note:** The `int.Parse()` method will throw an exception if the user enters text that cannot be converted to an integer. We'll cover robust error handling in a later topic.

## Recommended practices

Writing clean, readable, and maintainable code is crucial for any developer. Following established conventions makes your code easier for you and others to understand. Here are some fundamental best practices for C#.

### Naming Conventions
Consistent naming helps to understand the role of a piece of code at a glance.
- **PascalCase**: Use for class names, method names, properties, and records.
  ```csharp
  public class MyAwesomeClass
  {
      public int Data { get; set; }
      public void DoSomething() { /* ... */ }
  }
  ```
- **camelCase**: Use for local variables and method parameters.
  ```csharp
  string firstName = "John";
  int userAge = 30;
  
  public void GreetUser(string userName) { /* ... */ }
  ```
- **Prefix for private fields**: A common convention is to prefix private fields with an underscore (`_`).
  ```csharp
  public class Person
  {
      private readonly string _name;
      public Person(string name)
      {
          _name = name;
      }
  }
  ```

### Code Formatting
- **Braces**: Always use curly braces (`{}`) for `if`, `for`, `while`, etc., even if the body is a single line. This improves readability and prevents bugs.
  ```csharp
  // Good
  if (condition)
  {
      DoSomething();
  }

  // Avoid
  if (condition) DoSomething();
  ```
- **Indentation**: Use consistent indentation (typically 4 spaces) to reflect the code's structure. Most code editors, including VS Code, handle this automatically.

### Comments
Write comments to explain *why* you did something, not *what* you did. The code itself should clearly express what it's doing.
- Use `//` for single-line comments.
- Use `/* ... */` for multi-line comments.
- Use `///` for XML documentation comments on classes and methods. These are used by tools like IntelliSense to provide information about your code.
  ```csharp
  /// <summary>
  /// Calculates the sum of two integers.
  /// </summary>
  /// <param name="a">The first integer.</param>
  /// <param name="b">The second integer.</param>
  /// <returns>The sum of the two integers.</returns>
  public int Add(int a, int b)
  {
      return a + b;
  }
  ```

### Keep it Simple
- **Single Responsibility Principle (SRP)**: Aim for methods and classes that do one thing and do it well. A method called `GetUserAndSaveOrder` is likely doing too much. For example, instead of one large method, you should split the logic into smaller, more focused methods:

  ```csharp
  // Avoid this
  public void GetUserAndSaveOrder(int userId, OrderDetails order)
  {
      // Logic to get user from database...
      // Logic to validate the order...
      // Logic to save the order to the database...
  }

  // Prefer this
  public User GetUser(int userId)
  {
      // ... logic to get user
  }

  public bool ValidateOrder(OrderDetails order)
  {
      // ... logic to validate
  }

  public void SaveOrder(OrderDetails order)
  {
      // ... logic to save
  }
  ```

  As for the best practice on how to call these smaller methods, it often depends on the architecture, but a common pattern is to have a "service" or "manager" class that orchestrates these operations. This keeps the high-level business logic clear and separates it from the low-level implementation details.

  ```csharp
  public class OrderService
  {
      public void PlaceOrder(int userId, OrderDetails order)
      {
          User user = GetUser(userId);
          if (user != null && ValidateOrder(order))
          {
              SaveOrder(order);
              Console.WriteLine("Order placed successfully.");
          }
          else
          {
              Console.WriteLine("Failed to place order.");
          }
      }

      // The private methods would be here or in other specialized classes
      private User GetUser(int userId) { /* ... */ return new User(); }
      private bool ValidateOrder(OrderDetails order) { /* ... */ return true; }
      private void SaveOrder(OrderDetails order) { /* ... */ }
  }
  ```

- **Avoid "magic numbers" or strings**: Use named constants or enums instead of embedding literal values directly in your code. This makes the code more readable and easier to maintain.

  ```csharp
  // Using a const for a single value
  const int MaxLoginAttempts = 3;
  if (attempts < MaxLoginAttempts) { /* ... */ }

  // Using an enum for a set of related states or types.
  // By default, members are assigned values starting from 0 (Pending = 0, Processing = 1, etc.).
  // You can also explicitly assign integer values to each member, which is useful
  // when the values need to correspond to specific codes (e.g., from a database).
  public enum OrderStatus
  {
      Pending = 10,
      Processing = 20,
      Shipped = 30,
      Delivered = 40,
      Cancelled = 99
  }

  public void UpdateOrderStatus(OrderStatus status)
  {
      if (status == OrderStatus.Shipped)
      {
          // ... logic for shipping
      }
  }
  ```

## Program compilation and debugging

Once you've written your code, you need to compile and run it to see it in action. If you encounter errors or unexpected behavior, you'll need to debug it.

### Compilation
As we learned in Topic 1, the .NET CLI is your primary tool for this. To compile your application, navigate to your project's root directory in the terminal and run:
```bash
dotnet build
```
This command compiles your C# source code into Intermediate Language (IL) and packages it into an assembly (`.dll` or `.exe`). The output is placed in the `bin` folder within your project directory.

#### Build Configurations: Debug vs. Release
You'll notice a `Debug` subfolder inside `bin`. This is because the default build configuration is "Debug". .NET has two main build configurations:
- **Debug**: This configuration is used for development. It compiles your code with full symbolic debug information (`.pdb` files) and minimal optimizations. This allows the debugger to map the executing code back to your source code, letting you set breakpoints and inspect variables.
- **Release**: This configuration is for the final version of your application that you would deploy. It's highly optimized for performance and does not include debug symbols.

To build your application in the Release configuration, you would run:
```bash
dotnet build --configuration Release
```

### Running the Program
The easiest way to build and run your program during development is with the `dotnet run` command:
```bash
dotnet run
```
This command is a convenient shortcut that builds your code and immediately executes the resulting application.

### Debugging
Debugging is the process of finding and fixing defects in your code. With the C# Dev Kit extension in VS Code, you get a powerful, integrated debugging experience.

The basic workflow is:
1.  **Set a Breakpoint**: Find a line of code where you want to pause execution. Click in the margin to the left of the line number. A red dot will appear, indicating a breakpoint.
2.  **Start Debugging**: Press `F5` or go to the "Run and Debug" view and click the green play button. Your program will start and run until it hits your breakpoint.
3.  **Inspect and Step**: Once paused, you can:
    -   Hover over variables to see their current values.
    -   Use the "Variables" window to inspect all variables in the current scope.
    -   Use the debug toolbar to control execution:
        -   **Continue (F5)**: Resumes running until the next breakpoint.
        -   **Step Over (F10)**: Executes the current line and moves to the next one in the same method.
        -   **Step Into (F11)**: If the current line is a method call, it moves into that method's code.
        -   **Step Out (Shift+F11)**: Finishes executing the current method and returns to the line where it was called.
4.  **Stop Debugging**: Click the red stop button in the debug toolbar or press `Shift+F5`.

### A Note on Debugging in JetBrains Rider
JetBrains Rider, another popular IDE for .NET development, also offers a top-tier debugging experience with some powerful features. The basic principles of setting breakpoints and stepping through code are the same as in VS Code.

Some useful tools in Rider include:
- **Smart Step-Into**: When a line contains multiple method calls, Rider lets you choose which specific method to step into.
- **Run to Cursor**: You can place your cursor on a line of code and have the debugger run until it reaches that line, without needing to set a formal breakpoint.
- **Rich DataTips**: Rider's tooltips for inspecting variables are very powerful, allowing you to expand objects and even change their values on the fly during a debug session.
- **Immediate Window**: A powerful REPL-like window where you can execute code and evaluate expressions in the context of the current breakpoint.

We will explore debugging techniques more deeply in Topic 7.