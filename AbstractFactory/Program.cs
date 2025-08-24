using AbstractFactory.Example1;
using AbstractFactory.Example2;

// // Example 1:
// AbstractFactoryClass factory1 = new ConcreteFactory1();
// var client1 = new Client(factory1);
// client1.Run();
//
// AbstractFactoryClass factory2 = new ConcreteFactory2();
// var client2 = new Client(factory2);
// client2.Run();
//
// Console.ReadKey();

// // Example 2:
// Create and run the African animal world
var africa = new AnimalWorld<Africa>();
africa.RunFoodChain();

// Create and run the American animal world
var america = new AnimalWorld<America>();
america.RunFoodChain();

// Wait for user input
Console.ReadKey();