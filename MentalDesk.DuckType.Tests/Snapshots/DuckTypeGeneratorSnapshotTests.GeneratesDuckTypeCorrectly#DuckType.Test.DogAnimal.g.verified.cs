//HintName: DuckType.Test.DogAnimal.g.cs

namespace Test
{
    public partial class Test.DogAnimal(Test.Dog instance) : IAnimal
    {
        private readonly Test.Dog _instance = instance;

        public static implicit operator Test.DogAnimal(Test.Dog test.Dog) => new(test.Dog);
        public static implicit operator Dog(Test.DogAnimal test.DogAnimal) => test.DogAnimal._instance;

        public int NumberOfLegs
        {
            get => _instance.NumberOfLegs;
            set => instance.NumberOfLegs = value;
        }
        public string Sound => _instance.Sound;
        public void MakeSound(string sound) => _instance.MakeSound(string sound);
    }
}