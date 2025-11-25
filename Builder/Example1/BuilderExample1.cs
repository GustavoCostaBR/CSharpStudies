using System;
using System.Collections.Generic;

namespace Builder;

/// <summary>
/// The 'Director' class
/// </summary>
public class Director
{
    // Builder uses a complex series of steps
    public static void Construct(Builder builder)
    {
        builder.BuildPartA();
        builder.BuildPartB();
    }
}

/// <summary>
/// The 'Builder' abstract class
/// </summary>
public abstract class Builder
{
    public abstract void BuildPartA();
    public abstract void BuildPartB();
    public abstract Product GetResult();
}

/// <summary>
/// The 'ConcreteBuilder1' class
/// </summary>
public class ConcreteBuilder1 : Builder
{
    private Product _product = new Product();

    public override void BuildPartA()
    {
        _product.Add("PartA");
    }

    public override void BuildPartB()
    {
        _product.Add("PartB");
    }

    public override Product GetResult()
    {
        return _product;
    }
}

/// <summary>
/// The 'ConcreteBuilder2' class
/// </summary>
public class ConcreteBuilder2 : Builder
{
    private Product _product = new Product();

    public override void BuildPartA()
    {
        _product.Add("PartX");
    }

    public override void BuildPartB()
    {
        _product.Add("PartY");
    }

    public override Product GetResult()
    {
        return _product;
    }
}

/// <summary>
/// The 'Product' class
/// </summary>
public class Product
{
    private List<string> _parts = [];

    public void Add(string part)
    {
        _parts.Add(part);
    }

    public void Show()
    {
        Console.WriteLine("\nProduct Parts -------");
        foreach (var part in _parts)
            Console.WriteLine(part);
    }
}