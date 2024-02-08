//HintName: DuckTypeAttribute.g.cs

namespace MentalDesk.DuckType
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class DuckTypeAttribute<TClass, TInterface> : System.Attribute
    {
    }
}