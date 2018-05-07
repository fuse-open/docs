# Resources (ux:Key)

The `ux:Key` attribute in UX Markup converts the element on which it resides into a dynamic resource that applies to the subtree.

Resources can be used with *resource bindings*, created with the `{Resource resource_key}` binding syntax.

## Syntax

	<type ux:Key="resource_key" [ux:Value="value"] ... />

Where `type` is any type available to UX Markup, and `resource_key` is any string. 

> Allthough not strictly required, it is reccommended to use a `resource_key` that consists of valid Uno identifiers, separated by periods `.` for namespacing.

If the type is a value type (such as `float4` or `int`), the `ux:Value` attribute must be specified.

## Resource bindings

To create a binding to a resource key, we use the `{Resource resource_key}` syntax.

Example:

	<Panel Color="{Resource MyApp.ThemeColor}"

## Global default resources (`ux:Global`)

The `ux:Global` attribute can be used to define global defaults for resource keys. If a resource key is not resolved in the UX tree scope, a global resource mathing the resource binding key will be used instead.

### Example 

The following example defines a `ux:Global` default value for the `MyApp.ThemeColor` resource key, which then applies for all parts of the app tree where nothing else is specified.

	<App>
		<float4 ux:Global="MyApp.ThemeColor" ux:Value="Green" />

		<Navigator>
			<HomePage ux:Template="homePage" />
			<ProfilePage ux:Template="profilePage" />
			<SettingsPage ux:Template="settingsPage">
				<float4 ux:Key="MyApp.ThemeColor" ux:Value="Blue" />
			</SettingsPage>
		</Navigator>
	</App>

On the `SettingsPage`, we override the resource key using the `ux:Key` attribute, to `Blue` instead of `Green`. This means any use of `{Resource MyApp.ThemeColor}` whithin the settings page will be `Blue` instead of `Green`, unless overridden even deeper down the tree.
