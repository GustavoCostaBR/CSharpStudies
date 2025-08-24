# An overview of the Microsoft .NET platform

## Introduction
.NET is a free, open-source, cross-platform development platform for building many different kinds of applications. With .NET, you can use multiple languages, editors, and libraries to build for web, mobile, desktop, games, and IoT.
The main programming language for .NET is C#, but it also supports F# and Visual Basic.

## Overview of Microsoft .NET
.NET consists of the runtime and the SDK.
- **Runtime**: Includes the Common Language Runtime (CLR) and the Base Class Library (BCL). It must be installed on the machine where the application will run.
- **SDK (Software Development Kit)**: A set of libraries and tools that developers use to create .NET applications and libraries. It includes the `dotnet` CLI.

## Common Language Runtime
The CLR is the execution engine for .NET applications. It provides services like:
- **Memory Management**: Automatically handles memory allocation and deallocation through a Garbage Collector (GC).
- **JIT (Just-In-Time) Compilation**: Compiles the Intermediate Language (IL) code into machine code that the processor can execute.
- **Exception Handling**: Provides a unified way to handle errors.
- **Security and Type Safety**: Enforces security rules and type correctness.

It's important to note that with .NET 9, performance continues to be a major focus, with improvements in JIT compilation, garbage collection, and native AOT (Ahead-Of-Time) compilation, making applications faster and more efficient.

## Namespaces
Namespaces are used to organize code and to avoid naming conflicts. Think of them as a container for classes and other namespaces. A common convention is to have your namespace match the assembly name and the folder structure of your project.

For example, if you have a project named `MyAwesomeApp` and a class file located at `Models/User.cs`, the namespace for the `User` class would typically be `MyAwesomeApp.Models`.

You declare a namespace like this:
```csharp
// File: Models/User.cs
namespace MyAwesomeApp.Models
{
    public class User
    {
        // User properties and methods
    }
}
```

Starting with C# 10, you can use **file-scoped namespaces** to simplify your code and reduce indentation. This declares that all types within the file belong to the specified namespace, without needing to wrap them in curly braces. Most modern .NET templates use this style by default.

Here is the same example using a file-scoped namespace:
```csharp
// File: Models/User.cs
namespace MyAwesomeApp.Models;

public class User
{
    // User properties and methods
}

// You can add other classes here too; they will all be in the MyAwesomeApp.Models namespace.
public class Product 
{
    // Product properties and methods
}
```

To use a class from another namespace, you can use the `using` directive at the top of your file:
```csharp
// File: Services/UserService.cs
using MyAwesomeApp.Models;

namespace MyAwesomeApp.Services
{
    public class UserService
    {
        public void CreateUser()
        {
            var user = new User(); // No need for MyAwesomeApp.Models.User
        }
    }
}
```

### Implicit Usings

You might have noticed, especially in newer projects, that you don't always need to add `using` statements for very common namespaces like `System` or `System.Collections.Generic`. This is thanks to a feature called **Implicit Usings**.

Starting with .NET 6, projects created with modern SDKs automatically include a set of `global using` directives for namespaces that are common to that project type. These are added by the build process into a generated file in the `obj` folder.

This feature is enabled by default in new projects and is controlled by the `<ImplicitUsings>enable</ImplicitUsings>` tag in your project's `.csproj` file.

Because of implicit usings, you can often write code like this in a new console application without any `using` statements at the top of the file:

```csharp
// No 'using System;' is needed at the top of the file.
Console.WriteLine("Hello, World!");

// No 'using System.Collections.Generic;' is needed.
var numbers = new List<int> { 1, 2, 3 };
```

This helps to reduce boilerplate code and keep your files cleaner.

## Base Class Library (BCL)
The BCL is a massive library of pre-written code that is available to all .NET languages. It provides fundamental types and utility functions and is the foundation upon which higher-level application frameworks are built.

