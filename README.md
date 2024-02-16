MentalDesk.DuckType
===================

This is a duck typing library for C#. It allows you treat any object as if it implements any interface, so long as it 
has all the members required to do so. 

Whilst dynamic types in C# allow you to do this, they do so at the expense of static typing and so expose you to the 
possibility of runtime errors. 

By contrast, `MentalDesk.DuckType` uses source generators to create adapters around the objects that you want to duck 
type, so any changes to the members of those classes that would break the interface contract will result in a compiler 
error from the generated source code.

## Installation

To get started, add `MentalDesk.DuckType` to your project by running the following command:

```bash
dotnet add package MentalDesk.DuckType
```

## Usage

Imagine you have the following types:

```csharp
class Duck
{
    public void Quack() => Console.WriteLine("Quack!");
}

class Derek
{
    public void Quack() => Console.WriteLine("quack...");
}

public interface IDuck
{
    public void Quack();
}
```

Derek and Duck can both clearly quack, and you want to be able to do something like this:

```csharp   
var duck = new Duck();
var derek = new Derek();
foreach (var d in [duck, derek]) // <-- this wont compile
{
    d.Quack();
}
```

Using `MentalDesk.DuckType` you can add a couple of assembly attributes to your project:

```csharp
[assembly: DuckType<Duck, IDuck>]
[assembly: DuckType<Derek, IDuck>]
```

These `DuckType<TClass, TInterface>` attributes will trigger the duck typing source generators.

And then you can do this:

```csharp
var duck = new Duck();
var derek = new Derek();
IDuck[] ducks = [ duck.AsIDuck(), derek.AsIDuck() ];
foreach (var d in ducks)
{
    d.Quack();
}
```

Behind the scenes, `MentalDesk.DuckType` has generated adapter classes wrapping `Duck` and `Derek` and also created some
`AsIDuck()` extension methods that allow you to easily convert these to `IDuck` instances and leverage any common 
properties or methods that you've defined in the `IDuck` interface. It's statically typed, so it's safe and you don't 
have to write loads of code to make it work.