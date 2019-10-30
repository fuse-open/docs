# ux:Binding

`ux:Binding` is used to explicitly choose which property a UX object should bind to.

## Syntax

```xml
	<type ux:Binding="some_property" />
```

## Remarks

It is seldom necessary, but there are a few cases where we need to explicitly bind to another property than the default. An example can be found in the chapter about `ux:Dependency` and how those are forwarded using `ux:Binding`. Here are a few other examples:

### The Container class

In the following example we have to explicitly bind the `Rectangle` to the `Children` property of `Container` using `ux:Binding`. This is because `Container` doesn't have `Children` as its default property. It instead forwards its child elements to the element specified by its `Subtree` property.

```xml
	<Container ux:Class="MyContainer" Subtree="innerPanel">
	    <Rectangle ux:Binding="Children" CornerRadius="10" Margin="10">
	        <Stroke Color="Red" Width="2" />
	        <Panel Margin="10" ux:Name="innerPanel" />
	    </Rectangle>
	</Container>
```

### Specifying different forward and backward easing with `CubicBezierEasing` 

```xml
	<Move X="100" Duration="0.3">
	    <CubicBezierEasing ux:Binding="Easing" ControlPoints="0.4, 0.0, 1.0, 1.0" />
	    <CubicBezierEasing ux:Binding="EasingBack" ControlPoints="0.3, 0.0, 0.3, 1.0" />
	</Move>
```

In the above example, we have to use `ux:Binding` in order to explicitly bind the two `CubicBezierEasing` objects to different properties. Otherwise, they would both try to bind to the default property of `Move`, which for `CubicBezierEasing` would be the `Easing` property.

# ux:AutoBind

The `ux:AutoBind` attribute is used to specify whether a UX object should automatically bind to the default property (that matches its type) of its parent. This is something we normally don't have to think about, except for a few edge-cases.

## Syntax

```xml
	<type ux:AutoBind="true/false" />
```

## Remarks

```xml
	<StackPanel>
		<Text Value="Hello" />
		<Rectangle Color="Red" />
	</StackPanel>
```

In the above example, the `Text` and `Rectangle` are automatically bound to the `Children` property of the `StackPanel` this is an implementation detail we usually shouldn't worry about, but for the cases we do, we can use `ux:AutoBind` to turn of that behavior:

```xml
	<StackPanel>
		<Text Value="Hello" ux:AutoBind="false" />
		<Rectangle Color="Red" ux:AutoBind="false" />
	</StackPanel>
```

In the above example, we end up with two "free floating" elements which can be bound to properties by using `ux:Binding`.
