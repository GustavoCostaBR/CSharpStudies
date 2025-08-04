# Topic 6: Arrays and Collections

In C#, an array is a data structure that stores a fixed-size sequential collection of elements of the same type. Collections, on the other hand, are more flexible data structures that can grow or shrink in size.

### Creating and Initializing Arrays

You can declare an array by specifying the element type followed by square brackets `[]`.

#### One-Dimensional Arrays

You can create and initialize a one-dimensional array in several ways.

```csharp
// 1. Declare an array and then initialize it
int[] numbers;
numbers = new int[5]; // Creates an array of 5 integers (initialized to 0)

// 2. Declare, create, and initialize an array with specific values
string[] fruits = new string[] { "Apple", "Banana", "Orange" };

// 3. Shorter syntax for initialization
double[] prices = { 9.99, 15.50, 4.25 };

// Accessing elements
Console.WriteLine(fruits[0]); // Output: Apple

// Modifying elements
prices[2] = 5.00;
```

#### Multi-Dimensional Arrays

Multi-dimensional arrays (or rectangular arrays) are declared by adding commas to the square brackets.

```csharp
// A 2x3 two-dimensional array (2 rows, 3 columns)
int[,] matrix = new int[2, 3];

// Initialize with values
int[,] grid = new int[,] 
{
    { 1, 2, 3 },
    { 4, 5, 6 }
};

// Shorter syntax
int[,] anotherGrid = 
{
    { 1, 2 },
    { 3, 4 },
    { 5, 6 }
};

// Accessing elements (row, column)
Console.WriteLine(grid[0, 1]); // Output: 2
```

### Jagged Arrays

A jagged array is an array of arrays. The inner arrays can have different lengths.

```csharp
// Declare a jagged array of 3 inner arrays
int[][] jaggedArray = new int[3][];

// Initialize the inner arrays
jaggedArray[0] = new int[] { 1, 2 };
jaggedArray[1] = new int[] { 3, 4, 5, 6 };
jaggedArray[2] = new int[] { 7, 8, 9 };

// Accessing elements
Console.WriteLine(jaggedArray[1][2]); // Output: 5
```

### Difference Between Array and Collection

| Feature           | Array                                       | Collection                                  |
| ----------------- | ------------------------------------------- | ------------------------------------------- |
| **Size**          | Fixed size, determined at creation.         | Dynamic size, can grow and shrink at runtime. |
| **Type Safety**   | Strongly typed. All elements must be the same type. | Can be strongly typed (e.g., `List<T>`) or not (e.g., `ArrayList`). |
| **Performance**   | Generally faster for accessing elements by index. | Can have overhead due to dynamic nature.    |
| **Functionality** | Basic operations (get, set, length).        | Rich functionality (add, remove, sort, search, etc.). |
| **Namespace**     | `System`                                    | `System.Collections` or `System.Collections.Generic` |

### Basic Collections: `ArrayList` and `List<T>`

#### `System.Collections.ArrayList`

`ArrayList` is a non-generic collection from the early days of .NET. It can store elements of any type by treating them as `object`. This flexibility comes at the cost of type safety and performance due to boxing/unboxing.

**It is generally recommended to use `List<T>` instead of `ArrayList` in modern C# code.**

```csharp
using System.Collections;

ArrayList myArrayList = new ArrayList();

// Add elements of different types
myArrayList.Add(10);
myArrayList.Add("Hello");
myArrayList.Add(true);

// Accessing requires a cast
int firstNumber = (int)myArrayList[0];
string secondElement = (string)myArrayList[1];

Console.WriteLine(myArrayList.Count); // Output: 3
```

#### `System.Collections.Generic.List<T>`

`List<T>` is the modern, generic, and strongly-typed equivalent of `ArrayList`. It is the most commonly used collection for managing a list of objects.

-   **`T`** is the type parameter, which you replace with the specific type of elements you want to store.
-   Provides compile-time type safety.
-   Avoids performance overhead from boxing/unboxing value types.

```csharp
using System.Collections.Generic;

// Create a list of strings
List<string> names = new List<string>();

// Add elements
names.Add("Alice");
names.Add("Bob");
names.Add("Charlie");
// names.Add(123); // This would cause a compile-time error

// Access elements
Console.WriteLine(names[0]); // Output: Alice

// Common operations
names.Remove("Bob");
Console.WriteLine(names.Count); // Output: 2
bool hasAlice = names.Contains("Alice"); // true

// Iterate through the list
foreach (string name in names)
{
    Console.WriteLine(name);
}
```

