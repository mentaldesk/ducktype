// See https://aka.ms/new-console-template for more information

using System.Text.Json.Serialization;
using MentalDesk.DuckType;
using Sample;

var dogType = typeof(Dog);
Console.WriteLine($"{dogType.Name}");

// DogAnimal dog = new Dog();
// var animals = new IAnimal[]{dog};
//
// foreach (var animal in animals)
// {
//     Console.WriteLine($"The {animals.GetType().Name} says {animal.Sound}");
// }
//

namespace Sample
{
    [DuckType<Dog, IAnimal>()]
    public partial class DogAnimal {}

    //
    // public partial class DogAnimal(Dog instance) : IAnimal
    // {
    //     private readonly Dog _instance = instance;
    //
    //     public static implicit operator DogAnimal(Dog dog) => new(dog);
    //     public static implicit operator Dog(DogAnimal dogAnimal) => dogAnimal._instance;
    //
    //     public int NumberOfLegs => _instance.NumberOfLegs;
    //     public string Sound => _instance.Sound;
    // }
    //
    // [System.AttributeUsage(System.AttributeTargets.Class)]
    // public class DuckTypeAttribute(Type ClassType, Type InterfaceType) : System.Attribute
    // {
    // }
}

