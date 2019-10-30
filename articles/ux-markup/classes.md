# Classes (ux:Class)

The `ux:Class` attribute defines a new UX (Uno) class where the type of the XML element is the *base class*. This means the new class inherits all public properties, events and behaviors from the superclass.

Any tree of UX Markup elements can easily be converted into a component using the `ux:Class` attribute. Classes can contain @JavaScript tags that manages the inner business logic of the component.

## Syntax

```xml
	<base_class ux:Class="class_name" />
```

Where `base_class` is any `class` accesible to UX Markup, and `class_name` is a fully qualified class name (including namespace path, if desired, separated by `.`).

## Example

The following example defines a new class `MyCheckBox` which is blue while unchecked, and red while checked. Tapping the checkbox changes the state.

```xml
	<Panel ux:Class="MyCheckBox" Color="Blue">
		<bool ux:Property="Value" />
		<JavaScript>

			function toggle() {
				this.Value.value = !this.Value.value;
			}

			module.exports = { toggle: toggle };
			
		</JavaScript>

		<WhileTrue Value="{Property Value}" />
			<Change this.Color="Red" Duration="0.2" />
		</WhileTrue>
		<Tapped>
			<Callback Handler="{toggle}" />
		</Tapped>
	</Panel>
```

## Why use classes?

Fuse encourages breaking your app into components (classes) for several reasons:

* Good practice - Component-orientation keeps your code base clean, testable, scaleable and easy to maintain.
* Reuse - It is useful to create components to allow reusing pieces of UI and logic in multiple places.
* Styling - Creating new classes based on primitives is the recommended way to create a consistent look and feel throughout your project.


# InnerClass (`ux:InnerClass`)

`ux:InnerClass` is a special version of `ux:Class`. Just like `ux:Class`, it also defines a new UX (Uno) class and inherits all the properties of the *base class*. The special thing about `ux:InnerClass` is that it has access to the names of its parent scope. This also means that you can only create instances of it inside its parent scope.

> Note: `InnerClass` can seem like a very convenient feature at first glance, but you should be careful about using it as it can lead to tight coupling of components. Its usually a good idea to stick with `ux:Class` in most situations.

## Syntax

```xml
	<base_class ux:InnerClass="class_name" />
```

## Example

In the following example, notice that we access `toggleStatusPanel` which is defined outside of our class definition of `MyInnerClass`. We can do this because we declare it using `ux:InnerClass` instead of `ux:Class`.

```xml
	<App>
		<DockPanel>
			<Panel ux:Name="statusPanel" Color="#f00" Height="80" Dock="Top" />
			<WhileTrue ux:Name="toggleStatusPanel">
				<Change statusPanel.Color="#0f0" />
			</WhileTrue>
			
			<Panel ux:InnerClass="MyInnerClass" Height="80" Color="Blue" Margin="5">
				<Clicked>
					<Toggle Target="toggleStatusPanel" />
				</Clicked>
			</Panel>
			
			<StackPanel>
				<MyInnerClass />
				<MyInnerClass />
				<MyInnerClass />
				<MyInnerClass />
			</StackPanel>
		</DockPanel>
	</App>
```

## Remarks

Note that `ux:InnerClass` covers a very special case of componentization in Fuse and should generally not be needed. Prefer creating an explicit interface in and out of your components using `ux:Property` and `ux:Dependency`.