Here are some examples of essential namespaces from the BCL:
- **`System`**: Contains fundamental classes and base types, such as `Object`, `String`, `Int32`, `DateTime`, and `Console`.
- **`System.Collections.Generic`**: Provides generic collection classes like `List<T>` and `Dictionary<TKey, TValue>`, which are workhorses for storing groups of objects.
- **`System.IO`**: Includes types for reading from and writing to files and data streams, like `File` and `StreamReader`.
- **`System.Linq`**: Provides classes and interfaces that support LINQ (Language-Integrated Query), which offers powerful data querying capabilities.
- **`System.Net.Http`**: Contains classes for sending HTTP requests and receiving HTTP responses from a resource identified by a URI, most notably `HttpClient`.

## Common NuGet Packages
While the BCL is extensive, the .NET ecosystem is further expanded by NuGet, the package manager for .NET. It allows developers to create, share, and consume reusable code packages.

Here are a few examples of popular and important NuGet packages:
- **`Newtonsoft.Json` (or `System.Text.Json` which is now built-in)**: The de-facto standard for high-performance JSON manipulation in .NET.
- **`Microsoft.EntityFrameworkCore`**: A modern object-database mapper for .NET. It enables developers to work with a database using .NET objects, eliminating the need for most of the data-access code they usually need to write.
- **`xUnit` / `NUnit` / `MSTest`**: Popular frameworks for writing and running automated tests for your .NET applications.
- **`Serilog` / `NLog`**: Powerful libraries for structured logging, which is crucial for modern application monitoring and diagnostics.
- **`AutoMapper`**: A convention-based object-to-object mapping library that helps in transforming data from one object model to another.

## The .NET CLI (Command-Line Interface)
The .NET CLI is a cross-platform toolchain for developing, building, running, and publishing .NET applications. It's included with the .NET SDK. You can use it for all your development tasks, from creating a new project to deploying it.

Here are some of the most common commands:
- **`dotnet new <TEMPLATE>`**: Creates a new project from a template. For example, `dotnet new console` creates a new console application.
- **`dotnet build`**: Builds a project and all of its dependencies.
- **`dotnet run`**: Runs source code without any explicit compile or launch commands. It's useful for fast iteration.
- **`dotnet test`**: Runs unit tests using a test runner specified in the project.
- **`dotnet publish`**: Publishes the application and its dependencies to a folder for deployment to a hosting system.

## Common .NET Project Types
.NET is a versatile platform that can be used to build a wide variety of applications. Here are a few common project types:
- **Console App**: A command-line application that runs in a terminal. It's often used for tools, utilities, and background services. This is the simplest type of .NET application.
- **ASP.NET Core Web API**: A framework for building HTTP services that can be reached by a broad range of clients, including browsers and mobile devices. It's ideal for creating RESTful services.
- **Blazor Web App**: A modern web framework for building interactive web UIs with C# instead of JavaScript. You can build client-side UIs that run in the browser on WebAssembly, or server-side UIs that handle interactions over a real-time connection.
- **Class Library**: A project that produces a library of classes (a `.dll` file) that can be shared and reused by other .NET applications. It doesn't run on its own but provides functionality to other projects.

## Intermediate Language (IL) and Assemblies
When you compile your C# code, it isn't immediately turned into machine code that a processor can execute. Instead, it's compiled into **Intermediate Language (IL)**. IL is a CPU-independent set of instructions that can be understood by any .NET runtime. This IL code is then packaged into files called **assemblies**.

The CLR's Just-In-Time (JIT) compiler then translates this IL into native machine code at runtime, just before the code is executed. This approach is what makes .NET cross-platform: the same IL can be run on different operating systems and architectures, as long as there's a .NET runtime available to compile it.

Assemblies are the fundamental units of deployment, versioning, and security in .NET. They come in two main forms:
- **`.exe` (Executable)**: An executable assembly. This type of assembly represents a runnable program and must have a main entry point (a `Main` method) where execution begins. When you build a console app, for example, the output is an `.exe`.
- **`.dll` (Dynamic Link Library)**: A library assembly. This type of assembly contains reusable code (like classes and methods) that can be used by other assemblies. It does not have an entry point and cannot be run on its own. Class library projects produce `.dll` files.