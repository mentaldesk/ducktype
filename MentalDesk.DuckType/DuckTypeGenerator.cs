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

        // Generate source code for each enum found
        context.RegisterSourceOutput(typesToGenerate,
            static (spc, source) => Execute(source, spc));
    } 

    static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is CompilationUnitSyntax cus 
           && cus.ChildNodes().Any(sn => sn is AttributeListSyntax als && als.Parent == cus);
    
    static TypeToGenerate? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        // we know the node is a ClassDeclarationSyntax thanks to IsSyntaxTargetForGeneration
        var cus = (CompilationUnitSyntax)context.Node;

        // loop through all the attributes on the method
        foreach (AttributeListSyntax attributeListSyntax in cus.AttributeLists)
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
                    return GetTypeToGenerate(context, cus, containingAttribute);
                }
            }
        }

        // we didn't find the attribute we were looking for
        return null;
    }     
    
    static TypeToGenerate? GetTypeToGenerate(GeneratorSyntaxContext context, CompilationUnitSyntax classDeclaration, INamedTypeSymbol containingAttribute)
    {
        // Get details of the Class Type parameter
        var classTypeArgument = containingAttribute.TypeArguments[0];
        if (classTypeArgument is not INamedTypeSymbol classSymbol)
        {
            return null;
        }
        var classToWrap = classTypeArgument.MetadataName;
        
        // Get details of the Interface Type parameter
        var interfaceTypeArgument = containingAttribute.TypeArguments[1];
        if (interfaceTypeArgument is not INamedTypeSymbol interfaceSymbol)
        {
            return null;
        }

        // Get the full type name of the partial class we're building
        var nameSpace = classSymbol.ContainingNamespace.IsGlobalNamespace ? string.Empty : classSymbol.ContainingNamespace.ToString();
        var className = $"{ classSymbol.MetadataName }{ interfaceSymbol.MetadataName.WithoutI() }";
        

        // Get all the members in the interface
        var classMembers = interfaceSymbol.GetMembers();
        var memberNames = interfaceSymbol.MemberNames;

        return new TypeToGenerate(
            nameSpace,
            className, 
            classToWrap, 
            interfaceSymbol, 
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