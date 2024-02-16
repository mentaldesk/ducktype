//HintName: DuckType.DuckAnimal.g.cs

using Test;

namespace MentalDesk.DuckType
{
    internal class DuckAnimal : IAnimal
    {
        public DuckAnimal(Duck instance)
        {
            _instance = instance;
        }     
        
        private readonly Duck _instance;

        public static implicit operator DuckAnimal(Duck duck) => new(duck);
        public static implicit operator Duck(DuckAnimal duckAnimal) => duckAnimal._instance;

        public int NumberOfLegs
        {
            get => _instance.NumberOfLegs;
            set => _instance.NumberOfLegs = value;
        }
        public string Sound => _instance.Sound;
        public void MakeSound(string sound) => _instance.MakeSound(sound);
    }

    public static class DuckAnimalExtensions
    {
        public static IAnimal AsIAnimal(this Duck x) => new DuckAnimal(x);
    }
}