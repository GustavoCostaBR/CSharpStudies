# Topic 7: Error Handling and Debugging

This section covers handling runtime errors using exceptions and debugging applications to find and fix issues, with a focus on tools available in JetBrains Rider.

## Exception Handling

In C#, runtime errors are propagated through the program using a mechanism called exceptions. When an error occurs, an exception is "thrown". You can write code to "catch" and handle these exceptions.

### `try-catch` block

The `try-catch` statement consists of a `try` block followed by one or more `catch` clauses, which specify handlers for different exceptions.

```csharp
try
{
    // Code that may throw an exception
    int[] numbers = { 1, 2, 3 };
    Console.WriteLine(numbers[5]); // This will throw an IndexOutOfRangeException
}
catch (IndexOutOfRangeException ex)
{
    // Handle the specific exception
    Console.WriteLine($"Error: The index is out of the array's bounds. {ex.Message}");
}
catch (Exception ex)
{
    // Handle any other exceptions
    Console.WriteLine($"An unexpected error occurred: {ex.Message}");
}
```

### `finally` block

The `finally` block is used to execute a given set of statements, whether an exception is thrown or not. It's often used for cleanup code, like closing files or database connections.

```csharp
System.IO.StreamReader file = null;
try
{
    file = new System.IO.StreamReader("c:\\nonexistentfile.txt");
    // ... process file
}
catch (System.IO.FileNotFoundException ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    if (file != null)
    {
        file.Close();
    }
    Console.WriteLine("Finally block executed.");
}
```

### `throw` statement

You can explicitly throw an exception using the `throw` keyword. You can either re-throw a caught exception or throw a new instance of an exception class.

```csharp
public void ProcessValue(int value)
{
    if (value < 0)
    {
        throw new ArgumentOutOfRangeException(nameof(value), "Value cannot be negative.");
    }
    // ... process value
}
```

## Debugging in JetBrains Rider

JetBrains Rider provides a powerful suite of tools for debugging your .NET applications.

### Breakpoints

Breakpoints pause the execution of your program at a specific line of code. In Rider, you can set a breakpoint by clicking in the left gutter next to a line of code.

-   **Conditional Breakpoints**: You can configure a breakpoint to only trigger when a certain condition is met. Right-click the breakpoint icon and enter a boolean expression.
-   **Action Breakpoints**: Instead of pausing, a breakpoint can perform an action, like logging a message to the console.

### Stepping Through Code

Once execution is paused at a breakpoint, you can control the flow step-by-step:

-   **Step Over (F10)**: Executes the current line and moves to the next line in the same method.
-   **Step Into (F11)**: If the current line contains a method call, it moves the execution to the first line of that method.
-   **Step Out (Shift+F11)**: Continues execution until the current method returns, then pauses at the line where the method was called.
-   **Run to Cursor (Ctrl+F10)**: Resumes execution and pauses at the line where your cursor is.

### Inspecting Data

While debugging, you can inspect the state of your application:

-   **Variables Window**: Shows the values of local variables and parameters in the current scope.
-   **Watch Window**: Allows you to monitor specific variables or expressions of your choice throughout the debugging session.
-   **DataTips**: Hovering over a variable in the editor will show its current value in a tooltip.

### Advanced Tools

-   **Immediate Window**: Allows you to execute expressions and code statements at runtime while paused, which is useful for testing changes or inspecting complex objects.
-   **Call Stack Window**: Shows the sequence of method calls that led to the current execution point. You can navigate through the stack to see the state of the application at different levels.
-   **Exception Breakpoints**: You can configure the debugger to break automatically whenever an exception (or a specific type of exception) is thrown, even if it's inside a `try-catch` block. This is accessible via `Run | Stop on Exception...`.

## `finally` vs. Code After `catch`

A common point of confusion is the difference between putting cleanup code in a `finally` block versus placing it immediately after the `try-catch` block. The key difference is **guaranteed execution**.

A `finally` block is **guaranteed** to execute, regardless of what happens inside the `try` or `catch` blocks. This includes scenarios like:

1.  **An uncaught exception:** If an exception occurs in the `try` block that is not handled by any `catch` block, `finally` will still run before the program terminates.
2.  **A `return` statement:** If the `try` or `catch` block executes a `return` statement, the `finally` block will execute *before* the method returns its value.
3.  **An exception in a `catch` block:** If a new exception is thrown from within a `catch` block, the `finally` block is still executed.

Code placed after the `try-catch` block is **not** guaranteed to run in these situations.

### Code Example

This example demonstrates the execution flow with and without exceptions.

```csharp
public class FinallyExample
{
    public static void TestFinally(bool throwException)
    {
        try
        {
            Console.WriteLine("1. Inside try block.");
            if (throwException)
            {
                Console.WriteLine("2. Throwing exception now.");
                throw new Exception("This is a test exception.");
            }
            // If an exception is thrown, the next line is skipped.
            Console.WriteLine("3. End of try block."); 
            return; // Let's see what happens with a return statement
        }
        catch (Exception ex)
        {
            Console.WriteLine($"4. Inside catch block: {ex.Message}");
        }
        finally
        {
            // This block is always executed.
            Console.WriteLine("5. Inside finally block. Cleanup happens here.");
        }

        // This code is only reached if no exception was thrown OR if it was caught
        // AND there was no 'return' statement in the try/catch blocks.
        Console.WriteLine("6. Code after the try-catch-finally block.");
    }

    public static void Main()
    {
        Console.WriteLine("--- Running test WITHOUT exception ---");
        TestFinally(false);

        Console.WriteLine("\n--- Running test WITH exception ---");
        TestFinally(true);
    }
}
```

### Execution Output

**Scenario 1: No Exception (`TestFinally(false)`)**

```
--- Running test WITHOUT exception ---
1. Inside try block.
3. End of try block.
5. Inside finally block. Cleanup happens here.
// Notice that "6. Code after..." is NOT printed because of the 'return' in the 'try' block.
```

**Scenario 2: With Exception (`TestFinally(true)`)**

```
--- Running test WITH exception ---
1. Inside try block.
2. Throwing exception now.
4. Inside catch block: This is a test exception.
5. Inside finally block. Cleanup happens here.
6. Code after the try-catch-finally block.
```

### Summary

| Code Location | When does it execute? |
| :--- | :--- |
| **`finally` block** | **Always.** It's the last thing to run before control leaves the `try-catch` structure, even with `return` statements or uncaught exceptions. |
| **After `try-catch`** | Only if control flows out of the bottom of the entire `try-catch-finally` structure without hitting a `return` or an unhandled exception. |

**Conclusion:** Use the `finally` block for critical cleanup code, such as closing database connections, releasing file handles, or disposing of other unmanaged resources, because it's the only way to guarantee that the cleanup code will run.
