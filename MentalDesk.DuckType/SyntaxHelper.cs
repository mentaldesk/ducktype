using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MentalDesk.DuckType;

public static class SyntaxHelper
{
    public static string GetNamespace(this ClassDeclarationSyntax classDeclaration)
    {
        var parent = classDeclaration.Parent;
    
        while (parent != null)
        {
            if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
            {
                return namespaceDeclaration.ToFullString();
            }
            parent = parent.Parent;
        }
    
        return "";
    }    
}