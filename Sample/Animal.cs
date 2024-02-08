namespace Sample;

public interface IAnimal
{
    int NumberOfLegs { get; set; }
    string Sound { get; }
    void MakeSound(string sound);
}

public abstract class AnimalBase
{
    public abstract string Sound { get;  }
    public void MakeSound(string sound) => Console.WriteLine(sound);
}

public class Dog
{
    public int NumberOfLegs { get; set; } = 4;
    public string Sound => "Woof";
    public void MakeSound(string _) => Console.WriteLine(Sound);
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