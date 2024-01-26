namespace MentalDesk.DuckType;

public readonly record struct TypeToGenerate(string Name, List<string> Members)
{
    public readonly string Name = Name;
    public readonly List<string> Members = Members;
}