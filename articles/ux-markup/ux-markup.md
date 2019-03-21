# UX Markup Reference

UX Markup is an XML based language for expressing user interfaces, layout, effects and motion. UX Markup is an essential part of Fuse.

This chapter covers the meaning of the built-in syntaxes and attributes of the UX Markup language.

> Use the menu to the left to navigate the different topcis in this chapter.

## Overview

### Creating Objects

The basic function of a UX Markup element is to create (instantiate) an object of a certain *class*. For example, the following snippet creates instances of the @App and @Rectangle classes, and makes the @Rectangle a child of the @App.

```xml
	<App>
		<Rectangle Color="Blue" />
	</App>
```

The name of the element tag must be the name of an *Uno class* accessible in the project. In the above example, @App and @Rectangle are pre-defined classes within the Fuse libraries.

> To make the Fuse libraries accessible in your project, make sure you add `"Fuse"` to the `Packages` section of your `.unoproj`

### Creating Heirarchies

When one element (child) is placed inside another element (parent) in UX Markup, the outer element is called the *parent*, while the inner element is called the *child*. 
	
```xml
	<Grid Rows="1*,1*">
		<Rectangle Color="Blue" Margin="10" />
		<Rectangle Color="Blue" Margin="10" />
	</Grid>
```

By default, UX Markup will find a suitable property on the parent element to which it can *bind* the child element.

To disable this behavior for a given child element, we can specify `ux:AutoBind="false"`. This will create a loose object within your app that can be attached to your app later by logic.

To bind to a specific property of the parent, we can specify `ux:Binding="TheProperty"`. This will disable auto-binding for the element, and instead bind to `TheProperty`.

### Creating reusable classes (`ux:Class`)

Any UX element tree can be converted to a reusable class (component) by giving it the `ux:Class` attribute.

```xml
	<Panel ux:Class="MyNamespace.MyComponent" Color="Yellow" >
		<WhilePressed>
			<Scale Factor="2" Duration="0.3" Easing="BackOut" />
		</WhilePressed>
	</Panel>
```

This creates a new subclass of `Panel` called `MyComponent` in the `MyNamespace` namespace. Namespaces are optional, but recommended when creating components for reuse in other projects, to avoid name collisions.

Once a class is created, it can be instantiated just like any of the built-in classes:

```xml
	<MyNamespace.MyComponent />
```

Each `ux:Class` creates a new *root scope*, which means that nodes inside the class have no access to names (`ux:Name`) outside of the class. This means dependencies must be injected explicitly.

## The @App tag

The `App` tag is the ultimate root of your app project.

```xml
	<App>
		<!-- your app goes here -->
	</App>
```

Any @App tag is implicitly a `ux:Class` with the same name as the containing file excluding file extension. We can also give your app class a different name by specifying `ux:Class` manually:

```xml
	<App ux:Class="MyNamespace.MyApp" />
```

Projects that don't contain an `App` tag does not produce an actual app, but a packages of components that can be referenced by other projects.


## Namespaces

UX Markup supports XML namespaces (`xmlns`), but to keep code verbosity down, sensible defaults have been set. This is why you rarely see it used in UX markup files.

The default XML namespace (`xmlns`) points by default to all the standard Fuse namespaces, so classes from there (e.g. @App and @Rectangle) can be used without full namespace qualifier.

For other classes we can use the full qualified name of a class directly on a tag:

```xml
	<App>
		<MyNamespace.MyClass />
	</App>
```

Or, we can specify a custom `xmlns` declaration for a scope:

```xml
	<App xmlns:m="MyNamespace">
		<m:MyClass />
	</App>
```

## Including UX files (`ux:Include`)

We can include the content of one UX file in another using `ux:Include` as follows:

```xml
	<ux:Include File="Resources.ux" />
```


`ux:Include` is useful for declaring static resources such as @Fonts, @Images, colors, etc. in a separate file.

It is however not recommended practice for splitting parts of your view into files. See the [Creating Components](../componentization.md) article for a better approach.
