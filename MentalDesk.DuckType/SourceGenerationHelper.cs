using System.Text;

namespace MentalDesk.DuckType;

public static class SourceGenerationHelper
{
    public static string GenerateExtensionClass(TypeToGenerate typeToGenerate)
    {
        var className = typeToGenerate.ClassName;
        var classVariableName = className.ToCamelCase();
        
        var sb = new StringBuilder();
        sb.Append($@"
namespace MentalDesk.DuckType
{{
    { typeToGenerate.ClassAccessibility } partial class { className }(Dog instance) : IAnimal
    {{");
        sb.Append($@"
        private readonly Dog _instance = instance;

        public static implicit operator { className }(Dog dog) => new(dog);
        public static implicit operator Dog({ className } { classVariableName }) => {classVariableName }._instance;

        public int NumberOfLegs => _instance.NumberOfLegs;
        public string Sound => _instance.Sound;
");
    
        sb.Append(@"
    }
}");
        return sb.ToString();
    }
    
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return value;
        }
        return value[..1].ToLower() + value[1..];
    }

    public const string Attribute = @"
namespace MentalDesk.DuckType
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class DuckTypeAttribute<TClass, TInterface> : System.Attribute
    {
    }
}";    
}