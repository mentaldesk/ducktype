//HintName: DuckType.DogAnimal.g.cs

namespace MentalDesk.DuckType
{
    public partial class DogAnimal(Dog instance) : IAnimal
    {
        private readonly Dog _instance = instance;

        public static implicit operator DogAnimal(Dog dog) => new(dog);
        public static implicit operator Dog(DogAnimal dogAnimal) => dogAnimal._instance;

        public int NumberOfLegs => _instance.NumberOfLegs;
        public string Sound => _instance.Sound;
    }
}