# Topic 4: Program blocks

Program blocks are fundamental to programming. They allow you to control the flow of execution in your code, making decisions based on certain conditions or repeating actions multiple times. In C#, this is primarily achieved through **branching** (conditional statements) and **iteration** (loops).

## Program branching

Branching allows your program to execute different blocks of code based on whether a condition is true or false. This is the basis of decision-making in software.

### The `if` statement
The `if` statement is the most basic conditional statement. It executes a block of code only if a specified boolean condition evaluates to `true`.

```csharp
int userAge = 20;

if (userAge >= 18)
{
    Console.WriteLine("User is an adult.");
}
```

### The `else` statement
You can pair an `if` statement with an `else` statement to provide an alternative block of code to execute if the condition is `false`.

```csharp
int userAge = 16;

if (userAge >= 18)
{
    Console.WriteLine("User is an adult.");
}
else
{
    Console.WriteLine("User is a minor.");
}
```

### The `else if` statement
To handle multiple conditions in a sequence, you can use `else if`. C# will check each condition in order and execute the *first* block where the condition is `true`. If none are true, the final `else` block is executed (if it exists).

```csharp
int score = 85;

if (score >= 90)
{
    Console.WriteLine("Grade: A");
}
else if (score >= 80)
{
    Console.WriteLine("Grade: B"); // This block will be executed
}
else if (score >= 70)
{
    Console.WriteLine("Grade: C");
}
else
{
    Console.WriteLine("Grade: F");
}
```

### The Ternary Conditional Operator (`?:`)
For simple `if-else` logic that results in assigning a value to a variable, you can use the ternary operator for a more concise syntax.

```csharp
// Using if-else
string message;
if (userAge >= 18)
{
    message = "Welcome.";
}
else
{
    message = "Access denied.";
}

// Using the ternary operator
string conciseMessage = (userAge >= 18) ? "Welcome." : "Access denied.";
```

### The `switch` statement
When you have a single variable that you need to compare against many possible constant values, a `switch` statement can be cleaner and more readable than a long chain of `if-else if` statements.

Each `case` represents a value to check for. The `break` keyword is required to exit the `switch` block once a match is found. The `default` case is optional and runs if no other case matches.

```csharp
public enum DayOfWeek { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday }

DayOfWeek today = DayOfWeek.Monday;

switch (today)
{
    case DayOfWeek.Saturday:
    case DayOfWeek.Sunday:
        Console.WriteLine("It's the weekend!");
        break;
    case DayOfWeek.Monday:
        Console.WriteLine("Back to work.");
        break;
    default:
        Console.WriteLine("It's a weekday.");
        break;
}
```

### The `switch` expression (C# 8.0 and later)
Modern C# introduced the `switch` expression, which is a more concise and powerful way to write `switch` logic, especially when the result is assigned to a variable.

Key differences from the `switch` statement:
- It's an expression, meaning it evaluates to a value.
- It uses `=>` instead of `case` and colons.
- Cases are separated by commas.
- The `_` (discard) character is used for the default case.
- It's exhaustive: the compiler will warn you if you haven't covered all possible input values (e.g., all members of an `enum`).

```csharp
string messageForToday = today switch
{
    DayOfWeek.Saturday or DayOfWeek.Sunday => "It's the weekend!",
    DayOfWeek.Monday => "Back to work.",
    _ => "It's a weekday."
};

Console.WriteLine(messageForToday);
```

Switch expressions can also use relational patterns for more complex conditions:
```csharp
string GetGrade(int score) => score switch
{
    >= 90 => "A",
    >= 80 => "B",
    >= 70 => "C",
    _ => "F"
};

Console.WriteLine($"A score of 85 is a grade of {GetGrade(85)}"); // Output: A score of 85 is a grade of B
```

### The Many Uses of the `?` Operator (Null-Related Operators)
The `?` character is central to C#'s features for handling null values, making code safer and more concise. Let's break down its different uses.

#### 1. The Ternary Conditional Operator (`?:`)
As we've seen, this operator is a compact `if-else` for assigning a value.

```csharp
// variable = (condition) ? value_if_true : value_if_false;
string conciseMessage = (userAge >= 18) ? "Welcome." : "Access denied.";
```

#### 2. Nullable Value Types (`T?`)
By default, value types like `int` or `bool` cannot be `null`. Appending a `?` makes them "nullable," meaning they can hold their usual range of values *or* the value `null`. This is essential when dealing with data from databases or external sources where a value might be missing.

```csharp
// int nonNullableAge = null; // Compile error
int? nullableAge = null; // This is valid

if (nullableAge.HasValue)
{
    Console.WriteLine($"Age is: {nullableAge.Value}");
}
else
{
    Console.WriteLine("Age is not provided.");
}
```

#### 3. The Null-Conditional Operator (`?.` and `?[]`)
Also known as the "Elvis operator," this is used to safely access members or elements of an object that might be `null`. If the object on the left of the `?.` is `null`, the expression short-circuits and returns `null` immediately, instead of throwing a `NullReferenceException`.

```csharp
public class User {
    public string? Name { get; set; }
    public List<string>? Roles { get; set; }
}

User? currentUser = null;

// Without the operator, this would throw an exception:
// string name = currentUser.Name; 

// With the operator, it safely returns null.
string? name = currentUser?.Name; 
Console.WriteLine(name); // Output: null

// It also works for methods and indexers.
int? roleCount = currentUser?.Roles?.Count; // You can chain them!
string? firstRole = currentUser?.Roles?[0]; // Safely access an element.

Console.WriteLine($"Role count: {roleCount}"); // Output: Role count: 
```

#### 4. The Null-Coalescing Operator (`??`)
This operator provides a default value for a nullable type or a reference type that might be `null`. It says, "if the value on the left is `null`, then use the value on the right."

