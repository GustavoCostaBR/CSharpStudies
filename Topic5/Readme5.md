# Topic 5: Methods and Parameters

Methods are the building blocks of a C# program. They are reusable blocks of code that perform a specific task. By encapsulating logic within methods, you can make your programs more organized, readable, and easier to maintain. This practice is a cornerstone of the **Don't Repeat Yourself (DRY)** principle.

## What is a Method?

A method is a named sequence of statements that can be executed by calling its name. Think of it as a sub-program within your main program.

-   **Organization**: Methods help break down large, complex problems into smaller, manageable pieces.
-   **Reusability**: Once a method is written, it can be called multiple times from different parts of your program, avoiding code duplication.

## Anatomy of a Method Signature

Let's break down the components of a method definition, also known as its **signature**.

```csharp
public static int Add(int firstNumber, int secondNumber)
{
    // Method body
    int sum = firstNumber + secondNumber;
    return sum;
}
```

1.  **Access Modifier (`public`)**: Determines the visibility of the method.
    -   `public`: Accessible from any code.
    -   `private`: Accessible only from within the same class.
    -   `protected`: Accessible within the same class and by derived classes.
    -   `internal`: Accessible only within the same project (assembly).
    -   (We will explore these in more detail when we cover Object-Oriented Programming).

2.  **Optional Modifiers (`static`)**:
    -   `static`: The method belongs to the class itself, not to a specific instance of the class. You call it using the class name (e.g., `Math.Max()`).
    -   Other modifiers include `async`, `unsafe`, etc.

3.  **Return Type (`int`)**: The type of the value that the method sends back to the caller. If the method does not return a value, the return type is `void`.

4.  **Method Name (`Add`)**: A descriptive name that follows C# naming conventions (PascalCase).

5.  **Parameters (`(int firstNumber, int secondNumber)`)**: A comma-separated list of variables that the method receives as input. Each parameter has a type and a name. This list is enclosed in parentheses. If a method takes no parameters, you still need empty parentheses `()`.

6.  **Method Body (`{ ... }`)**: The block of code, enclosed in curly braces, that contains the statements to be executed when the method is called.

## Defining and Calling Methods

Here's a simple example of defining and then calling a method.

```csharp
// Define a method that takes no parameters and returns nothing.
void SayHello()
{
    Console.WriteLine("Hello, World!");
}

// Call the method to execute its code.
SayHello(); // Output: Hello, World!

// Define the Add method from our example above.
int Add(int a, int b)
{
    return a + b;
}

// Call the method and store its return value in a variable.
int result = Add(5, 3);
Console.WriteLine(result); // Output: 8
```

## Passing Parameters

How a method handles the data you pass to it depends on whether the parameter is a **value type** or a **reference type**.

### Passing Value Types
When you pass a value type (like `int`, `double`, `bool`, or any `struct`) to a method, a **copy** of the variable is created. The method works with this copy. Any changes made to the parameter inside the method **do not affect** the original variable in the calling code. This is known as **pass-by-value**.

```csharp
void Square(int number)
{
    number = number * number; // This modifies the copy
    Console.WriteLine($"Inside method: {number}");
}

int originalNumber = 5;
Square(originalNumber); // Pass the value to the method
Console.WriteLine($"Outside method: {originalNumber}"); // The original value is unchanged

// Output:
// Inside method: 25
// Outside method: 5 
```

### Passing Reference Types
When you pass a reference type (like a `class` instance, `string`, or array) to a method, a **copy of the reference** is passed. Both the original variable and the method parameter now point to the **same object** on the heap.

This means if you modify the *object* that the parameter refers to (e.g., by changing one of its properties), the changes **will be visible** outside the method.

```csharp
// Using the User class from Topic 4
public class User { public string Name { get; set; } }

void DeactivateUser(User user)
{
    // This changes the Name property of the object pointed to by 'user'
    user.Name = user.Name + " (Deactivated)";
}

var myUser = new User { Name = "Alice" };
DeactivateUser(myUser); // Pass the reference to the method
Console.WriteLine(myUser.Name); // The original object has been modified

// Output:
// Alice (Deactivated)
```

