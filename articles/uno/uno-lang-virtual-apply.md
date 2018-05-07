# virtual (apply)

The keyword virtual can be used in draw statements to apply objects (effects) only known at runtime. This is referred to as virtual apply.

The purpose of this feature is to allow code modules written completely independently of each other to still collaborate to construct shaders and data sets for draw statements in combinations controlled dynamically at runtime.

This allows e.g. shading for a material, batching, skinning and environmental properties like lighting and fog, to be coded independently and merged together automatically to perform combined shader effects. This can be done e.g. by creating a base class for each category of effects, a subclass for each actual effect implementation, and apply a reference to the base classes virtually in a draw statement.

Placing virtual in front of references in a draw statement indicates that the compiler should perform a run-time type check of the reference to determine what meta properties to use for the reference. The meta properties for the actual runtime type will be used, instead of the compile-time type.

## Example usage

Consider a rendering engine that wants to support:

* A range of different material/shader types - where new material types should be easy to add later
* Different rendering pipelines for different platforms/performance targets, e.g. a forward renderer and a deferred renderer
* Shadow mapped light sources
* Batching and skinning

To accommodate this, a large number of shaders is needed. With Uno, this can be handled in a clean way very easily thanks to virtual apply. You simply create a base class for each category of effects, and subclasses for each implementation.

To achieve the above, it is logical to separate the shading into three categories of effects:

* A Renderer base class to control the mesh data going in to the draw statement
* A Material base class to control the appearance of objects
* A Pass base class to control the output from the shader. E.g. when rendering to shadow map we want to use a different camera and write out the depth, not color from the pixel shader.
* The class hierarchy could for instance look like this:

virtualapply.png

This design is very similar to the default framework found in Uno.Scenes.

We then choose one of the base classes to be responsible for implementing the actual draw statements, and the other ones will be applied virtually within those draw statements. In this case, we choose the renderer as the actual drawing class.

This means BatchingRenderer and SkinnedMeshRenderer can implement their draw statements something like this:

```csharp
// Note - this class doesn't actually do any batching, 
// this is just for illustrating virtual apply

public class BatchingRenderer: Renderer
{
    public Material Material { get; set; }
    public Mesh Mesh { get; set; }
    ...
    public void Draw(DrawContext dc)
    {
           ...

        draw DefaultShading, Mesh, virtual Material, virtual dc.Pass;
    }
   }
   
```
   
In this case, the compiler will generate code that inspects the types of Material and dc.Pass, and performs the corresponding draw-calls based on the combined meta properties. Note that Mesh is not applied virtually, and hence the meta properties from the compile-time type will always be used, even if Mesh happened to have subclasses.

## Important output size considerations

As there may be multiple sub-classes of both Material and dc.Pass, the compiler might have to generate a large amount of different code paths, only to select a few of them at run time. In the above example, each draw statement in each Renderer will generate 3 x 4 = 12 code paths, potentially with 12 different shaders.

The compiler uses the class hierarchy to determine what combinations to generate. The number of virtual elements in a draw path, as well as the number of subclasses for each virtually applied base class, should therefore be minimized within a project to avoid bloating the output code with a lot of unused shader combinations. Therefore it is also recommended to limit the amount of possible combinations by giving the compiler hints about what type of object will be used in a virtual apply; e.g. if you know the material will always be a DiffuseMapMaterial in a particular case, you should type virtual Material as DiffuseMapMaterial in the above example.