using System.Text;
using Microsoft.CodeAnalysis;

namespace MentalDesk.DuckType;

public static class SourceGenerationHelper
{
    public static string GenerateExtensionClass(TypeToGenerate typeToGenerate)
    {
        var className = typeToGenerate.ClassName;
        var classNameVariable = className.ToCamelCase();
        var classToWrap = typeToGenerate.ClassToWrap;
        var classToWrapVariable = classToWrap.ToCamelCase();
        var interfaceName = typeToGenerate.InterfaceToApply.Name;
        
        var sb = new StringBuilder();
        sb.Append($@"
namespace MentalDesk.DuckType
{{
    { typeToGenerate.ClassAccessibility } partial class { className }({ classToWrap } instance) : { interfaceName }
    {{");
        sb.Append($@"
        private readonly { classToWrap } _instance = instance;

        public static implicit operator { className }({ classToWrap } { classToWrapVariable }) => new({ classToWrapVariable });
        public static implicit operator Dog({ className } { classNameVariable }) => {classNameVariable }._instance;
");

        // Implement all the properties from the interface
        foreach (var memberName in typeToGenerate.MemberNames)
        {
            var classMember = typeToGenerate.ClassMembers.FirstOrDefault(x => x.Name == memberName);
            switch (classMember)
            {
                case IPropertySymbol { GetMethod: not null, SetMethod: not null } property:
                {
                    var propertyAccessibility = property.DeclaredAccessibility.ToString().ToLowerInvariant();
                    var propertyType = property.Type.ToDisplayString(); 
                    var propertyName = property.Name;
                    sb.Append($@"
        { propertyAccessibility } { propertyType } { propertyName }
        {{
            get => _instance.{ propertyName };
            set => instance.{ propertyName } = value;
        }}");
                    break;
                }
                case IPropertySymbol { GetMethod: not null } getOnlyProperty:
                {
                    var propertyAccessibility = getOnlyProperty.DeclaredAccessibility.ToString().ToLowerInvariant();
                    var propertyType = getOnlyProperty.Type.ToDisplayString(); 
                    var propertyName = getOnlyProperty.Name;
                    sb.Append($@"
        { propertyAccessibility } { propertyType } { propertyName } => _instance.{ propertyName };");
                    break;
                }
            }
        }
        
        // public int NumberOfLegs => _instance.NumberOfLegs;
        // public string Sound => _instance.Sound;
    
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