However, if you reassign the parameter to a *new object* inside the method, this change will **not** affect the original variable. You are only changing where the method's copy of the reference points.

```csharp
void TryToReplaceUser(User user)
{
    // This only changes the local 'user' parameter to point to a new object.
    // The original variable 'myUser' is unaffected.
    user = new User { Name = "Bob" }; 
}

var myUser = new User { Name = "Alice" };
TryToReplaceUser(myUser);
Console.WriteLine(myUser.Name); // Still "Alice"

// Output:
// Alice
```

## Parameter Modifiers

C# provides special keywords to change the default parameter-passing behavior. These give you more control over how methods interact with your data.

### The `ref` Modifier
The `ref` keyword causes a parameter to be passed by **reference**, not by value. This works for both value types and reference types. Instead of passing a copy, the method gets a direct reference to the original variable.

-   Any changes made to the parameter inside the method **will affect** the original variable.
-   The variable passed as a `ref` argument **must be initialized** before it is passed to the method.

```csharp
void SquareByRef(ref int number)
{
    number = number * number; // This now modifies the original variable
}

int valueToSquare = 5;
SquareByRef(ref valueToSquare); // Pass the variable by reference
Console.WriteLine(valueToSquare); // The original value is now changed

// Output:
// 25
```

### The `out` Modifier
The `out` keyword is also used to pass a parameter by reference. It's very similar to `ref`, but with a different intent. `out` is used when you want a method to **return more than one value**.

-   Any changes made to the parameter inside the method **will affect** the original variable.
-   The variable passed as an `out` argument **does not have to be initialized** before being passed.
-   The method **must assign a value** to the `out` parameter before it returns.

This is commonly used in methods like `int.TryParse`, which returns a `bool` to indicate success and the parsed value through an `out` parameter.

```csharp
void GetDimensions(double area, out double width, out double height)
{
    // The method must assign values to the out parameters.
    width = Math.Sqrt(area);
    height = width;
}

double myWidth, myHeight; // No need to initialize
GetDimensions(100, out myWidth, out myHeight);

Console.WriteLine($"Width: {myWidth}, Height: {myHeight}");

// Output:
// Width: 10, Height: 10
```
You can also declare the `out` variable directly in the method call for more concise syntax:
```csharp
GetDimensions(144, out double w, out double h);
Console.WriteLine($"Width: {w}, Height: {h}"); // Output: Width: 12, Height: 12
```

### The `in` Modifier
The `in` keyword, introduced in C# 7.2, is also used to pass a parameter by reference, but it ensures that the argument **cannot be modified** by the method.

This is primarily a performance optimization for passing large `struct` types. By passing by reference, you avoid the cost of copying the large struct, and by making it `in`, you get a compile-time guarantee that the method won't change your original data.

