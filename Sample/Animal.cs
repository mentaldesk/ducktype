namespace Sample;

public interface IAnimal
{
    int NumberOfLegs { get; set; }
    string Sound { get; }
    void MakeSound();
}

public abstract class AnimalBase
{
    public abstract string Sound { get;  }
    void MakeSound() => Console.WriteLine(Sound);
}

public class Dog
{
    public int NumberOfLegs { get; set; } = 4;
    public string Sound => "Woof";
    void MakeSound() => Console.WriteLine(Sound);
}

public class Lion : AnimalBase
{
    public int NumberOfLegs { get; set; } = 4;
    public override string Sound => "Roar";
}

public class Duck : AnimalBase
{
    public int NumberOfLegs { get; set; } = 2;
    public override string Sound => "Quack";
}