# Layout

Fuse comes with an extremly powerful layout system that lets one create UIs in a reactive way that will fit on many different screen sizes and aspect ratios. It also makes it straight forward to make components that work with a wide range of input types and sizes.

## Defining UI layout

UIs in Fuse are created by a hierarchy of @Panels, as well as other primitives like @Rectangle and @Image. The various @Panel types like @StackPanel, @Grid and @DockPanel are used to position and size multiple elements relative to each other as well as relative to the available screen size.

The following example shows how to make a grid with two rows and three columns filled with rectangles of various colors.

```
<App>
	<Grid RowCount="2" ColumnCount="3">
		<Rectangle Color="#ff4500"/>
		<Rectangle Color="#ee7942"/>
		<Rectangle Color="#ee6363"/>
		<Rectangle Color="#ffb90f"/>
		<Rectangle Color="#eeb422"/>
		<Rectangle Color="#eedc82"/>
	</Grid>
</App>
```

We can nest several panels to create more complex structures:

```
<App>
	<StackPanel>
		<DockPanel Height="50">
			<Text Value="Hello" Dock="Left"/>
			<Rectangle Color="#f00" />
		</DockPanel>
		<DockPanel Height="50">
			<Text Value="World" Dock="Left"/>
			<Rectangle Color="#00f" />
		</DockPanel>
	</StackPanel>
</App>
```


## Element layout

For each UX-tag representing a `Visual` element, we can set properties that will affect the elements `Width`, `Height`, `Opacity` and much more. We can also control how it relates to its parent node and its children nodes using `Alignment`, `Padding` and `Margin`.

### Margin / Padding

`Margin` and `Padding` are two of the most used element properties. `Margin` controls the distance from the edges of an element to the corresponding edges of its container. @Padding works similarly to @Margin, but instead works inwards. It sets the distance from its edges to the corresponding edges of its child elements.

Both `Margin` and `Padding` are of type `float4`, which means they have four floating point value components. They are specified in UX as comma separated lists:
```
<Panel Margin="10,20,30,40" />
<Panel Padding="10,20,30,40" />
```

Each value represents one of the elements four edges: "10(left),20(top),30(right),40(bottom)".

We can also write in shortened forms:

```
<Panel Margin="10" /> <!-- is expanded to "10,10,10,10" -->
<Panel Padding="10,20" /> <!-- is expanded to "10,20,10,20" -->
```

### Alignment

In the cases where an element is smaller than the available space it is assigned by its parent, we can use the `Alignment` property to control where in its available space it is placed.

The following example shows how to place rectangles in the four corners of the screen.

```
<App>
	<Panel>
		<Rectangle Alignment="TopLeft"     Width="40" Height="40" Color="Red" />
		<Rectangle Alignment="TopRight"    Width="40" Height="40" Color="Blue" />
		<Rectangle Alignment="BottomLeft"  Width="40" Height="40" Color="Green" />
		<Rectangle Alignment="BottomRight" Width="40" Height="40" Color="Yellow" />
	</Panel>
</App>
```

### Width / Height

The `Width` and `Height` properties can be used to ensure a given `Element` will have specific dimensions on-screen. By default, these are usually set to `Auto`, which means the element's size will be determined automatically by the layout engine. By explicitly setting them to concrete values, we can have fine-grained control over the dimensions of our element.

These properties can be set independently of each other, e.g. `Width="30px"` and `Height="Auto"`.

Additionally, properties like `MinWidth`/`MaxWidth` and `MinHeight`/`MaxHeight` can be used to constrain an element's size to given minimum or maximum values, respectively.

### Units

Many layout properties, like `Width`, `Height`, `X` and `Y`, can be specified by different units. The following units are available in most cases.

* Percent - `Width="50%"` makes the `Width` of an element 50 percent of its parent width.
* Points (default) - `Width="50"` makes the `Width` of an element 50 points. This means that the width will appear the same on all screen densities.
* Pixels - `Width="50px"` makes the `Width` be exactly 50 pixels. This means that the width will appear smaller on screens with higher pixel densities.

## Layout rules

Panels are used to arrange several elements in relation to each other. All panel types have an associated `Layout` object attached to it. The plain `Panel` type uses @DefaultLayout to lay out its children. It does nothing, other than place them on top of each other in the Z-axis. The @StackPanel has an associated @StackLayout and places its children in a vertical or horizontal stack. Read more about the @StackPanel and other panel types in the upcoming sub sections.

Some panel types, like the @Grid and @DockPanel needs information per child element in order to lay them out. In these cases, we can use the accompanying attached properties associated with the panel type. Attached properties have the form `ClassName.PropertyName="Value"` and are available to all elements. On all unambiguous cases, we can ommit the "Class" part, and just do `PropertyName="Value"`. In this case, attached properties are syntactically identical to normal properties.

Here are a couple examples:

* Dock - Use the `Dock` attached property to specify which side a particular element should be docked to (`Top`, `Bottom`, `Left`, `Right`, `Fill`).
* Grid - Use the `Row` and `Column` attached properties to specify which row and column an element should be placed in.

Here is an example of how it looks in UX:

```
<Grid ColumnCount="3" RowCount="1">
	<Panel Row="0" Column="0" Color="#f00"/>
	<Panel Row="0" Column="2" Color="#00f"/>
</Grid>
```

## Draw order and the ZOffset property

Normally, the order in which elements are drawn depends on their place in the child list. Elements defined further up in the document are drawn on top of elements defined further down. In the following case, The red rectangle will be drawn on top of the green circle.

```
<Panel>
	<Rectangle Color="Red" Margin="100"/>
	<Circle Color="Green"/>
</Panel>
```

To take control over this behavior, we can use the @ZOffset property. This property is a float value with a default of 0.0. The higher it is the higher on top the element will be drawn.

In the following example, the @Circle is drawn on top of the @Rectangle because its `ZOffset` of 1.0 is higher than the rectangles `ZOffset` of 0.0 (default).

```
<Panel>
	<Rectangle Color="Red" ZOffset="0.0" Margin="100"/>
	<Circle Color="Green" ZOffset="1.0"/>
</Panel>
```
