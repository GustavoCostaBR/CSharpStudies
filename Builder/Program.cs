using Builder;
using Builder.Example2;
using Builder.Example3;
using static System.Console;

// Example1
// var director = new Director();
//
// Builder.Builder b1 = new ConcreteBuilder1();
// Builder.Builder b2 = new ConcreteBuilder2();
//
// // Construct two products
//
// Director.Construct(b1);
// var p1 = b1.GetResult();
// p1.Show();
//
// Director.Construct(b2);
// var p2 = b2.GetResult();
// p2.Show();
//
// // Wait for user
//
// Console.ReadKey();

// Create shop
var shop = new Shop();

// Construct and display vehicles
shop.Construct(new ScooterBuilder());
shop.ShowVehicle();

shop.Construct(new CarBuilder());
shop.ShowVehicle();

shop.Construct(new MotorCycleBuilder());
shop.ShowVehicle();

// Run fluent refactored Example2
FluentVehicleExample2Demo.Run();

// Run Example3 comparison (Builder vs Factory approaches)
Example3Demo.Run();

// Wait for user
ReadKey();