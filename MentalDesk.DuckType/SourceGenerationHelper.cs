using System.Text;

namespace MentalDesk.DuckType;

public static class SourceGenerationHelper
{
    public static string GenerateExtensionClass(TypeToGenerate typeToGenerate)
    {
        var sb = new StringBuilder();
        sb.Append(@"
namespace MentalDesk.DuckType
{
    public partial class DogAnimal(Dog instance) : IAnimal
    {");
        sb.Append(@"
        private readonly Dog _instance = instance;

        public static implicit operator DogAnimal(Dog dog) => new(dog);
        public static implicit operator Dog(DogAnimal dogAnimal) => dogAnimal._instance;

        public int NumberOfLegs => _instance.NumberOfLegs;
        public string Sound => _instance.Sound;
");
    
        sb.Append(@"
    }
}");

        return sb.ToString();
    }        
}