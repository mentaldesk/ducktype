using System.Runtime.CompilerServices;

namespace MentalDesk.DuckType.Tests;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init() =>
        VerifySourceGenerators.Initialize();
}