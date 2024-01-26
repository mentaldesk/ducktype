namespace Sample;

public interface IAnimal
{
    public int NumberOfLegs { get; }
    public string Sound { get; }
}

public class Dog
{
    public int NumberOfLegs => 4;
    public string Sound => "Woof";
}

public class Lion
{
    public int NumberOfLegs => 4;
    public string Sound => "Roar";
}

public class Duck
{
    public int NumberOfLegs => 2;
    public string Sound => "Quack";
}