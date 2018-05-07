# draw_dispose

Disposes the resources associated with any draw statements in the containing class.

## Example usage

```csharp
public void Dispose()
{
    draw_dispose;
}
```

### Remarks

draw statements may allocate GPU resources such as vertex buffers and textures that require explicit disposal when their are no longer needed. The draw_dispose statement can be used to perform such cleanup at a controlled time.