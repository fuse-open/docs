### Migration guide to Fuse 0.20

#### No more Themes or Styles

* Remove the `Theme` property from your `<App />` class. This means no more `Theme="Basic"`.
* Remove `ux:InheritStyle`, `ux:Cascade`, `IgnoreStyle` attributes, as they no longer have any meaning or effect.

An empty app now looks like this:

```xml
<App>
</App>
```

Note that all semantic controls (`Button`, `Switch`, `Slider` and `TextInput`) will no longer get the `Basic` theme apperance, but instead a much more plain generic look/no look at all. To keep the old appearance, prefix all uses of those controls with `Basic.`, e.g. `<Basic.Button />`. Remember to add a reference to `Fuse.BasicTheme` in your `.unoproj` if you want access to this (not default).

The long deprecated `<Style>` tag is now finally removed. Rewrite to use `ux:Class` instead, which is a much nicer and much more performant way of doing consistent styling.

Some users have a legacy habit of using `<Style ux:Class="MyStyle">` as a dummy-container for a collection of classes. In this case, we can simply replace `<Style ..` with `<Panel ..` or any other dummy to keep this pattern.


#### Changes in default package references

* Remove all the `Fuse.*` packages (including `FuseCore`) and replace with two simple references to `Fuse`, `FuseJS`.

You might also want to add a reference to the following depending on whether you need a feature:

* `"Fuse.GeoLocation"`
* `"Fuse.Camera"`
* `"Fuse.Vibration"`
* `"Fuse.Launcher"`
* `"Fuse.PushNotifications"`
* `"Fuse.LocalNotifications"`
* `"Fuse.BasicTheme"`

A minimal .unoproj file should now look something like this:

```json
{
  "Packages": [
    "Fuse",
    "FuseJS",
    "Fuse.GeoLocation"
  ],
  "Includes": [
    "*"
  ]
}
```

If you have references to `Uno.*` packages, whether you need to keep them around depends on whether you have `.uno` code depending on those packages.

If you want to use the `Basic.` controls, add a reference to `Fuse.BasicTheme`. These are now not included by default to avoid bloat in your generated app if you are not using it.

You need to keep references to third party packages that you depend on in either UX, Uno or JS.

#### `ux:Global` on JavaScript tags is now deprecated

Using `ux:Global` on JavaScript modules to make them accessible to `require()` will have unpredictable behavior in `fuse preview`.

Instead require the JS file directly by including it in your project as a bundle file, for example:

```json
"Includes": [
    "*",
    "*.js:Bundle"
]
```

and then use require with the filename instead:

```js
var foo = require('foo'); // requires foo.js in the project root
var bar = require('./../bar'); // requires bar.js in the folder above the current file
```

Check out [the detailed docs](fusejs/fusejs.md#importing-modules-by-file-name) for more info.


#### Changes to TextInput and TextEdit APIs

`TextEdit` is now removed. `TextInput` now gives an undecorated text input control. If you want a decorated text input, we can get the old `Basic` style by using `Basic.TextInput`.

* Replace all occurrences of `TextEdit` with `TextInput`. The previous use case for the `TextEdit` was to get an undecorated `TextInput`. Instead, the `TextInput` is now undecorated.
* For existing `TextInput`, use the `Basic.TextInput` if you want to keep the old style, or keep as `TextInput` if you want to keep it undecorated. Y
* `TextInput` is now always single line. To get the multiline version of `TextInput`, use a `TextView` instead.
* `PlainTextEdit` no longer exists. Use `TextInput` instead.
* As `TextInput` is invisible by default, we can use the new `TextBox` to get a decorated text input (easier while prototyping).

We can now style `TextInput` by putting elements inside it. It behaves like a `DockPanel` where remaining space is the actual editor:
```xml
	<TextInput>
		<Rectangle Dock="Bottom" Height="2" Color="Blue" Margin="4" />
	</TextInput>
```

#### Changes (bugfixes) to UX element ordering semantics

* `Element`s placed inside `Trigger`s will now be placed at the same location in the children list as their containing `Trigger`.

Here is an example where this matters:

```xml
<Panel>
	<Each Count="2">
		<ElemOne/>
	</Each>
	<ElemTwo/>
</Panel>
```

The previous behavior turned the above UX into the following equivalent:

```xml
<Panel>
	<ElemTwo/>
	<ElemOne/>
	<ElemOne/>
</Panel>
```
Notice that the element inside the `Each` are placed after element outside it.

The new behavior works like this:

```xml
<Panel>
	<ElemOne/>
	<ElemOne/>
	<ElemTwo/>
</Panel>
```

The elements are now placed where you'd intuitively expect them to be.

#### Consolidated ux:Template and ux:Factory concepts

In UX markup, the concept of Factory has been renamed to Template. The old Template concept is removed (was used in Styles, which are now removed).

The new Template-concept is used heavily in the new Router/Navigator APIs.

* Instead of `ux:Generate="Factory"` use `ux:Generate="Template"`

We can also create a named template by using `ux:Template="the_name"`.

The `PageControl.DotFactory` has been renamed to `PageControl.DotTemplate`.


#### Changes to animation APIs

* `Attractor.SimulationType` has been removed in favor of two new properties; `Type` and `Unit`. Take a look [here](https://www.fusetools.com/learn/reference/fuse/animations/attractor_1) for details.

* `LinearNavigation` no longer has an `Easing` and `Duration` property.

If you need to use a custom `Easing` and `Duration`, rewrite

```xml
	<LinearNavigation Easing="CubicOut" Duration="0.3" />
```

To this:

```xml
	<LinearNavigation>
		<NavigationMotion Easing="CubicOut" Duration="0.3" />
	</LinearNavigation>
```

#### Uno API Changes

These changes are only relevant if you have custom `.uno` code in your projects.

* `protected override void OnRooted()` and `OnUnrooted()` no longer take the parent node as an argument. The Parent property is assumed to be set at the time these methods are called.

* Nodes no longer have a separate list for elements, behaviors and properties. They are all in the same list called `Children`.

* Many places that used to take in a `Node` now take in a `Visual` instead.
  For example, in the`WhileLoaded` trigger:

```csharp
public static void SetState(Node n, bool loaded)
```
has become
```csharp
public static void SetState(Visual v, bool loading)
```
