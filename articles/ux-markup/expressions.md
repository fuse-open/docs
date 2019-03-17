# UX Reactive Expressions

UX Markup offers a simple, powerful and extensible *reactive expression* syntax that can be used on any property on any object. This document explains how it works.

> Note that UX expressions is an experimental feature and is subject to change. If you're using this in UX markup, please keep an eye on the changelog when updating. If you're using this feature from Uno, be aware that several large API changes are planned.

## Quick Summary

UX Markup's *Reactive Expressions* allow you to combine *literals*, *resources* and all supported *bindings* into expressions using *operators* and *functions*. These expressions update on a push-based, automatic basis if any of the *bindings* produce a new value.

In the following example, a slider controls the position of a rectangle.

	<Rectangle X="spring({Property slider.Value})*30% + 50%" Alignment="TopLeft" Width="32" Height="32" />
	<Slider ux:Name="slider" Minimum="-1" Maximum="1" />

Note in particular the expression for the `X`-offset of the rectangle:

	spring({Property slider.Value})*30% + 50%

This means that the rectangle will be offset by `50%`, plus a value that ranges between `-30%` and `30%` through a `spring` physics function.

## Bindings

Bindings are objects that listen for changes in the environment and produces a new value that feeds into the UX expression. All binding types can be used as part of expressions:

* Data bindings (`{path.to.data}`)
* Property bindings (`{Property someObject.SomeProperty}` or `{Property SomeProperty}`)
* Resource bindings (`{Resource resourceKey}`)

Note that inline expressions (`{= expression}`) also use the curly brace syntax, yet these are technically not bindings.


## Arithmetic

UX expressions can add (`+`), subtract (`-`), multiply (`*`) and divide (`/`) any scalar or vector value, with or without units (such as `%` and `px`).

	<Panel Width="{foo} * 100% + 40%" />
	<Slider Margin="{foo}" />

Arithmetic can only be performed between values of the same unit, or between one value with unit and one value without unit. You can **not** do `10% + 10px`, for example. This will give a runtime error.

## Vectors

Vectors (up to 4 components) can be constructed using the comma `,` operator:

	<Panel Margin="10, {spacing}, 10, {spacing} / 2" />

Vectors will also be automatically expanded to the appropriate size from a shorter vector, according to the following rules: `X` -> `XXXX`, `XY -> XYXY` and `XYZ` -> `XYZ1`.

	<Panel Margin="{spacing}" />

## Strings and text

String properties (such as `<Text Value=""`) are parsed differently from other properties. A string property is treated as a literal string, except for curly braced operands.

	<Text>Hello, {username}!</Text>

Or, equivalently:

	<Text Value="Hello, {username}" />

To embed a computed expression inside a string value, you can use the `{= expression}` syntax, for example:

	<Text>Hello, {= toLower({username})}!</Text>

## Functions

> Note: The current set of functions are experimental and will change. Keep an eye on future changelogs.

These functions are currently available:

### Layout

* `x(element)` - returns the X coordinate of an element's position, relative to its parent.
* `y(element)` - returns the Y coordinate of an element's position, relative to its parent.
* `width(element)` - returns the computed width resulting from layout, in points, of another UX element.
* `height(element)` - returns the computed height resulting from layout, in points, of another UX element.

### Strings

* `toLower(s)` - returns a lowercase version of the given string.
* `toUpper(s)` - returns the uppercase version of the given string.

### Math

* `min(a, b)` - returns the minimum of two numeric values (component by component if vector).
* `max(a, b)` - returns the maximum of two numeric values (component by component if vector).
* `abs(value)` - returns the absolute value of the input number
* `mod(a, b)` - returns the remainder after division of `a` with `b`
* `fract(value)` - returns the fractional part of the input number by removing the integer digits
* `trunc(value)` - returns the integer part of the input number by removing any fractional digits
* `odd(value)` - returns true if the rounded `value` is odd, false otherwise
* `even(value)` - returns true if the rounded `value` is even, false otherwise
* `sign(value)` - returns the sign of the input number, indicating whether the number is positive, negative or zero
* `round(value)` - returns the value of the input number rounded to the nearest integer
* `ceil(value)` - returns the smallest integer greater than or equal to the input number
* `floor(value)` - returns the largest integer less than or equal to the input number
* `sqrt(value)` - returns the square root of the input number
* `pow(a, b)` - returns `a` raised to the power of `b`
* `exp(value)` - returns a number representing `e` raised to power of `value`, where `e` is Euler's number
* `exp2(value)` - returns 2 raised to power of `value`
* `log(value)` - returns the natural logarithm (base _e_) of the input number
* `log2(value)` - returns the base 2 logarithm of the input number
* `sin(radians)` - returns the sine of the input angle given in radians
* `cos(radians)` - returns the cosine of the input angle given in radians
* `tan(radians)` - returns the tangent of the input angle given in radians
* `asin(value)` - returns the arcsine (in radians) of the input number
* `acos(value)` - returns the arccosine (in radians) of the input number
* `atan(value)` - returns the arctangent (in radians) of the input number
* `atan2(y, x)` - returns the arctangent of the quotient of the arguments
* `degreesToRadians(degrees)` - returns the value in radians for the input in degrees
* `radiansToDegrees(radians)` - returns the value in degrees for the input in radians

### Misc

* `attract(value, config)` - animates the change in a value by using an `AttractorConfig` to define the animation style

```xml
<AttractorConfig Unit="Points" Easing="SinusoidalInOut" Duration="0.3" ux:Global="asPoints" />
<Panel>
	<Translation X="attract({xOffset}, asPoints)"/>
</Panel>
```

* `alternate(value, groupSize)` - alternate between true/false value for ranges of integers
* `spring(value)` - performs a spring physics simulation that chases the given `value`.
* `lerp(from, to, step)` - returns the linear interpolation between two values
* `clamp(value, min, max)` - returns the value restricted by the range specified
* `index()` - returns the index of this item inside `Each.Items`
* `offsetIndex()` - returns the index of this item inside `Each.Items`, relative to `Each.Offset`

```xml
<JavaScript>
	var items = [1,2,3,4,5,6,7,8,9,10];
	module.exports = {
		items: items
	};
</JavaScript>
<StackPanel ItemSpacing="8" Margin="8">
	<Each Items="{items}" Offset="2" Limit="4">
		<Panel ux:Name="item" Height="48">
			<Text Value="value: {}; index: {= index(item)}; offsetIndex: {= offsetIndex(item)}" Alignment="CenterLeft" />
		</Panel>
	</Each>
</StackPanel>
```

**Note:** Avoid using layout functions (like `width(element)`, `height(element)`, `x(element)` or `y(element)`) as input to layout parameters (like `Width`, `Height`, `Margin` or `Padding`) . Doing this can cause unwanted or glitchy behavior:

```xml
<Panel Width="10" Height="width(this)" />
```
