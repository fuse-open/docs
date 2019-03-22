# Local names (ux:Name)

The `ux:Name` attribute in UX Markup is used to provide a local name to a UX object.

## Syntax

```xml
	<type ux:Name="node_name" />
```

Where `type` is any type accessible in UX Markup, and `node_name` is a valid Uno identifier.

## Remarks

Any object (XML element) in UX markup can be given a `ux:Name` attribute to assign a local name to it. 

```xml
	<Panel ux:Name="panel1" />
```

Once a name is given, the object can be referenced by that name within the same *scope*. Any UX expression can then refer to this object by it's plain name. Local names take precedence over global resources with the same name.

```xml
	<Rectangle LayoutMaster="panel1" />
```

Names can be used direclty in UX Expressions, for example like this:

```xml
	<Rectangle Width="width(panel1) / 2"
```

> Note that ux:Name can not be data-bound as it is a compile-time attribute. Data-binding happens at run-time.

## Valid names (Uno identifiers)

Names must be valid Uno identifiers. This means they can only contain letters (A-Z or a-z), numbers and underscores, and can not start with a number.
