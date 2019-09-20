## Globals (`ux:Global`)

The `ux:Global` attribute in UX Markup converts the element on which it resides into a static global resource.

Static global resources can be used locally in you project, or used to define resource libraries that can be refered from other projects.

## Syntax

```xml
	<type ux:Global="resource_key" [ux:Value="value"] ... />
```

Where `type` is any type available to UX Markup, and `resource_key` is any string. 

> Allthough not strictly required, it is recommended to use a `resource_key` that consists of valid Uno identifiers, separated by periods `.` for namespacing.

If the type is a value type (such as `float4` or `int`), the `ux:Value` attribute must be specified.

## Examples

As an example, Fuse defines global resources for common color names like `Red` and `Blue`. These can be referred to by their *name*:

```xml
	<Panel Color="Blue" />
```

You can define custom global resources of any type using the `ux:Global` attribtue:

```xml
	<float4 ux:Global="MyProject.WarmBlue" ux:Value="#18f" />
```

And then use it anywhere:

```xml
	<Rectangle>
		<Stroke Width="3" Color="MyProject.WarmBlue" />
	</Rectangle>
```

Note that global resource names may contain periods `.`. It is encouraged to use periods in resource names for grouping according to project, company or context.

Global resources are resolved at *compile-time* and can not change dynamically. For dynamic resources, see *Resources*.

## Globals as default resources

The `ux:Global` attribute also defines a global default value for resource bindings. See docs on `ux:Key` for more info.
