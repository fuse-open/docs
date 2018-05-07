# block

The block keyword declares a block of block items that can be applied or used within other blocks, classes, or in draw statements.

Blocks can be thought of as reusable effects, graphics assets or pieces of shading that can be stacked on top of other blocks to ultimately create draw statements.

As opposed to a class, blocks can not hold fields or other state, and no members except block items.

Blocks can not extend classes or blocks, nor implement interfaces. Instead, blocks may use meta property signatures from other blocks through their using-list.

## Syntax

```
[modifiers] block block_name[: using_list]
{
    [block_items]
}
```

Where the using_list is a comma-separated list of other blocks which the block will borrow meta property signatures from. Note that the block only borrows signatures (names and types), and not definitions, from its using-list. This is useful to e.g. create blocks that work on the terminology from a known standard shading library (e.g. @Uno.Scenes.DefaultShading).

The block_items are any of the following:

* Meta properties
* apply directives
* meta block objects
* drawable block objects

## Example

```csharp
// Decalares a new block that borrows meta property signatures from DefaultShading
public block Scaling: DefaultShading
{
    // Applies another block
    apply Bar;

    // Defines a public meta property that can be overriden later
    public float ScalingFactor: 1.0f;

    // Multiplies the VertexPosition from DefaultShading by the ScalingFactor
    VertexPosition: prev * ScalingFactor;

    // Creates a meta block
    meta block Moo
    {
        // Defines that drawables inside this meta block should use the origiunal VertexPosition
        VertexPosition: prev prev VertexPosition;

        // Draws the original using green color
        drawable block Green
        {
            PixelColor: float4(0,1,0,1);
        }
        // Draws the original using green color, with offset position
        drawable block Blue
        {
            Position: float3(100, 0, 0);
            PixelColor: float4(0,1,0,1);
        }
    }
}
```