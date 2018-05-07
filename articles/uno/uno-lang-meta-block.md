# meta_block

The meta block keyword sequence creates a scope for meta properties that will only apply to drawable block objects and other meta blocks within it.

The meta block construct can be useful to group together drawables that share meta properties in a heirarchial manner. For example, meta blocks can be used to express transform heirarchies in a static manner instead of creating an actual object heiarchy at runtime, allowing the compiler to optimize the data set in light of the applied shading.

## Syntax

```csharp
meta block <name>
{
    <contents>
}
```

### Remarks

Meta blocks can be placed within other meta blocks, classes, blocks, and inline blocks.

Meta blocks in a draw path are visited when the compiler looks for drawables while compiling draw statement. This means that meta blocks that contain no drawable block objects have no effect.

When resolving meta properties for a drawable within a meta block, only the meta properties declated before the drawable block within them are honored.

The meta block mechanism is primarily utilized by the Uno.Content.Models.ModelBlockFactory class when importing 3D models at compile-time as blocks, but can also be used manually through this keyword sequence. The difference between importing a model as a block of nodes and as a run-time object hierarchy is that the block approach allows the compiler to perform optimizations on the whole data set at compile time in light of the shading and effects being applied in each draw statement. This results in the most compact and optimal runtime code and dataset to draw the model.

See also

* [draw](uno-lang-draw.md)
* [drawable block](uno-lang-drawable-block.md)