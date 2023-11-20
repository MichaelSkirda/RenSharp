# RenSharp

Programing language for creating _Visual novels_ and _Dialogues_ with [RenPy](https://github.com/renpy/renpy) like syntax.

Supports **_Python_** (_IronPython_) scripting. Compatable with **_Unity_**.

## Get Started (Console)
### Hello world

* Install RenSharp nuget package
* Create .csren file:
```
label start:
    "Hello, world!"
```

* Create instace of RenSharpCore. First arguemnt is relative/absolute path to .csren file. If you're using relative path **DO NOT FORGET** to set option "Copy to outout directory" to "Always copy" or "Copy newer version". If you'd changed file but behavior the same try "Shift + B" or "Build->Rebuild solution".
* Call ```ExecuteNext()```

```csharp
var renSharp = new RenSharpCore("./path/to/file.csren");

while (true)
{
    renSharp.ExecuteNext();
    Console.ReadKey();
}

// Output:
// Hello, world!
// Exception: UnexpectedEndOfProgramException
```

We will use the same C# app that's only uses ReadKey() and ExecuteNext().

### Comments
Comments starts with ```#```. Everything after # will be ignored.

### Say statement


### Variables

```
label start:
    age = 19
    name = "Michael"
    "Hello, my name is {name} and I'm a {age} years old."
    
    # Output:
    # Hello, my name is Michael and I'm a 19 years old.
```



### Cycles


```
label start:
	i = 1
	while(i < 10):
		"Hello {i}"
		i++
	"That's all!"
		
# Output:
# Hello 1
# Hello 2
# ...
# Hello 9
# That's all!
```
## Get Started (Unity)
Comming soon!
