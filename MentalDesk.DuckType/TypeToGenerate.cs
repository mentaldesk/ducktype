using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace MentalDesk.DuckType;

public readonly record struct TypeToGenerate(
    string? Namespace,
    string ClassName,
    string ClassToWrap, 
    INamedTypeSymbol InterfaceToApply,
    ImmutableArray<ISymbol> ClassMembers,
    ImmutableArray<string> MemberNames)
{
    
}