-   The method cannot reassign the parameter or any of its fields (if it's a `struct`).
-   It tells the reader of the code that the method only inspects the data, it doesn't mutate it.

```csharp
// Assume Point is a large struct
struct Point3D { public double X, Y, Z; /* ... other data ... */ }

void PrintPoint(in Point3D point)
{
    // point.X = 10; // This would be a compile-time error.
    Console.WriteLine($"({point.X}, {point.Y}, {point.Z})");
}

var myPoint = new Point3D { X = 10, Y = 20, Z = 30 };
PrintPoint(in myPoint); // Pass by reference to avoid a copy, with a guarantee of no modification.
```

## Advanced Parameter Features

C# offers several features to make method calls more flexible and readable.

### Optional Parameters and Named Arguments

You can give a parameter a **default value**, making it optional for the caller. Optional parameters must come after all required parameters in the method signature.

When calling a method with multiple optional parameters, you can use **named arguments** to specify which values you are providing, improving readability and allowing you to skip other optional parameters.

```csharp
// 'isActive' and 'role' are optional parameters.
void CreateUser(string username, bool isActive = true, string role = "Guest")
{
    Console.WriteLine($"Creating user {username}. Active: {isActive}. Role: {role}.");
}

// Calling the method in different ways:
CreateUser("Alice"); // Uses both default values.
CreateUser("Bob", false); // Overrides 'isActive', uses default for 'role'.
CreateUser("Charlie", false, "Admin"); // Overrides both.

// Using named arguments to improve clarity and skip an optional parameter.
CreateUser(username: "David", role: "Moderator"); // Skips 'isActive', which remains true.

// Output:
// Creating user Alice. Active: True. Role: Guest.
// Creating user Bob. Active: False. Role: Guest.
// Creating user Charlie. Active: False. Role: Admin.
// Creating user David. Active: True. Role: Moderator.
```

### The `params` Keyword
The `params` keyword allows a method to accept a **variable number of arguments** of the same type. The arguments are passed as a comma-separated list and are available inside the method as an array.

-   A method can only have one `params` parameter.
-   It must be the last parameter in the method signature.

```csharp
// This method can accept any number of integers.
double CalculateAverage(params int[] numbers)
{
    if (numbers == null || numbers.Length == 0)
    {
        return 0;
    }

    int sum = 0;
    foreach (int number in numbers)
    {
        sum += number;
    }
    
    return (double)sum / numbers.Length;
}

// Calling the method with different numbers of arguments.
double avg1 = CalculateAverage(2, 4, 6); // Pass arguments directly.
double avg2 = CalculateAverage(5, 10, 15, 20, 25);
double avg3 = CalculateAverage(); // You can even pass none.

Console.WriteLine(avg1); // Output: 4
Console.WriteLine(avg2); // Output: 15
Console.WriteLine(avg3); // Output: 0
```

## Method Overloading

Method overloading is the ability to define multiple methods in the same class with the **same name**, as long as their **signatures are different**. The signature difference can be in:
-   The number of parameters.
-   The type of parameters.
-   The order of parameters.
-   The use of parameter modifiers (`ref`, `out`, `in`)

The return type alone is **not** enough to differentiate two methods.

Overloading is useful when you want to provide different ways to perform the same logical operation.

```csharp
// A class with overloaded 'Process' methods.
public class DataProcessor
{
    // 1. Process an integer
    public void Process(int data)
    {
        Console.WriteLine($"Processing integer: {data}");
    }

    // 2. Process a string
    public void Process(string data)
    {
        Console.WriteLine($"Processing string: {data}");
    }

    // 3. Process an integer and a boolean flag
    public void Process(int data, bool force)
    {
        Console.WriteLine($"Processing integer: {data} with force flag: {force}");
    }
}

var processor = new DataProcessor();
processor.Process(123);       // Calls version 1
processor.Process("hello");   // Calls version 2
processor.Process(456, true); // Calls version 3
```

## Expression-Bodied Methods

For methods that contain only a single statement or expression, you can use a more concise syntax called an **expression-bodied method**. It uses the `=>` (lambda arrow) to define the method body.

This is purely syntactic sugar and makes simple methods much cleaner.

```csharp
// Standard method syntax
public int Add(int a, int b)
{
    return a + b;
}

// Equivalent expression-bodied syntax
public int AddConcise(int a, int b) => a + b;

// Also works for void methods
public void PrintMessage(string message) => Console.WriteLine(message);
```

## Local Functions

A local function is a method that is defined **inside another method**. This is useful for creating helper functions that are only needed by the containing method.

-   They can only be called from the method they are defined in.
-   They can access variables (including parameters) of the containing method.
-   This helps to keep your class clean from small, single-purpose helper methods that aren't used anywhere else.

```csharp
void ProcessOrders(IEnumerable<Order> orders)
{
    foreach (var order in orders)
    {
        // Call the local function
        double finalPrice = CalculateFinalPrice(order.Price, order.TaxRate);
        Console.WriteLine($"Order ID {order.Id} has final price {finalPrice:C}");
    }

    // Define a local function inside ProcessOrders
    // It can access the 'orders' parameter, but it doesn't need to here.
    double CalculateFinalPrice(decimal price, decimal taxRate)
    {
        var tax = price * taxRate;
        return (double)(price + tax);
    }
}

// Assume Order class exists
public class Order { public int Id; public decimal Price; public decimal TaxRate; }
```

## Best Practices: Returning Values vs. Modifying Parameters

You've asked an excellent question: even if you *can* modify an object passed by reference, *should* you?

As a general principle, **prefer returning a new or modified object instead of modifying the input object directly (mutation)**. This practice leads to cleaner, more predictable code.

### Why Prefer Returning Values?

1.  **Clarity and Readability**: The change is explicit in the calling code. There are no hidden side effects.
    ```csharp
    // Very clear: a new object is created with the changes.
    var updatedUser = user.Deactivate(); 
    ```

2.  **Reduces Side Effects**: Code that doesn't modify external state is easier to test and reason about. You don't have to track which methods might be secretly changing your objects. This is a core principle of functional programming.

3.  **Enables Method Chaining**: If methods return a modified instance, they can be chained together fluently.
    ```csharp
    // This pattern is only possible if each method returns a new User instance.
    var admin = user.WithRole("Admin").WithStatus("Active");
    ```

### When is it Better to Modify Parameters?

While returning values is the default best practice, there are important exceptions:

1.  **The "Try-Get" Pattern with `out`**: This is the standard C# idiom for returning a success flag and a value. It's efficient and universally understood.
    ```csharp
    if (int.TryParse(text, out int result))
    {
        // ... use result
    }
    ```

2.  **High-Performance Scenarios with `ref` and `in`**: When working with very large `structs`, passing by reference with `ref` or `in` avoids expensive memory copies. The performance gain can be significant and justifies the trade-off.

3.  **Clear Intent to Mutate**: Sometimes a method's entire purpose is to modify an object's state, and the method name makes this obvious. `List<T>.Sort()` is a perfect example; it is expected to sort the list in-place.

**Conclusion**: Start by designing your methods to be pure functions that return new values. Only reach for `ref`, `out`, or in-place mutation when you have a specific, intentional reason like performance or a well-established C# idiom.

### `in` and `ref` and Memory Allocation (Stack vs. Heap)

A common question is whether using `in` or `ref` affects where a `struct` is stored.

The short answer is: **`in` and `ref` do not change where the struct is allocated.** They only change *how* the struct is passed to the method. They pass a direct pointer (a reference) to the struct's existing memory location, wherever that may be.

The real question is, "Where was the struct allocated *before* it was passed to the method?"

Here are the rules:

1.  **Allocated on the Stack**: If the struct is a local variable inside a method, it lives on the stack. Passing it with `in` or `ref` gives the method a pointer to that stack location.

    ```csharp
    void MyMethod()
    {
        Point3D myPoint = new Point3D { X = 10, Y = 20, Z = 30 }; // myPoint is on the stack
        
        // The method gets a pointer to myPoint's location on the stack.
        // No new memory is allocated on the heap.
        PrintPoint(in myPoint); 
    }
    ```

2.  **Allocated on the Heap**: If the struct is a field of a `class`, or part of an array, it lives on the heap along with the containing object or array. Passing it with `in` or `ref` gives the method a pointer to that heap location.

    ```csharp
    public class MyObject
    {
        // This Point3D struct will be part of MyObject's data on the heap.
        public Point3D Location { get; set; }
    }

    void MyOtherMethod()
    {
        var myObject = new MyObject(); // myObject is on the heap.
                                       // myObject.Location is also on the heap.

        // The method gets a pointer to the struct's location within the heap.
        PrintPoint(in myObject.Location);

        // Similarly, for an array:
        var points = new Point3D[10]; // The array and all its structs are on the heap.
        
        // The method gets a pointer to the first struct's location within the heap.
        PrintPoint(in points[0]);
    }
    ```

**In summary:** `in` and `ref` are about avoiding a *copy* of the data. They are not about changing the data's home from the stack to the heap or vice-versa. The struct stays exactly where it was, and the method simply gets a direct address to it.
