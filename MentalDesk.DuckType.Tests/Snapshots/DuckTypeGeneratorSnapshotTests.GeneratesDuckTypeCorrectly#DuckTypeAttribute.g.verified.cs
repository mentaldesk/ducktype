//HintName: DuckTypeAttribute.g.cs

namespace MentalDesk.DuckType
{
    [System.AttributeUsage(System.AttributeTargets.Enum)]
    public class DuckTypeAttribute<TClass, TInterface> : System.Attribute
    {
    }
}