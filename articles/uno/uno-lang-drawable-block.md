# drawable block

The drawable block keyword sequence identifies a block that should generate a draw call to the underlying graphics API when encountered in the draw path of a draw statement.

The drawable block keyword sequence can be used within classes or blocks. It can also be used as modifier on a class declaration, which will insert an empty drawable at the beginning of the class.

## Syntax

```csharp
drawable block [<optional name>]  { <optional meta property block> }
```

The optional meta property block can be used to override meta properties that are specific to this drawable.


### Example

```csharp
class Foo
{
    apply DefaultShading;

    drawable block SomeModel1
    {
        apply Model("Model1.DAE");
        Position: float3(10,0,0);   // This property will only apply to the model within this scope (Model1.DAE)
    }

    drawable block SomeModel2
    {
        apply Model("Model2.DAE");
        Position: float3(-10,0,0);  // This property will only apply to the model within this scope (Model2.DAE)
    }

    public void Draw()
    {
        // This will draw both SomeModel1 and SomeModel2 which exist in the draw path of this statement
        // and omit the implicit drawable block that would otherwise be drawn.
        draw;
    }
}
```