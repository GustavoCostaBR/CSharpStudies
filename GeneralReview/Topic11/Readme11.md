# Topic 11: Creating and Destroying Objects

Properly managing the lifecycle of an object—from its creation to its final cleanup—is essential for writing stable and resource-efficient applications. This topic covers constructors for object initialization and the mechanisms in .NET for resource management, including the Garbage Collector and the `IDisposable` pattern.

## Object Constructors and Initialization

A **constructor** is a special method in a class that is executed when a new instance of that class is created. Its primary job is to initialize the object's data fields and put it into a valid, usable state.

### Default and Parameterized Constructors

-   **Default Constructor**: A constructor that takes no arguments. If you don't define any constructors in your class, the C# compiler provides a public, parameterless one for you automatically.
-   **Parameterized Constructor**: A constructor that takes one or more arguments, allowing you to provide initial values for fields when the object is created.

```csharp
public class DatabaseConnection
{
    public string ConnectionString { get; }
    public int Timeout { get; }

    // 1. Parameterized Constructor
    public DatabaseConnection(string connectionString, int timeout)
    {
        Console.WriteLine("Parameterized constructor called.");
        this.ConnectionString = connectionString;
        this.Timeout = timeout;
    }

    // 2. Default Constructor (using constructor chaining)
    public DatabaseConnection() : this("DefaultConnection", 30)
    {
        Console.WriteLine("Default constructor called.");
        // The ': this(...)' syntax calls the other constructor first.
        // This avoids duplicating initialization logic.
    }
}
```

### Object Initializers

Object initializers provide a flexible and readable syntax for creating an object and setting its public properties right after the constructor is called.

```csharp
public class Report
{
    public string Title { get; set; }
    public string Author { get; set; }
    public bool IsPublished { get; set; }
}

// Usage with an object initializer
var report = new Report
{
    Title = "Annual Financials",
    Author = "John Doe",
    IsPublished = false // Properties are set immediately after creation
};
```

## Object Destruction and Garbage Collection

