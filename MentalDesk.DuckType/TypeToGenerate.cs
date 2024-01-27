namespace MentalDesk.DuckType;

public readonly record struct TypeToGenerate(
    string classAccessibility, 
    string ClassName,
    string ClassToWrap, 
    string InterfaceToApply,
    List<string> Members)
{
    public string ClassAccessibility { get; init; } = classAccessibility;
    public string ClassToWrap { get; init; } = ClassToWrap;
    public string InterfaceToApply { get; init; } = InterfaceToApply;
    public readonly string ClassName = ClassName;
    public readonly List<string> Members = Members;
}