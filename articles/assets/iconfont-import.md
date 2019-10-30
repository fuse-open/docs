# Icon fonts

Icon fonts, like [FontAwesome](http://fontawesome.io/), are special fonts that contain vector icons instead of characters. These provide crisp icons in your app without having to use @(MultiDensityImageSource).

## Importing an Icon font

Icon fonts work just like any other font. To use one, you need to create a @(Font) element, like this:

	<Font File="assets/fontawesome-webfont.ttf" ux:Global="FontAwesome" />

With this done, we can use the font from any component by setting `Font` to whatever the global name of the `Font` is. In our case, this would be `FontAwesome`.

## Using an icon font

As the icon font uses special unicode characters, you need to use escaped character literals. These are escaped differently in JavaScript and UX.

### UX

In UX, you escape the unicode literals as you normally would in xml. This example shows a @(Text) containing the [cog icon](http://fontawesome.io/icon/cog/) from FontAwesome:

	<Text Font="FontAwesome">&#xf013;</Text>

Note that this relies on `FontAwesome` being the global name for the font.

### JavaScript

In JavaScript, there are two cases. If the character code you want to use is below 65536 (or, is able to be written with 4 hexadecimal digits), we can escape it like this:

	var cog = '\uF013';

However, if the character code is higher than 65535, you have to do a bit of a workaround due to JavaScript using UCS-2 encoding for its characters, meaning each character is 16 bits wide. The trick to handle these cases are *surrogate pairs*. We can read more on surrogate pairs, including how to convert to them [here](https://mathiasbynens.be/notes/javascript-encoding).

Once you have managed to escape your character as a string on the JavaScript side, we can use it in UX as you normally would with databinding:

	<JavaScript>
		var Observable = require("FuseJS/Observable")
		var cog = Observable('\uF013');
		module.exports = {
			cog: cog
		};
	</JavaScript>
	<Text Value="{cog}" Font="FontAwesome" />
