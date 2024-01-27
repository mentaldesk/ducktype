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
    public static partial class EnumExtensions
    {");
        sb.Append(@"
            public static string ToStringFast(this ").Append(typeToGenerate.Name).Append(@" value)
                => value switch
                {");
        foreach (var member in typeToGenerate.Members)
        {
            sb.Append(@"
            ").Append(typeToGenerate.Name).Append('.').Append(member)
                .Append(" => nameof(")
                .Append(typeToGenerate.Name).Append('.').Append(member).Append("),");
        }

        sb.Append(@"
                _ => value.ToString(),
            };
");
    
        sb.Append(@"
    }
}");

        return sb.ToString();
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