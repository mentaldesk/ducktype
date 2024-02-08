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
        if (!string.IsNullOrEmpty(typeToGenerate.Namespace))
        {
            sb.Append($@"
namespace {typeToGenerate.Namespace}
{{");
        }
        sb.Append($@"
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
                case IMethodSymbol method:
                {
                    var methodAccessibility = method.DeclaredAccessibility.ToString().ToLowerInvariant();
                    var returnType = method.ReturnType.ToDisplayString();
                    var methodName = method.Name;
                    var parameters = method.Parameters.Select(x => $"{x.Type.ToDisplayString()} {x.Name}").ToArray();
                    var parameterList = string.Join(", ", parameters);
                    sb.Append($@"
        { methodAccessibility } { returnType } { methodName }({ parameterList }) => _instance.{ methodName }({ parameterList });");
                    break;
                }
            }
        }

        sb.Append(@"
    }");

        if (!string.IsNullOrEmpty(typeToGenerate.Namespace))
        {
            sb.Append(@"
}");
        }

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
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class DuckTypeAttribute<TClass, TInterface> : System.Attribute
    {
    }
}";    
}