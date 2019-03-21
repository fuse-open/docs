# Alive themes

An [Alive.FallbackTheme](api:alive/fallbacktheme) is required inside a @Panel at the root of our App.
This sets up default theme colors and makes sure we always have one of the themes enabled at any point in the UX tree.

```xml
	<App>
		<Panel>
			<Alive.FallbackTheme />

			<Panel>
				<!-- app content -->
			</Panel>
		</Panel>
	</App>
```

**Note:** Alive.FallbackTheme is explicitly a [LightTheme](api:alive/lighttheme).
To make the entire app be [DarkTheme](api:alive/darktheme), enclose it within a @Panel together with the rest of the app's content.

```xml
	<App>
		<Panel>
			<Alive.FallbackTheme />

			<Panel>
				<Alive.DarkTheme />

				<!-- app content -->
			</Panel>
		</Panel>
	</App>
```

In fact, you can place a theme on any @Visual, which will make that @Visual and all of its children use the specified theme.
In the below example, the [ThemedCard](api:alive/themedcard) has the dark theme, while the [Slider](api:alive/slider) has the light theme.

```xml
	<App>
		<Panel>
			<Alive.FallbackTheme />
			
			<Alive.ThemedCard>
				<Alive.DarkTheme />

				<Alive.Slider>
					<Alive.LightTheme />
				</Alive.Slider>

			</Alive.ThemedCard>
		</Panel>
	</App>
```

Themes work by exposing a collection of [Resources](articles:ux-markup/resources).
The components themselves can then access these resources using `{Resource` bindings.

## Overriding the accent color

Each theme exposes an `Alive.AccentColor`, which many controls use this to color themselves.

Since the dark and light themes use different colors, we must specify an accent color for both themes.
**Note:**: You _should not_ override `Alive.AccentColor` directly, since it will be overriden by any themes deeper in the tree.
Instead, provide the same value for both `Alive.Light.AccentColor` and `Alive.Dark.AccentColor`

```xml
	<Panel>
		<float4 ux:Key="Alive.Light.AccentColor" ux:Value="Alive.Light.Blue" />
		<float4 ux:Key="Alive.Dark.AccentColor" ux:Value="Alive.Dark.Blue" />
	</Panel>
```