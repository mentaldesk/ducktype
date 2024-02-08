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
        
        // Do a simple filter for classes
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
    
    static TypeToGenerate? GetTypeToGenerate(GeneratorSyntaxContext context, ClassDeclarationSyntax classDeclaration, INamedTypeSymbol containingAttribute)
    {
        // Get the semantic representation of the class syntax
        var semanticModel = context.SemanticModel;
        if (semanticModel.GetDeclaredSymbol(classDeclaration) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong
            return null;
        }

        // Get the full type name of the partial class we're building
        var nameSpace = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToString();
        var classAccessibility = classSymbol.DeclaredAccessibility.ToString().ToLowerInvariant();
        var className = classSymbol.MetadataName;
        
        // Get details of the Class Type parameter
        var classToWrap = containingAttribute.TypeArguments[0].MetadataName;
        
        // Get details of the Interface Type parameter
        var interfaceSymbol = containingAttribute.TypeArguments[1];
        if (interfaceSymbol is not INamedTypeSymbol interfaceToApply)
        {
            return null;
        }

        // Get all the members in the interface
        var classMembers = interfaceToApply.GetMembers();
        var memberNames = interfaceToApply.MemberNames;

        return new TypeToGenerate(
            nameSpace,
            classAccessibility, 
            className, 
            classToWrap, 
            interfaceToApply, 
            classMembers, 
            memberNames.ToImmutableArray()
            );
    }

    static void Execute(TypeToGenerate? typeToGenerate, SourceProductionContext context)
    {
        if (typeToGenerate is { } value)
        {
            // generate the source code and add it to the output
            string result = SourceGenerationHelper.GenerateExtensionClass(value);
            // Create a separate partial class file for each enum
            context.AddSource($"DuckType.{value.ClassName}.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }
}