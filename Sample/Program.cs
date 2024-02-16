using MentalDesk.DuckType;
using Sample;

// These would normally go in an AssemblyInfo.cs file but to make the sample
// simpler to read/navigate, we've put them here. They're just marker attributes
// to trigger our source generators. 
[assembly: DuckType<Dog, IAnimal>]
[assembly: DuckType<Duck, IAnimal>]

// Note that neither of these classes implements IAnimal
var dog = new Dog();
var duck = new Duck();

// But we can use the AsIAnimal extension method to treat them as if they
// do because they both have all the methods and properties of IAnimal.
IAnimal[] animals = [dog.AsIAnimal(), duck.AsIAnimal()];

// And now we can 
foreach (var critter in animals)
{
    // Have a look at the type of critter in your debugger... you'll see the
    // generated class that implements IAnimal and wraps the original critter
    // in it's warm blanket of type camouflage.
    critter.MakeSound();
}