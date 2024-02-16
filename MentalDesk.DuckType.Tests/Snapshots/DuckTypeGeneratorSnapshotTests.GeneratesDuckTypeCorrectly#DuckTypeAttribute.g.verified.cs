//HintName: DuckTypeAttribute.g.cs

namespace MentalDesk.DuckType
{
    [System.AttributeUsage(System.AttributeTargets.Assembly, AllowMultiple=true)]
    public sealed class DuckTypeAttribute<TClass, TInterface> : System.Attribute
    {
    }
}