### Other Common Collections

#### `Dictionary<TKey, TValue>`

A `Dictionary<TKey, TValue>` stores a collection of key-value pairs. It is highly optimized for looking up a value based on its key.

-   **`TKey`**: The type of the keys. Keys must be unique.
-   **`TValue`**: The type of the values.
-   Provides fast lookups, additions, and removals.

```csharp
// Create a dictionary to store student IDs and names
var studentGrades = new Dictionary<string, double>();

// Add entries
studentGrades.Add("S101", 95.5);
studentGrades["S102"] = 88.0; // Another way to add/update

// Check if a key exists
if (studentGrades.ContainsKey("S101"))
{
    // Retrieve a value by its key
    double grade = studentGrades["S101"];
    Console.WriteLine($"Grade for S101: {grade}"); // Output: 95.5
}

// Iterate through key-value pairs
foreach (KeyValuePair<string, double> entry in studentGrades)
{
    Console.WriteLine($"Student: {entry.Key}, Grade: {entry.Value}");
}

// Just iterate through keys
foreach (string studentId in studentGrades.Keys)
{
    Console.WriteLine(studentId);
}
```

#### `HashSet<T>`

A `HashSet<T>` is a collection that contains no duplicate elements. It is optimized for high-performance set operations like union, intersection, and difference. It is ideal when you need to store a list of unique items and quickly check for their existence.

-   `Add(T item)`: Adds an item to the set. Returns `false` if the item is already present.
-   `Remove(T item)`: Removes an item from the set.
-   `Contains(T item)`: Checks if an item exists in the set.
-   `UnionWith(IEnumerable<T> other)`: Modifies the current set to include all elements present in itself and in the specified collection.
-   `IntersectWith(IEnumerable<T> other)`: Modifies the current set to only include elements that are also in the specified collection.

```csharp
// Create a set of unique tags
var tags = new HashSet<string>();

// Add tags
tags.Add("C#");
tags.Add("Programming");
tags.Add("Development");
tags.Add("C#"); // This will not be added again

Console.WriteLine(string.Join(", ", tags)); // Outputs: C#, Programming, Development

// Check for the existence of a tag
if (tags.Contains("Programming"))
{
    Console.WriteLine("The 'Programming' tag exists.");
}
```

#### `Queue<T>`

The `Queue<T>` collection operates on a first-in, first-out (FIFO) basis. Elements are added to the back of the queue and removed from the front.

- **Enqueue**: Adds an element to the end of the queue.
- **Dequeue**: Removes and returns the element at the beginning of the queue.
- **Peek**: Returns the element at the beginning of the queue without removing it.

```csharp
var queue = new Queue<string>();
queue.Enqueue("First");
queue.Enqueue("Second");
Console.WriteLine(queue.Dequeue()); // Outputs: First
```

#### `Stack<T>`

A `Stack<T>` represents a last-in, first-out (LIFO) collection of objects. It is useful for scenarios like implementing an "undo" feature, where the last action performed is the first one to be undone.

-   `Push(T item)`: Adds an item to the top of the stack.
-   `Pop()`: Removes and returns the item at the top of the stack.
-   `Peek()`: Returns the item at the top of the stack without removing it.

```csharp
// Create a stack to represent a history of actions
var history = new Stack<string>();

// Add actions
history.Push("Action 1");
history.Push("Action 2");
history.Push("Action 3"); // This is now at the top

// "Undo" the actions
while (history.Count > 0)
{
    string lastAction = history.Pop(); // "Undoes" 3, then 2, then 1
    Console.WriteLine($"Undoing: {lastAction}");
}
```

### LINQ (Language-Integrated Query)

LINQ provides a powerful and consistent way to query data from different sources, including collections, databases, and XML. It allows you to write declarative queries directly in C#.

A LINQ query consists of three parts:
1.  **Data Source**: Any object that implements the `IEnumerable<T>` interface (like `List<T>`, `Dictionary<T>`, arrays, etc.).
2.  **Query Creation**: The query expression that defines what to retrieve.
3.  **Query Execution**: The query is executed when you iterate over the query variable (e.g., in a `foreach` loop). This is called **deferred execution**.

#### LINQ Method Syntax (Fluent API)

This is the most common way to write LINQ queries, using a chain of extension methods.

