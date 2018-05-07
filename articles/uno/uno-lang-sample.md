# sample

The sample keyword is used as an operator for reading filtered samples from textures on the GPU.

## Syntax

```
sample(texture, texCoord [, samplerState])
```

Where texture is an expression of type texture2D or textureCube.

The texCoord defines at what position to sample the texture.

* If texture is a texture2D, then texCoord must be of type float2. The texture image is mapped to range (0..1), (0..1), irrespective of the actual resolution of the texture. When specifying co-ordinates outside this range, the texture will repeat or clamp, based on the samplerState settings.
* If texture is of type textureCube, then texCoord must be of type float3. This value is interpreted as a direction vector pointing to a position within the cube which is to be sampled. The length of the vector is irrelevant.

The samplerState is an optional argument of type Uno.Graphics.SamplerState that specifies extra options on how to sample the texture. The samplerState must be evaluated on a CPU stage (cannot be changed per vertex or pixel). If the sampler state is not specified, Uno.Graphics.SamplerState.TrilinearClamp is used by default.

### Remarks

In Uno Feature Level 2.0, sample can only be used on the pixel stage (in pixel shaders).
