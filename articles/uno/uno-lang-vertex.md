# vertex

The vertex keyword can be used as an operator in meta properties to force a value into the vertex shader or a later shader stage.

Note that the compiler will move operations into the vertex shader whenever the operation depends on a value derived from a vertex_attrib, so this keyword is rarely neccessary to use explicitly. This keyword is mainly here for symmetry with the pixel keyword.

There are but a few cases where it is desirable to use the vertex keyword:

* If you are CPU bound and performing costy arithmetic operations on the CPU and the number of vertices is fairly small.
* To reduce the amount of vertex data passed to the GPU, in the case where data can be reconstructed by performing calculations on other data.

Note that the vertex keyword does not guarantee that the evaluation will happen in the vertex shader specifically, it can also be moved to a later shader stage, such as the pixel shader, if the expression involves other meta properties that are evaluated at a later stage.

## Example

In the following code, if the matrix u can be evaluated on the cpu, the following meta property will also be evaluated there. This is because the compiler will always prefer to do calculations on the CPU instead of the vertex shader as this will result in a lot of redundant shading operations.

```csharp
float4x4 m: Matrix.Invert(u);
```

If you for some reason want the operation to be performed in the vertex shader, e.g. to ease the job for the CPU, we can use the vertex operator to force the evaluation into the shader.

```csharp
float4x4 m: Matrix.Invert(vertex u);
```