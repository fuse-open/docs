# Properties (ux:Property)

The `ux:Property` attribute in UX Markup defines a new property on the containing `ux:Class`.

User-defined properties allows us to expose configurable or animateable parameters for our custom components.

Properties can be refered to within UX Markup using *property bindings*, on the form `{Property PropertyName}` or `{Property objectName.PropertyName}`.

> When creating a class (component) in UX Markup, it automatically inherits all properties from the base class.

## Syntax

```xml
	<type ux:Property="property_name" />
```

Where `type` is the name of an Uno type (such as `Panel` or `string`), and `property_name` is a valid Uno identifier.

## Default values

The defaul value for a property can be specified as a regular property on the containing `ux:Class`.

Example:

```xml
	<Panel ux:Class="MyComponent" NumberOfThings="13">
		<int ux:Property="NumberOfThings" />
		...
	</Panel>
```

## Remarks


### Property bindings

Properties can be refered to within UX Markup using *property bindings*, on the form `{Property PropertyName}` or `{Property objectName.PropertyName}`.

Property bindings can be *two-way* (default), *read-only* (`{ReadProperty ...}`) or *write-only* (`{WriteProperty ...}`).

### Example

In the following example, the control defines a new property called `BackgroundColor`, with a default value (set on the root node) of `#f00`. This color is then property-bound to the `Color` property of a background rectangle with rounded corners. 

```xml
	<Panel ux:Class="MyButton" BackgroundColor="#f00">
		<float4 ux:Property="BackgroundColor" />
		<Text>SUBMIT</Text>
		<Rectangle Layer="Background" Color="{Property BackgroundColor}" CornerRadius="10" />
	</Panel>
```

When the component is instantiated, the user can either leave the default `BackgroundColor`, or set a new one.

```xml
	<MyButton />  <!-- this one will keep the default color -->
	<MyButton BackgroundColor="#0f0" /> <!-- this one will use green instead -->
```

Properties can also be animated using a `<Change>` animator:

```xml
	<MyButton ux:Name="mb1" />
	<WhilePressed>
		<Change mb1.BackgroundColor="#00f" Duration="1" />
	</WhilePressed>
```

### Choosing the right type for a property

Properties are *strong typed*, and must be qualified with an Uno type name. Any Uno type can be used, including all regular UX tag names.

Often properties will have *value types*, such as numbers, text, colors, sizes (potentially with units such as `%` or `px"), or vectors. It is important to pick the property type that best captures the intended range of valid values for the properties.

In many cases, we are *property-binding* the property directly to an existing property. In this case, see the documentation for the existing property, and prefer to use the same type. In the above example, we used *float4* as the type, because this is the type of the @Color property of @Panel.

Here are some guidelines on what types to use:

* **Whole Numbers** - Use `int`
* **Decimal Numbers** - Use `double` (or `float`)
* **Numbers with units** such as `10%` - Use `Size`
* **2D-vectors** - Use `float2` if the value hs

### What's the difference between `ux:Property` and `ux:Dependency` ?	

Properties are similar to dependencies (`ux:Dependency`), with the following differences:

* Properties are mutable. This means they can be changed at any time, and even animated.
* The component can provide a default value, while dependencies must always be injected by the user.
* The property can be *property-bound* using property bindings.

One drawback of using properties instead of dependencies is that due to their mutable nature, *objects* passed in as properties can not be referenced in UX markup by name, in e.g. `<Change>` animators. 

Unlike dependencies, properties are not available directly as named objects in JavaScript, but rather as `Observable` properties on the `this` object in the root scope of the module.

## Using Properties in JavaScript.

All user-defined properties (defined through `ux:Property`) are automatically reflected as `Observable` objects in `JavaScript` modules in the scope of the property.

Here is an example of how to respond to property changes in JavaScript:

```xml
	<Panel ux:Class="RgbDisplayer" RGB="#A00">
		<float4 ux:Property="RGB" />
		<JavaScript>
			var Observable = require("FuseJS/Observable");
			var rgbString = this.RGB.map(function(val) {
				return  "R: " + (val[0]*255).toFixed(1) + 
					" G: " + (val[1]*255).toFixed(1) + 
					" B: " + (val[2]*255).toFixed(1);
			});
			module.exports = { rgbString: rgbString };
		</JavaScript>
		<Text Value="{rgbString}" />
	</Panel>
```

### Important to understand about Properties in JavaScript

Note that all UX objects and properties reside on the UI thread, while JavaScript runs in the background. This means that **the value of the property is not neccessarily up-to-date at the time your JavaScript code runs**. This is a common cause of confusion.

Instead of reading the observable's `.value` property, use reactive operators like `.map()` or `.onValueChanged()`. This also ensures that your component updates as it should when the value of the property changes or is animated.

If you need the value of the property available immediately when your module executes, consider using `ux:Dependency` instead.

### Passing observables through properties

Observables can also be passed into custom made components using Properties. We can add a property which accepts Observables by making an `object` property:

```xml
	<Panel ux:Class="CoolPanel">
		<object ux:Property="ObservableProperty" />
		<JavaScript>
			var passedInObservable = this.ObservableProperty.inner();
		</JavaScript>
	</Panel>
	<JavaScript>
		var Observable = require("FuseJS/Observable");
		module.exports = {
			valueToPass: Observable("123")
		};
	</JavaScript>
	<CoolPanel ObservableProperty="{valueToPass}" />
```

You are almost always interested in using `inner()` when fetching an observable passed in through properties. This is because the javascript value `this.PropertyName` is an observable with whatever `PropertyName` contains. If we pass an Observable in, `this.PropertyName` will contain an observable with the observable we passed through.
