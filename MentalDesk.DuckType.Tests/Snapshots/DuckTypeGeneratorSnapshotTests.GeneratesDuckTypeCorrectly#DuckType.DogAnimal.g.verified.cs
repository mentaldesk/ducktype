//HintName: DuckType.DogAnimal.g.cs

namespace MentalDesk.DuckType
{
    public partial class DogAnimal(Dog instance) : IAnimal
    {
        private readonly Dog _instance = instance;

        public static implicit operator DogAnimal(Dog dog) => new(dog);
        public static implicit operator Dog(DogAnimal dogAnimal) => dogAnimal._instance;

        public int NumberOfLegs
        {
            get => _instance.NumberOfLegs;
            set => instance.NumberOfLegs = value;
        }
        public string Sound => _instance.Sound;
        public void MakeSound(string sound) => _instance.MakeSound(string sound);
    }
}