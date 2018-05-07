# Scripting & Data-binding

The `<JavaScript>` tag in UX Markup creates a module which is instantiated for each instance of the containing scope.

Fuse implements the CommonJS module system, which means that we can export variables to the outside world by attaching them to the `module.exports` object. 

Data-binding is then done using the curly brace syntax, `Property="{variable}"`.

## App-global scripts

We can place a `<JavaScript>` tag anywhere inline in the UX markup by using the `<JavaScript/>` tag, for example directly in the `<App>` tag to create an app-global module:

	<App>
		<JavaScript>
			module.exports.foo = "bar";
		</JavaScript>
		<Panel>
			<Text Value="{foo}"/>
		</Panel>
	</App>

Code does not need to be inline in the UX markup. We can put it in a separate file, and reference it by name. This has the samme effect:

	<App>
		<JavaScript File="Main.js"/>
		<Panel>
			<Text Value="{foo}"/>
		</Panel>
	</App>

## Component-local scripts

We can also place `<JavaScript>` tags inside an `ux:Class`. This will evaluate the module for each instance of the class, giving us component-local state and data.

	<Panel ux:Class="MyComponent">
		<JavaScript>
			var Observable = require("FuseJS/Observable");
			var foo = Observable(10); 
			module.exports = { foo }
		</JavaScript>

		<Text Value="{foo}" />
	</Panel>

The 'foo' variable now exists for each instance of MyComponent, and the data binding below 

## Important note about JavaScript in Fuse

The business logic in Fuse is can be written in JavaScript or any language that compiles to JavaScript, for example using external transpilers like Babel or TypeScript.

It is important to explain that Fuse is not a web or browser-based platform despite using markup and JavaScript. The ways Fuse handle data binding, layout and animation are all tailored for the Fuse platform. Much like how any Node.js library that depends on modules like `process` and `fs` wouldn't be expected to work in a web page, Fuse's approach to JS is the same.

In short: you'll have greater success with Fuse (and more fun!) if you approach it as a new thing rather than a typical DOM-based JS framework. :)

## Data-binding example

The curly brace syntax binds to the closest object in the data context that matches the binding path.

	<App>
		<JavaScript>
			var hello = "world";

			function writeHello(){
				console.log("hello " + hello);
			}

			module.exports = {
				hello : hello,
				writeHello : writeHello
			};
		</JavaScript>
		<Panel>
			<Text Value="{hello}" Clicked="{writeHello}"/>
		</Panel>
	</App>
	
## Binding functions

We can also bind functions to events like `Clicked`, `Tapped` and `Callback.Handler`. Bound functions are passed a parameter containing various properties. These properties also vary depending on which event the function has been bound to:

 * Pointer events provide (for events like `Clicked` and `Tapped`):
   - `x`, `y`: Global pointer coordinates
   - `localX`, `localY`: pointer coordinates relative to the component origin
   - `eventName`: Name of the event fired
 * @(MapView)'s `MarkedTapped` event provides:
   - `label` -  name of the marker which has been tapped
 * @(MapView)'s `LocationTapped` and `LocationLongPressed` events provide:
   - `latitude`, `longitude` - coordinates relevant to the fired event.
 * @(Navigator)
   - `Navigated` and `PageProgressChanged` provide `name`, the name of the page being navigated to.
 * @(Image) and @(ImageSource)'s `Error` event provides:
   - `reason` - The reason which caused the error
 * @(Element)'s `Placed` event provides:
   - `x`, `y` - New coordinates of element
   - `width`, `height` - New width and height of element
 * @(ScrollView)' `ScrollPositionChanged` provides:
   - `value` - Current scroll value, as an array containing x at [0] and y at [1]
   - `relativePosition` - The amount of change in scroll since the user started a scroll
 * @(OnUserEvent) provides custom parameters you pass in through `event.raise()`. You can read more about this @(UserEvent:here.)

In all cases, we additionally get a `data` property, which contains the data context of the node from which the function was called. A normal use of this property, is to retrieve the data object from which the node was created, for example when using the `Each` class.
 
The following example shows how the age value can be retrieved from a clicked event happening on an item in an `Each` tag.

	<JavaScript>
		function clicked(arg) {
			console.log("Age: " + arg.data.age);
		}
		module.exports = {
			clicked: clicked,
			items: [
				{ name: "Jake", age: 24 },
				{ name: "Julie", age: 25 },
				{ name: "Jerard", age: 26 }
			]
		};
		</JavaScript>
		<StackPanel>
		<Each Items="{items}">
			<StackPanel Clicked="{clicked}" Margin="10">
				<Text Value="{name}" />
			</StackPanel>
		</Each>
	</StackPanel>


## Data-binding to arrays

We can data-bind to an array using the @Each behavior:

	<App>
		<JavaScript>
			var colors = ["#f00", "#0f0", "#00f"];

			module.exports = {
				colors: colors
			};
			</JavaScript>
		<StackPanel>
			<Each Items="{colors}">
				<Rectangle Color="{}" Height="40"/>
			</Each>
		</StackPanel>
	</App>

The @Each behavior will instantiate its children once for each item in its bound array. That item then becomes the data context for that instance which is why we can data-bind to the color value by an empty set of curly braces `{}`.

## Making the UI react to changes

Most of the time one wants to display dynamic data that changes over the lifetime of the app. This is simply done by storing the data in an `Observable`. The `Observable` is a core part of the Fuse reactive data-binding system, and is used everywhere in Fuse apps.

Observables act like a single value or a list of values. Any data-bound `Observable` will update the view automatically when its value is changed:

	<App>
		<JavaScript>
			var Observable = require('FuseJS/Observable');
			var count = Observable(0);
			function increment(){
				count.value = count.value + 1;
			};
			module.exports = {
				count : count,
				increment : increment
			};
		</JavaScript>
		<StackPanel>
			<Text Value="{count}"/>
			<Button Text="increment" Clicked="{increment}"/>
		</StackPanel>
	</App>

### Changing lists

We can also add more items to and `Observable` by using the `add` method:

	<App>
		<JavaScript>
			var Observable = require('FuseJS/Observable');
			var numbers = Observable(0);
			function increment(){
				numbers.add(numbers.length);
			};
			module.exports = {
				numbers : numbers,
				increment : increment
			};
		</JavaScript>
		<DockPanel>
			<Button Text="increment" Clicked="{increment}" Dock="Top"/>
			<StackPanel>
				<Each Items="{numbers}">
					<Text Value="{}"/>
				</Each>
			</StackPanel>
		</DockPanel>
	</App>


The @Each behavior will update itself when the contents of its data-bound @Observable changes.

### Adding public JavaScript functions to components

We can add our own functions to a component, and call them from anywhere the component is instantiated:

	<Panel ux:Class="TestThing" >
		<JavaScript>
			this.doThing = function() {
				console.log("thing");
			};
		</JavaScript>
	</Panel>
	<JavaScript>
		exports.thingPressed = function() {
			thing.doThing();
		};
	</JavaScript>
	<TestThing ux:Name="thing" />
	<Button Text="Press me" Clicked="{thingPressed}" />
