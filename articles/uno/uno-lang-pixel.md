# pixel

The pixel is on operator that can be used in meta property expressions to force the result of the expression into the pixel/fragment shader.

## Example

In the following code, if both LightDirection and Normal can be evaluated in the vertex shader. The compiler will always prefer to do calculations in the vertex shader instead of the pixel shader to save pixel shader fill rate.

```csharp
DiffuseLight: Vector.Dot(LightDirection, -Vector.Normalize(Normal));
```

This will however give per-vertex lighting, which in some cases gives too poor visual quality. To improve quality, we can use the pixel keyword in front of an operand to force subsequent calculations into the pixel shader, improving quality at the cost of fill rate.

```csharp
DiffuseLight: Vector.Dot(LightDirection, -Vector.Normalize(pixel Normal));
```

Here, the Normal value will be forced into the pixel shader before the Vector.Normalize is performed, resulting in per-pixel normalized vectors and much prettier lighting.