# RenSharp

Programing language for creating _Visual novels_ and _Dialogues_ with [RenPy](https://github.com/renpy/renpy) like syntax.

Supports **_Python_** (_IronPython_) scripting. Compatable with **_Unity_**.

## Get Started (Console)

1. Create .csren file:
```
label start:
    "Hello, world!"
```
2. Install RenSharp nuget package
3. Create RenSharpCore object
```csharp
var renSharp = new RenSharpCore("./path/to/file.csren");

while (true)
{
    Console.ReadKey();
    Command command = renSharp.ReadNext();
}
```

## Get Started (Unity)