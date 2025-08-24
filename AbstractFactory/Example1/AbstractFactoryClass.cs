namespace AbstractFactory.Example1;

public abstract class AbstractFactoryClass
{
    public abstract AbstractProductA CreateProductA();
    public abstract AbstractProductB CreateProductB();
}

public class ConcreteFactory1 : AbstractFactoryClass
{
    public override AbstractProductA CreateProductA()
    {
        return new ProductA1();
    }

    public override AbstractProductB CreateProductB()
    {
        return new ProductB1();
    }
}

public class ConcreteFactory2 : AbstractFactoryClass
{
    public override AbstractProductA CreateProductA()
    {
        return new ProductA2();
    }

    public override AbstractProductB CreateProductB()
    {
        return new ProductB2();
    }
}

public abstract class AbstractProductA
{
}

public abstract class AbstractProductB
{
    public abstract void Interact(AbstractProductA a);
}

public class ProductA1 : AbstractProductA
{
}

public class ProductB1 : AbstractProductB
{
    public override void Interact(AbstractProductA a)
    {
        Console.WriteLine(GetType().Name +
                          " interacts with " + a.GetType().Name);
    }
}

public class ProductA2 : AbstractProductA
{
}

public class ProductB2 : AbstractProductB
{
    public override void Interact(AbstractProductA a)
    {
        Console.WriteLine(GetType().Name +
                          " interacts with " + a.GetType().Name);
    }
}

public class Client
{
    private AbstractProductA _abstractProductA;
    private AbstractProductB _abstractProductB;
    // Constructor
    public Client(AbstractFactoryClass factoryClass)
    {
        _abstractProductB = factoryClass.CreateProductB();
        _abstractProductA = factoryClass.CreateProductA();
    }
    public void Run()
    {
        _abstractProductB.Interact(_abstractProductA);
    }
}