```csharp
var numbers = new List<int> { 5, 10, 3, 8, 15, 1, 12 };

// 1. Find all even numbers
var evenNumbers = numbers.Where(n => n % 2 == 0);

// 2. Order them in ascending order
var orderedEvenNumbers = evenNumbers.OrderBy(n => n);

// 3. Project them into a new form (e.g., a string)
var resultStrings = orderedEvenNumbers.Select(n => $"Number: {n}");

// The query executes here, when we iterate
foreach (var str in resultStrings)
{
    Console.WriteLine(str);
}
// Output:
// Number: 8
// Number: 10
// Number: 12

// You can also chain the methods together:
var evenNumberStrings = numbers
    .Where(n => n % 2 == 0)
    .OrderBy(n => n)
    .Select(n => $"Number: {n}");
```

#### LINQ Query Syntax

This syntax is closer to SQL and can be more readable for complex queries. The compiler translates this syntax into method calls.

```csharp
var numbers = new List<int> { 5, 10, 3, 8, 15, 1, 12 };

// The same query as above, but using query syntax
var evenNumberStringsQuery =
    from n in numbers
    where n % 2 == 0
    orderby n
    select $"Number: {n}";

// The query executes here
foreach (var str in evenNumberStringsQuery)
{
    Console.WriteLine(str);
}
```

#### Common LINQ Methods

-   `Where(predicate)`: Filters a sequence based on a condition.
-   `Select(selector)`: Projects each element of a sequence into a new form.
-   `OrderBy(keySelector)` / `OrderByDescending(keySelector)`: Sorts the elements.
-   `First(predicate)` / `FirstOrDefault(predicate)`: Returns the first element that satisfies a condition. `First` throws an exception if no element is found, while `FirstOrDefault` returns the default value for the type (e.g., `null` for reference types).
-   `Single(predicate)` / `SingleOrDefault(predicate)`: Similar to `First`, but throws an exception if more than one element satisfies the condition.
-   `Any(predicate)`: Checks if any element satisfies a condition.
-   `All(predicate)`: Checks if all elements satisfy a condition.
-   `Count()`: Returns the number of elements in a sequence.
-   `Sum()`, `Average()`, `Min()`, `Max()`: Perform aggregate calculations.
-   `ToList()`, `ToArray()`: Immediately executes the query and returns the results in a new list or array. This is useful to avoid re-executing the query every time you access the results.

### LinkedList<T>

A `LinkedList<T>` is a doubly linked list. Each element (or *node*) in the list contains a value and pointers to the next and previous nodes. This structure makes it very efficient to add or remove elements from anywhere in the list, as it only requires updating the pointers of the adjacent nodes. However, it does not provide direct access by index like a `List<T>` or an array, so retrieving an element at a specific position requires traversing the list from the beginning or end.

**When to use `LinkedList<T>`:**
-   When you need to perform frequent insertions or deletions in the middle of a collection.
-   When you don't need fast random access to elements by index.

Each element in a `LinkedList<T>` is a `LinkedListNode<T>` object.

#### Basic Operations

```csharp
// 1. Create a LinkedList of strings
var shoppingList = new LinkedList<string>();

// 2. Add elements to the end
LinkedListNode<string> milkNode = shoppingList.AddLast("Milk");
shoppingList.AddLast("Bread");

// 3. Add an element to the beginning
shoppingList.AddFirst("Eggs");

// 4. Add an element after a specific node
// shoppingList is now: Eggs, Milk, Bread
shoppingList.AddAfter(milkNode, "Cheese");

// 5. Add an element before a specific node
// shoppingList is now: Eggs, Cheese, Milk, Bread
shoppingList.AddBefore(milkNode, "Yogurt");
// shoppingList is now: Eggs, Yogurt, Cheese, Milk, Bread

// 6. Iterate through the list
Console.WriteLine("Shopping List:");
foreach (string item in shoppingList)
{
    Console.WriteLine(item);
}

// 7. Remove an element
shoppingList.Remove("Bread");

// 8. Find a node
LinkedListNode<string> cheeseNode = shoppingList.Find("Cheese");
if (cheeseNode != null)
{
    Console.WriteLine($"Found: {cheeseNode.Value}");
    Console.WriteLine($"Next item: {cheeseNode.Next?.Value}"); // Milk
    Console.WriteLine($"Previous item: {cheeseNode.Previous?.Value}"); // Yogurt
}
```
