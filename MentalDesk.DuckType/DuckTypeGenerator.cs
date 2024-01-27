using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MentalDesk.DuckType;

[Generator]
public class DuckTypeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "DuckTypeAttribute.g.cs", 
            SourceText.From(SourceGenerationHelper.Attribute, Encoding.UTF8)));
        
        // Do a simple filter for enums
        IncrementalValuesProvider<TypeToGenerate?> typesToGenerate = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s), // select classes with attributes
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx)) // select classes with the [DuckTypeAttribute] attribute and extract details
            .Where(static m => m is not null); // Filter out errors that we don't care about

        // If you're targeting the .NET 7 SDK, use this version instead:
        // IncrementalValuesProvider<TypeToGenerate?> typesToGenerate = context.SyntaxProvider
        //     .ForAttributeWithMetadataName(
        //         "MentalDesk.DuckType.DuckTypeAttribute",
        //         predicate: static (s, _) => true,
        //         transform: static (ctx, _) => GetTypeToGenerate(ctx.SemanticModel, ctx.TargetNode))
        //     .Where(static m => m is not null);

        // Generate source code for each enum found
        context.RegisterSourceOutput(typesToGenerate,
            static (spc, source) => Execute(source, spc));
    } 

    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    
    static TypeToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in classDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                INamedTypeSymbol containingAttribute = attributeSymbol.ContainingType;
                string fullName = containingAttribute.OriginalDefinition.ToDisplayString();

                if (fullName == "MentalDesk.DuckType.DuckTypeAttribute<TClass, TInterface>")
                {
                    return GetTypeToGenerate(context, classDeclaration, containingAttribute);
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }     
    
    static TypeToGenerate? GetTypeToGenerate(GeneratorSyntaxContext context, SyntaxNode classDeclaration, INamedTypeSymbol containingAttribute)
    {
        // Get the semantic representation of the class syntax
        SemanticModel semanticModel = context.SemanticModel;
        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong
            return null;
        }

        // Get the full type name of the class
        string? className = classSymbol.ToString();

        // Get all the members in the enum
        ImmutableArray<ISymbol> classMembers = classSymbol.GetMembers();
        var members = new List<string>(classMembers.Length);

        // Get all the fields from the enum, and add their name to the list
        foreach (ISymbol member in classMembers)
        {
            if (member is IFieldSymbol { ConstantValue: not null })
            {
                members.Add(member.Name);
            }
        }

        foreach (var member in classMembers)
        {
            if (member is IFieldSymbol { ConstantValue: not null })
            {
                members.Add(member.Name);
            }
        }

        return new TypeToGenerate(className, members);
    }

    static void Execute(TypeToGenerate? typeToGenerate, SourceProductionContext context)
    {
        if (typeToGenerate is { } value)
        {
            // generate the source code and add it to the output
            string result = SourceGenerationHelper.GenerateExtensionClass(value);
            // Create a separate partial class file for each enum
            context.AddSource($"DuckType.{value.Name}.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }
}