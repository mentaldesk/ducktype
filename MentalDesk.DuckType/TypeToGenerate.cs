namespace MentalDesk.DuckType;

public readonly record struct TypeToGenerate(string classAccessibility, string ClassName, List<string> Members)
{
    public string ClassAccessibility { get; init; } = classAccessibility;
    public readonly string ClassName = ClassName;
    public readonly List<string> Members = Members;
}