```csharp
string? userNameFromDb = null;

// If userNameFromDb is null, use "Guest".
string displayName = userNameFromDb ?? "Guest";
Console.WriteLine(displayName); // Output: Guest

string? actualUserName = "Gustavo";
displayName = actualUserName ?? "Guest";
Console.WriteLine(displayName); // Output: Gustavo
```

#### 5. The Null-Coalescing Assignment Operator (`??=`)
This operator, introduced in C# 8.0, assigns the value of the right-hand operand to the left-hand operand only if the left-hand operand is `null`. It's a convenient shorthand.

```csharp
List<int>? numbers = null;

// This line says: "if numbers is null, create a new list and assign it to numbers."
numbers ??= new List<int>();
numbers.Add(5);

Console.WriteLine(numbers.Count); // Output: 1

// If numbers was not null, the assignment would not happen.
numbers ??= new List<int>() { 10 }; // This does nothing.
Console.WriteLine(numbers[0]); // Output: 5
```

## Iteration

Iteration, or looping, allows you to execute a block of code repeatedly. This is essential for processing collections of data, repeating an action until a condition is met, or simply running a task a specific number of times.

### The `for` loop
The `for` loop is ideal when you know exactly how many times you want to iterate. It consists of three parts: an initializer, a condition, and an iterator.

-   **Initializer**: Executed once before the loop starts. Typically used to declare and initialize a loop counter variable.
-   **Condition**: A boolean expression that is checked *before* each iteration. If it's `true`, the loop body executes. If it's `false`, the loop terminates.
-   **Iterator**: Executed *after* each iteration. Typically used to increment or decrement the loop counter.

```csharp
// This loop will print numbers from 0 to 4
for (int i = 0; i < 5; i++)
{
    Console.WriteLine($"Current number is: {i}");
}
```

### The `foreach` loop
The `foreach` loop is the simplest and most common way to iterate over all the elements in a collection, such as an array, a `List`, or a `Dictionary`. It's less error-prone than a `for` loop for this purpose because you don't need to manage an index variable.

```csharp
var names = new List<string> { "Alice", "Bob", "Charlie" };

foreach (var name in names)
{
    Console.WriteLine($"Hello, {name}!");
}
```
**Note on Modifying Collections in `foreach`:**
You've asked an excellent question about what "read-only" means in the context of a `foreach` loop. Let's clarify this, as it's a crucial concept.

The loop variable (`name` in the example above) is indeed read-only. This means you **cannot reassign the variable itself**. For example, this would be illegal:
```csharp
// ILLEGAL: Cannot assign to 'name' because it is a 'foreach iteration variable'
foreach (var name in names)
{
    name = "New Name"; // This will not compile.
}
```

However, this "read-only" behavior has different implications for value types vs. reference types:

-   **Reference Types (`class`):** If your collection contains objects of a reference type, you **CAN** modify the properties of those objects. The loop variable holds a copy of the *reference* to the object in memory, so you are modifying the original object.

    To demonstrate, I have created a [`User.cs`](./Examples/User.cs) class. Let's see this in action:
    ```csharp
    var users = new List<User>
    {
        new User("Alice"),
        new User("Bob")
    };

    // You CAN modify the properties of the user object
    foreach (var user in users)
    {
        user.IsActive = false; 
    }

    // The original objects in the list are now changed.
    Console.WriteLine(users[0].IsActive); // Output: False
    ```

-   **Value Types (`struct`):** If your collection contains value types, the loop variable is a *copy of the actual value*. Modifying it would only change the copy, not the original in the collection. To prevent this confusion, C# makes the members of the copied struct read-only as well.

    ```csharp
    // Assume Point is a struct from Topic 3
    var points = new List<Point> { new Point(1, 2) };
    foreach (var p in points)
    {
        // ILLEGAL: Cannot modify members of 'p' because it is a 'foreach iteration variable'
        // p.X = 100; // This will not compile.
    }
    ```

The most important rule remains: **You cannot add or remove items from the collection you are currently iterating over with a `foreach` loop.** Doing so will throw an `InvalidOperationException`. If you need to do that, use a `for` loop and carefully manage the index.

### The `while` loop
A `while` loop executes a block of code as long as a specified boolean condition remains `true`. The condition is checked *before* each iteration. This is useful when you don't know in advance how many times you need to loop.

```csharp
int count = 0;
while (count < 3)
{
    Console.WriteLine($"Looping with while, count is {count}");
    count++;
}
```
Be careful to ensure that the condition will eventually become `false`; otherwise, you will create an infinite loop.

### The `do-while` loop
The `do-while` loop is similar to the `while` loop, but with one key difference: the condition is checked *after* the loop body executes. This guarantees that the loop body will run at least once.

```csharp
int number;
do
{
    Console.WriteLine("Enter a number greater than 10:");
    // Assume Console.ReadLine() is parsed into the 'number' variable
    number = int.Parse(Console.ReadLine()); 
} while (number <= 10);

Console.WriteLine("Correct, the number is greater than 10.");
```

### Loop Control Statements
You can change the flow of a loop from within its body using control statements.

-   **`break`**: Immediately terminates the innermost loop it's in. Execution continues at the first statement after the loop.

    ```csharp
    for (int i = 0; i < 10; i++)
    {
        if (i == 5)
        {
            break; // The loop stops when i is 5
        }
        Console.WriteLine(i); // Will print 0, 1, 2, 3, 4
    }
    ```

-   **`continue`**: Skips the rest of the current iteration and proceeds to the next one.

    ```csharp
    for (int i = 0; i < 5; i++)
    {
        if (i == 2)
        {
            continue; // Skips the Console.WriteLine when i is 2
        }
        Console.WriteLine(i); // Will print 0, 1, 3, 4
    }
    ```