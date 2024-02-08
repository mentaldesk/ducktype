//HintName: DuckType.DogAnimal.g.cs

namespace Test
{
    public partial class DogAnimal : IAnimal
    {
        public DogAnimal(Dog instance)
        {
            _instance = instance;
        }     
        
        private readonly Dog _instance;

        public static implicit operator DogAnimal(Dog dog) => new(dog);
        public static implicit operator Dog(DogAnimal dogAnimal) => dogAnimal._instance;

        public int NumberOfLegs
        {
            get => _instance.NumberOfLegs;
            set => _instance.NumberOfLegs = value;
        }
        public string Sound => _instance.Sound;
        public void MakeSound(string sound) => _instance.MakeSound(sound);
    }
}