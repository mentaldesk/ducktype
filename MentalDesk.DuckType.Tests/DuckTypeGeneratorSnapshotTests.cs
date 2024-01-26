namespace MentalDesk.DuckType.Tests;

public class DuckTypeGeneratorSnapshotTests
{
    [Fact]
    public Task GeneratesDuckTypeCorrectly()
    {
        // The source code to test
        var source = @"
using MentalDesk.DuckType;

namespace DuckTypeGeneratorSnapshotTests
{
    public interface IAnimal
    {
        public int NumberOfLegs { get; }
        public string Sound { get; }
    }

    public class Dog
    {
        public int NumberOfLegs => 4;
        public string Sound => ""Woof"";
    }

    [DuckType<Dog, IAnimal>()]
    public partial class DogAnimal
    {
    }
}";

        // Pass the source code to our helper and snapshot test the output
        return TestHelper.Verify(source);
    }
}