# Structuring app resources

In this article we will discuss best practices around managing assets and resources in Fuse projects.

We usually separate between resources and assets as follows:

- Assets usually come in the form of files containing content like images, videos or fonts.
- Resources define certain values that can be used throughout our app, like colors, font declarations (which acts as a reference to a font file usable in our apps) or strings with different translations.

## Folder structure

We like to keep files organized according to which role they fulfill in our app. Thusly, we usually create a _Resources_ folder and an _Assets_ folder, where we put our resources and assets respectively. Beyond this, feel free to further organize your assets and resources in subsequent subfolders.

### Example

- MyApp/
	- MyApp.unoproj
	- MainView.ux
	- Components/
	- Pages/
	- Resources/
		- ColorPalette.ux
		- ColorTheme.ux
		- Fonts.ux
	- Assets/
		- Fonts/
			- RobotoBold.ttf
			- RobotoRegular.ttf
		- Images/
			- DefaultAvatar.png
			- SplashScreen.jpg

## Resources

A resource in Fuse is a value that we associate with a key (a name), so that it can be referenced by that key instead of having to replicate the exact value in multiple places. Examples of values that we prefer to make resources for include colors, fonts, strings and even margins and paddings.
Another very useful aspect of resources is that they can be overridden at any time, meaning we can use this to define color palettes / themes that we can swap out even while the app is running.

There are two ways we can define resources and while they act in a similar way, they have some subtle but important differences.

- **Static resources** are defined using the `ux:Global` attribute and allow us to associate a value with a key and make it available __globally__ in our app.
- **Dynamic resources** are defined using the `ux:Key` attribute and allow us to associate a value with a key. This makes the value available to the entire subtree of where it is defined.

Lets take a look at an example:

```xml
<float4 ux:Global="AppBackground" ux:Value="#7FDBFF" />
```
Here we have defined the name `AppBackground` to refer to the blueish color #7FDBFF. This value can then be used like so, anywhere in our app:

```xml
<Rectangle Color="AppBackground" />
```

Defining static resources/globals with meaningful names like this lets us easily tweak the look and feel of our app with only a single code change, even though the color is used in many places.

A limitation of static globals is that they cannot be changed while the app is running. For that use-case, we have dynamic resources:

```xml
<Panel>
	<float4 ux:Key="AppBackground" ux:Value="#7FDBFF" />
	<Rectangle Color="{Resource AppBackground}"/>
</Panel>
```

In the above example we've defined the same color, but as a dynamic resource instead. There are a couple things to notice about this snippet.
We have wrapped the `Rectangle` and `float4` resource in a `Panel` to illustrate that the resource has to be either a sibling of the `Rectangle`, or belong to one of its ancestors in order to be visible. Since we put it on the `Panel` it is available to the panels descendants, or any of its descendants.
You might also have noticed that we needed to use a special _binding syntax_ in order to access this resource: `{Resource AppBackground}`. Since the resource might change while the app is running, we need to create a binding here so that our `Rectangle` is notified whenever that occurs. 

<blockquote class="callout-info">

The `{Resource <key>}` binding also works for static resources defined using `ux:Global`. It will first look for a dynamic resource defined by `ux:Key`, but if none is present, it will pick the static one by the same name instead.

</blockquote>

## Color palettes

When defining a color palette for our app, it can be a good idea to put it in its own file. This way we don't get a long list of color definitions cluttering our UI code, and it becomes easier to make color changes at a later stage. One detail of Fuse is that all files must define a `ux:Class` in order to be included in the app. When we just want to define a bunch of resources however, we don't want to also have to define a class to contain them.
To make grouping resources more convenient, UX markup has a special tag called `ux:Resources`, which is used as follows:

```xml
<ux:Resources>
	<float4 ux:Global="MyApp.RedColor" ux:Value="#F44336" />
	<float4 ux:Global="MyApp.PrimaryColor" ux:Value="MyApp.RedColor" />
	<float4 ux:Global="MyApp.TopBarBackgroundColor" ux:Value="MyApp.PrimaryColor" />
</ux:Resources>
```

All resources defined inside `ux:Resources` tags are automatically included in the app.

### Tips on structuring color palettes

- You should define all colors as static resources using `ux:Global`. Instead of using these globals directly however, it is a good idea to instead use the `{Resource <key>}` binding syntax so we can easily define various color schemes at a later stage.
- It's better to use semantic names for colors. For example, instead of naming colors after their value, like `RoseGold` or `MintGreen`, use names that hint to where in the app the colors should be used, like `LightBackgroundColor`, `DarkTextColor` or `MainAccentColor`.

## Themes

The current way of creating themes in Fuse is to simply group all resources associated with the theme in an active trigger:

ColorPalette.ux
```xml
<ux:Resources>
	<float4 ux:Global="Sienna" ux:Value="#a0522d" />
	<float4 ux:Global="SkyBlue" ux:Value="#7ec0ee" />
	
	<float4 ux:Global="MyApp.PrimaryColor" ux:Value="Sienna" />
</ux:Resources>
```

MyTheme.ux
```xml
<WhileTrue ux:Class="MyTheme" Value="true">
	<float4 ux:Global="MyApp.PrimaryColor" ux:Value="SkyBlue" />
</WhileTrue>
```

We can then very easily use this theme in our app by adding it to our app root:

```xml
<App>
	<MyTheme />
	<Panel Color="{Resource MyApp.PrimaryColor}" />
</App>
```

Since `MyTheme` inherits from the `WhileTrue` trigger, it can easily be turned on and off by either data-binding its `Value` property to a `JavaScript` variable, or with pure UX using for example a `Clicked` trigger and a `Toggle` action:

```xml
<App>
	<MyTheme ux:Name="myTheme"/>
	<Panel Color="{Resource MyApp.PrimaryColor}">
		<Clicked>
			<Toggle Target="myTheme" />
		</Clicked>
	</Panel>
</App>
```

<blockquote class="callout-info">

Note that we here have also defined a `ux:Global` for `MyApp.PrimaryColor`. This is to make sure our app has a fallback value for the case when no theme has been specified.

</blockquote>


## Fonts and text styles

Fuse supports both `.otf` (OpenType) and `.ttf` (TrueType) font files. In order to make a font available throughout your app you can create a static resource for it:

```xml
<Font ux:Global="RobotoBold" File="Assets/Fonts/Roboto-Bold.ttf" />
```

The font is then ready to be used in `Text` and `TextInput` objects by assigning their `Font` property:

```xml
<Text Font="RobotoBold">Hello with Font</Text>
```

Instead of assigning your custom font to all text objects however we prefer creating a set of text styles using `ux:Class` and instead use those in our app. This makes it easy to switch our fonts, text size and colors throughout our app by making changes in a single place.

```xml
<Text ux:Class="MyApp.TitleText" Font="RobotoBold" FontSize="20" Color="{Resource MyApp.TitleTextColor}" />
<Text ux:Class="MyApp.BodyText" Font="RobotoRegular" FontSize="13" Color="{Resource MyApp.BodyTextColor}" />
```

```xml
<StackPanel>
	<MyApp.TitleText>This is a title</MyApp.TitleText>
	<MyApp.BodyText>And this is some body text...</MyApp.BodyText>
</StackPanel>
```
