namespace MentalDesk.DuckType;

[AttributeUsage(System.AttributeTargets.Class)]
public class DuckTypeAttribute<TClass, TInterface> : Attribute
{
}
