# undefined

The undefined keyword is a special expression that allows meta properties to be set to "undefined", overriding any actual definitions further up the draw path while keeping its type known.

The undefined keyword can only be used in meta property definitions, and can not be combined with other expressions.

## Example

```csharp
// Defines a meta property
float SomeMetaProperty: 1.0f;

...

// "Undefines" the meta property. Any subsequent draw statements will not see the above definition, however its type will still be visible.
SomeMetaProperty: undefined;
```

The undefined keyword can also be used to "conditionally" undefine meta properties:

```csharp
// Undefines SomeMetaProperty if DiffuseMap is defined as texture2D, otherwise keep previous definition.
SomeMetaProperty: req(DiffuseMap as texture2D) undefined, prev;
```