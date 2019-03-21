# vertex_attrib

The vertex_attrib keyword is used as an operator in meta properties to fetch an element from an array or vertex buffer into the vertex shader. A meta property that uses the vertex_attrib keyword will always be evaluated in the vertex shader stage or later.

There are two main methods for using this operator:

* Fetching directly from arrays
* Fetching from manually managed vertex and index buffers

These in turn can be used in indexed and non-indexed mode.

## Fetching directly from arrays

The vertex_attrib operator can be used to fetch data directly from arrays, conveniently hiding the complexity of manual management of GPU buffers. This method is appropriate in contexts where the vertex and index data is static, or changes completely every time. In cases where data changes more randomly, using manually managed vertex buffers and index buffers can improve performance (see below).

The vertex_attrib operator takes a vertex array, and optionally an index array, as arguments, and returns a single element from the vertex array. Subsequent operations on the returned value happens in parallell for all vertices, when the meta property is involved in a draw operation. When an index array is used, the indices in the index array is used to fetch vertices from the vertex array, allowing re-use of vertices.

### Non-indexed fetch

This method will convert the array provided to a vertex buffer, on initialization if possible, and fetch an element from that buffer using the array's data type. The syntax for this method is:

```csharp
vertex_attrib(vertexArray)
```


#### Example:

```csharp
float4[] verts = new [] { float4(0), float4(1), float4(2) };

float4 v: vertex_attrib(verts);
```

### Indexed fetch

This method will convert the provided arrays into a vertex and index buffer respectively, on initialization if possible, and fetch an element from the vertex buffer using an index from the index buffer. The syntax for this method is:

```
vertex_attrib(vertexArray, indexArray)
```

#### Example:

```csharp
float4[] verts = new [] { float4(0), float4(1), float4(2), float4(3) };
ushort[] indices = new ushort[] { 0,1,2, 0,2,3 };

float4 v: vertex_attrib(verts, indices);
```

When using multiple vertex attributes, the index array argument must be the same for all vertex_attrib operators involved in the same draw operation, otherwise a compile error will be generated.

### Performance considerations

Note that if the vertex or index buffer expression can't be evaluated to constant buffers, the compiler will in most cases have to re-upload the data to the buffers every frame. This may in turn lead to suboptimal peformance. Consider using a vertex buffer and index buffer instead where we can control upload of data manually.

There are also cases where your data is really static but this is impossible for the compiler to detect (such as when you generate the vertex data at load time). In this case, prefixing your vertex and index array meta properties or fields with the init keyword will allow the compiler to assume the data is static after initialization.

## Fetching from managed buffers

The vertex_attrib operator can also be used to fetch data from manually managed vertex and index buffers, allowing more fine-tuned control over when data is uploaded to the GPU. This method also allows multiple vertex attributes to be interleaved in the samme buffer.

The syntax for this method is:

```csharp
vertex_attrib<T>(vertexAttributeType, vertexBuffer, stride, offset [, indexType, indexBuffer])
```

Where:

* T is the data type the loaded vertex attribute should be converted to, for example float4.
* vertexAttributeType is an Uno.Graphics.VertexAttributeType enum describing how to interpret the binary data of the vertex attribute, for example Uno.Graphics.VertexAttributeType.SByte4Normalized.
* vertexBuffer is the vertex buffer to load data from. Starting in Decibel, this argument can be null, which gives undefined values as result.
* stride is the distance, in bytes, between each vertex in the vertex buffer.
* offset is the distance, in bytes, from the start of the buffer to the first vertex.

And optionally, if doing indexed fetches:

* indexType is an Uno.Graphics.IndexType enum specifying the data type of the indices.
* indexBuffer is the index buffer to load data from.

When multiple vertex attributes are participating in a draw operations, they must all use the same indexBuffer and indexType, or else a compile error will be generated.

### Example:

```csharp
draw 
{
    VertexPosition: vertex_attrib<float4>(VertexAttributeType.SByte4Normalized, buffer, 4, 0);
};
```