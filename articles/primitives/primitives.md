# Primitives

Fuse comes with a set of primitive elements which are the basic building blocks of more complex visual elements. These primitives includes @Text, @Rectangle, @Circle, @Image and @Video.


## Shapes

The two most used shapes in apps are rectangles and circles. They each have their own type in Fuse:

```
<Panel>
	<Circle Color="#bbfdff" />
	<Rectangle Color="#ff6eb4" />
</Panel>
```
Check out the full documentation for @Rectangle and @Circle.

## Color

All visual elements in Fuse have a `Color` property. This property maps to what would intuitively be considered the main color of the element in question.

For shapes, like rectangle and circle, one expects the `Color` property to translate to a @SolidColor brush of that color. This means that the following sample:

```
<Rectangle Color="#f0f" />
```

is actually turned into

```
<Rectangle Fill="#f0f"/>
```

Since the `Fill` property is of the @Brush type, the `Fill="#f0f` part is actually expanded even further into the following equivalent:

```
<Rectangle>
	<SolidColor Color="#f0f"/>
</Rectangle>
```

Where the @SolidColor is "auto-bound" to the rectangles `Fill` property.

## Brushes and Strokes

Colors in Fuse are represented as Brushes. To get a normal solid color, we can use the @SolidColor brush. All shapes has a `Fill` property, which is of the `Brush` type. Because setting solid colors on shapes is so common, there exist a special `Color` property.

## Text 

The @Text class is the basic primitive to display static text. 

Here is an overview of the difference between the different text-related controls:

* Text - Displays non-interactable text which can wrap over multiple lines.
* TextInput - Single-line user-interactable text control.
    * Text only - has no special decoration
    * Has a DockLayout, which mean we can place children using the `DockPanel.Dock` attached property.
    * bool IsPassword
    * TextInputActionHandler ActionTriggered
* TextBox - Like TextInput, but has basic decoration so it is easier to when empty (e.g. for prototyping)
* TextView
    * Multiline TextInput
    * Undecorated
    * cannot be password
    * cannot have special action

* Basic.TextInput - A decorated TextInput from the Fuse.BasicTheme package.


## Image / Video

We can easily include images and videos by using the @Image and @Video elements:

```
<Image File="someImage.jpg" />
<Video File="someVideo.mp4" />
```

Both @Image and @Video also supports getting a url instead of a local file:

```
<Image Url="http://www.some.com/image.jpg" />
<Video Url="http://www.some.com/video.mp4" />
```

Check out @Image and @Video documentation detailed documentation.
