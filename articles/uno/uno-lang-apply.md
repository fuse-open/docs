# apply

The apply keyword inserts all meta properties from a block, class or object into the draw path.

Apply-statements can be used anywhere meta properties are allowed and is equivalent of copying the meta properties by reference from whatever is being applied into the scope.

Apply-statements are the key to creating reusable blocks of effects and shading logic in Uno, as they allow blocks of meta properties to be used and combined in various contexts without copying code.

## Syntax

```csharp
apply blockExpression;
```

Where blockExpression is either:

* A qualified name of a declared block, such as Uno.Scenes.DefaultShading.
* A qualified name of a class.
* An object variable.
* A call to an Uno.Compiler.ImportServices.BlockFactory class constructor, such as Uno.Content.Models.ModelBlockFactory.
* Applying a declared block
* Consider the following block is declared:

```csharp
namespace Foo
{
    block Bar
    {
        public float3 SomeMetaProperty: float3(100, 100, 100);
    }
}
```

Then consider applying this block e.g. inside a class, like this:

```csharp
class Moo
{
    apply Foo.Bar;
}
```

This is equivalent of:

```csharp
class Moo
{
    public float3 SomeMetaProperty: float3(100, 100, 100);
}
```

## Applying block factories

Block factories are special classes that derive from Uno.Compiler.ImportServices.BlockFactory and have names that end in 'BlockFactory'. Block factories provide the syntax tree for a block of meta properties at compile time. A built-in block factory is the Uno.Content.Models.ModelBlockFactory class. This class can be used to import 3D-models as meta properties into the draw path easily.

When you apply a BlockFactory, you must omit the 'BlockFactory' suffix from the name. This means you can use the Uno.Content.Models.ModelBlockFactory class by just typing 'Model', like this:

```csharp
using Uno.Scenes;
using Uno.Content.Models;

class MyModelRenderer
{
    apply DefaultShading;
    apply Model("somemodel.fbx"); // This invokes Uno.Content.Models.ModelBlockFactory at compile time

    ...
}
```

## Applying classes and objects

Applying a class has the same effect as applying a block. Only metaproperties will be inserted into the draw path when applying a class, not other members of the type.

Applying an object has the same effect as applying the type of the object variable, with the object acting as this in that context. Note that it is the compile-time type of the variable that statically determines what metaproperties will be used, even if the actual object at runtime is of a derived type.

The following example shows how a class can be applied and its meta properties manipulated without requiring a specific object instance.

```csharp
class Foo
{
    public int Bar;  // Note: this must be public to be visible to meta properties when the class is applied in other classes
    public int MetaBar: Bar;
}

class Moo
{
    apply Foo;    // Copying meta properties from 'Foo', acting at an for the time being unknown object instance.

    public int DoubleMetaBar: MetaBar * 2; 
}

```

We can apply a non-static class with metaproperties that refer to instance variables, like above. This is very useful as it allows you define metaproperties that work on data in object instances that are not yet known. If you do so, you must however later, for example in a derived class that actually draws something, apply an instance of that class to give the meta properties a context.

```csharp
class Roo: Moo
{
    void Draw(Foo f)
    {
        draw
        {
            apply f; // Overrides all meta properties from 'Foo', now with 'f' as object instance.

            // 'DoubleMetaBar' will now refer to f.Bar * 2 here.
        }
    }
}
```