namespace MentalDesk.DuckType.Tests;

public class DuckTypeGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesDuckTypeCorrectly()
    {
        // The source code to test
        var source = @"
using MentalDesk.DuckType;
using Test;

[assembly: DuckType<Dog, IAnimal>]
[assembly: DuckType<Duck, IAnimal>]

namespace Test;

public interface IAnimal
{
    public int NumberOfLegs { get; set; }
    public string Sound { get; }
    public void MakeSound(string sound);
}

public class Dog
{
    public int NumberOfLegs { get; set; } = 4;
    public string Sound => ""Woof"";
    public void MakeSound(string _) => Console.WriteLine(Sound);
}

public class Duck
{
    public int NumberOfLegs { get; set; } = 2;
    public string Sound => ""Quack"";
    public void MakeSound(string _) => Console.WriteLine(Sound);
}
";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
}