.NET features automatic memory management for **managed resources** (most objects you create in C#). The **Garbage Collector (GC)** is a background process that periodically checks for objects in the heap that are no longer referenced by any part of your application. It then reclaims the memory used by these objects.

### The Problem: Unmanaged Resources

While the GC is great for memory, it knows nothing about **unmanaged resources**. These are things outside the direct control of the .NET runtime, such as:
-   File handles (`FileStream`)
-   Database connections
-   Network sockets
-   Graphics handles (fonts, brushes)

If you don't explicitly release these resources, they can remain locked, leading to memory leaks, performance degradation, and system instability.

## The `IDisposable` Pattern: Deterministic Cleanup

The standard way to handle unmanaged resources in .NET is to implement the `System.IDisposable` interface. This interface defines a single method, `Dispose()`, which is a signal to consumers of your class that it holds resources that need to be cleaned up.

```csharp
public class FileReader : IDisposable
{
    private FileStream _fileStream;
    private bool _disposed = false; // To detect redundant calls

    public FileReader(string filePath)
    {
        // This is an unmanaged resource.
        _fileStream = new FileStream(filePath, FileMode.Open);
    }

    public string ReadLine()
    {
        // ... logic to read from _fileStream ...
        return "line from file";
    }

    // The public Dispose method required by IDisposable
    public void Dispose()
    {
        Dispose(true);
        // Tell the GC not to call the finalizer, since we've already cleaned up.
        GC.SuppressFinalize(this);
    }

    // Protected virtual method for actual cleanup logic
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // Clean up managed resources here (if any)
        }

        // Clean up unmanaged resources here
        if (_fileStream != null)
        {
            _fileStream.Dispose();
            Console.WriteLine("FileStream disposed.");
        }

        _disposed = true;
    }
}
```

### The `using` Statement

Calling `Dispose()` manually is possible, but it's easy to forget, especially if exceptions occur. The `using` statement provides a robust and syntactically clean way to ensure `Dispose()` is always called.

**Classic `using` block:**
```csharp
void ProcessFile(string path)
{
    using (var reader = new FileReader(path))
    {
        // You can use the 'reader' object here.
        // The compiler automatically generates a try/finally block.
        // reader.Dispose() will be called at the end of this block,
        // even if an exception is thrown.
    }
}
```

**`using` declaration (C# 8.0 and later):**
This modern syntax is often preferred as it reduces nesting. The object is disposed at the end of the current scope (usually the end of the method).
```csharp
void ProcessFileModern(string path)
{
    using var reader = new FileReader(path);
    // ... use reader ...
    // reader.Dispose() is called automatically when the method ends.
}
```

## Finalizers (Destructors)

A class can have a **finalizer** (historically called a destructor), which looks like a constructor with a tilde (`~`) prefix.

```csharp
~FileReader()
{
    // This is the finalizer.
    Dispose(false);
}
```

Finalizers are called by the Garbage Collector at some non-deterministic time after an object becomes eligible for collection. **You should almost never implement a finalizer.**

-   **Why Avoid Finalizers?** They are slow, non-deterministic, and add complexity. An object with a finalizer survives an extra GC cycle, which can negatively impact performance.
-   **When to Use Them?** Only as a last-resort safety net to clean up *unmanaged resources* in case a developer forgets to call `Dispose()`. The `IDisposable` pattern shown above includes the correct way to integrate a finalizer.

## Most Useful Takeaways & Best Practices

1.  **`IDisposable` is for Resource Management, Not Memory**: The GC handles memory. `IDisposable` is for everything else that needs to be released, closed, or cleaned up.
2.  **Always Use `using`**: When you encounter a class that implements `IDisposable`, always wrap its usage in a `using` statement or declaration. This is the single most important practice for preventing resource leaks.
3.  **Prefer `class` over `struct` for `IDisposable`**: While structs can implement `IDisposable`, it can lead to confusing behavior because copies of the struct might not be disposed correctly.
4.  **Avoid Finalizers**: Only implement a finalizer if your class *directly* owns an unmanaged resource (e.g., a raw handle from a P/Invoke call), and always implement the full `IDisposable` pattern alongside it.

## Practical Example: Is `IDisposable` Needed for Entity Framework?

**Yes, absolutely.** This is one of the most common and important use cases for the `IDisposable` pattern in modern .NET applications.

The `DbContext` class in Entity Framework (EF) Core is the heart of database communication. It implements `IDisposable` because it manages underlying database connections, which are expensive and limited unmanaged resources.

If you fail to dispose of your `DbContext` instances, the database connections they hold might not be returned to the connection pool promptly. This can lead to **connection pool exhaustion**, where your application can no longer get a connection to the database, causing it to hang or fail entirely.

### Correct Usage with `DbContext`

You should always ensure your `DbContext` is disposed, and the `using` statement is the perfect tool for the job.

```csharp
// Assume you have a DbContext like this:
public class MyDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    // ... configuration ...
}

public class ProductRepository
{
    public Product GetProductById(int productId)
    {
        // The 'using' declaration ensures that context.Dispose() is called
        // automatically when the method exits. This closes the database
        // connection and cleans up resources.
        using var context = new MyDbContext();

        var product = context.Products.Find(productId);

        return product;
    } // <-- context.Dispose() is called here!

    public void AddProduct(Product product)
    {
        // Using the classic block syntax works just as well.
        using (var context = new MyDbContext())
        {
            context.Products.Add(product);
            context.SaveChanges();
        } // <-- context.Dispose() is called here!
    }
}
```

In modern applications using Dependency Injection, the framework often manages the lifetime of the `DbContext` for you (e.g., creating it per-request in a web app). However, the principle remains the same: the framework is responsible for calling `Dispose()` on the context when its scope ends. If you ever create a `DbContext` manually, you are responsible for disposing of it.
