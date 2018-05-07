# fixed (arrays)

Uno supports fixed arrays, which are arrays with a static size that is known compile-time.

Fixed arrays are primarily used in code intended for GPU execution, as regular (reference typed) arrays cannot be used within shader code. Fixed arrays must be used when passing array uniform data from CPU to GPU shaders.

Fixed array variables are not references, and cannot be used in place of regular arrays. The only things we can do with them are initialization, and then reading/writing the element at a particular index. We can also get the compile-time constant length of a fixed array through the Length property.

Fixed arrays cannot be passed by value or reference, nor can they be boxed. Thus, only local variables, fields and meta properties may be of fixed array type.

## Syntax

Fixed arrays are declared and initialized as follows:

```csharp

// Declare a fixed array field/variable with an explicit size 10
fixed float FixedArrayField[10];

// Declare some fixed array meta properties with an implicit size 3
fixed float FixedArrayMetaProperty[]: fixed float[] { 0, 1, 2 };
fixed float FixedArrayMetaProperty[]: fixed [] { 0.0f, 1.0f, 2.0f };
```

Fixed arrays can be read and written to the regular way:

```csharp
for (var x = 0; x < FixedArrayField.Length; x++)
    FixedArrayField[x] = 1337.0f;

var f = FixedArrayField[3];

```

### Remarks

Fixed arrays are similar to structs in the sense that they are typically stack-allocated, inlined in the containing data structure, or allocated in a static memory region, depending on context. On platforms that don't support this, the compiler will still make the array behave as if it did.

Note that when using fixed arrays for passing uniform data to shaders, there are quite strict limitations on how much uniform data we can pass into a single shader, typically max 4 KB per draw call if targeting WebGL or OpenGL ES 2.0 based platforms. The Uno compiler will tell you when you exceed this limit.