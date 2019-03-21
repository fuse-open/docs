# Templates (ux:Template)

The `ux:Template` attribute in UX Markup, when placed on an XML element, specifies that the given XML element and its entire subtree is a template (`Uno.UX.Template`). Templates can construct an arbitrary number of copies of the UX Markup tree at runtime.

## Syntax

```xml
	<type ux:Template="template_name" />
```

Where `type` is any valid UX Markup type, and `template_name` is a valid Uno identifier.

## Remarks

Templates are used in several contexts, most noticeably:

* Inside an `<Each>` repeater, the content nodes are implicitly templates.
* Inside a `<Navigator>`, page templates can be provided instead of regular nodes, to allow the `Navigator` to construct new copies of each page template on demand.

## The `ux:Name` of the template instance

Inside node marked `ux:Template`, the current copy (instance) of the current template can be refered to by the `template_name`, the same way as if it was named the same way using `ux:Name`.

For example, in a contained `<JavaScript>` tag, we can refer to the template instance by name:

```xml
	<Page ux:Template="homePage">
		<JavaScript>
			homePage.Parameter.onValueChanged(module, function() { ...
```

Also, in UX markup in general, we can use the page name in expressions:

```xml
	<Page ux:Template="homePage">
		<Rectangle LayoutMaster="homePage" ... />
```

## Using templates for component customizability

Templates can be used to increase the customizability of a component by allowing it to take custom elements to be used for its appearance. An example of this in action is the @PageIndicator element, which spawns an element from a template for each page in a `PageControl`. An element is made a template by setting the `ux:Template` property to a key you want to identify that template by. This key is used by template accepting elements when looking for appropriate templates to draw.

Templates are drawn using `Each`. `Each` has a property called `TemplateSource`, which specifies the element from which `Each` gets its template. As mentioned before, templates identify themselves using a key. The key `Each` will look for a template with is specified using the `TemplateKey` property.

A common `TemplateSource` is a `ux:Class` the `Each` is a child of:

```xml
	<StackPanel ux:Class="CoolRepeater" Background="#FAD">
		<Each TemplateSource="this" TemplateKey="Item" Count="20">
			<Text>Default template</Text>
		</Each>
	</StackPanel>
```

This is a custom component which accepts a template, which it repeats 20 times. If no template is given, the `Each` will fallback to its default template: whatever is inside the `Each`. The custom component can then be used like this:

```xml
	<CoolRepeater>
		<Text ux:Template="Item">Hello, world!</Text>
	</CoolRepeater>
```
