# Feature overview

This article gives an overview of the high-level concepts and patterns in Fuse. Follow links to read more in-depth sections on each topic.

## Uno Projects (`.unoproj`)

[The Uno Projects article](uno-projects.md) covers how to configure your app project, manage references, includes and bundling.

This article assumes your project is configured to include at least one UX Markup file to contain your @App tag.

## The @App tag

The App tag is the root of your application tree. The presence of an @App tag in one of your project's UX Markup files indicates that the project is an app, and not just a library of components.

Fuse automatically finds the @App tag of your project and uses that as the root component. We can only have one @App tag in our project.

	<App>
		<Text>Hello, world!</Text>
	</App>

## Components

In Fuse, an app is tree of UX markup components (instances of Uno classes).

The basic building blocks are primitives such as @Text, @Rectangle, @Video, @Slider or @Image. These can be composed using @Panels for hierarchical layout, such as @StackPanel and @Grid.

	<App>
		<StackPanel>
			<Text>Hello, World!</Text>
			<Text>Hello again!</Text>
		</StackPanel>
	</App>

At the root levels of your @App, each element is only instantiated (created) *once* for the entire lifetime of the app. However, there are special UX nodes such as @Each and attributes like `ux:Template` which can create multiple instances, lazy-instantiate and recycle components as appropriate.

> See the [Primitives](../primitives/primitives.md), [Layout](../layout/layout.md) and @Control chapters for more info on these topics

## Scripting and data contexts

Business logic in Fuse apps is done using script components such as the @JavaScript class. These can be placed at any level in the app tree.

	<App>
		<JavaScript>
			console.log("Hello, World!");

			module.exports = {
				items: [
					{ color: "#f00" },
					{ color: "#0f0" },
					{ color: "#00f" }
				]
			}
		</JavaScript>
		<StackPanel>
			<Each Items="{items}">
				<Panel Color="{color}" Height="40" Margin="10">
					<JavaScript>
						console.log("hello!");
					</JavaScript>
				</Panel>
			</Each>
		</StackPanel>
	</App>

Scripts will execute once for each instance of the containing node. For the first @JavaScript in the above example, that means just once for the entire app. However, the @JavaScript inside the `Each` will execute once for each instance of the Panel. In the above example, `hello!` is logged to the console three times.

The resulting `module.exports` from each script will become *data context* for the subtree. We can use *data binding* with the curly brace `{binding}` syntax against the data context to populate the view with data.

Data contexts cascade. This means at any node, you have access to all data contexts in the outer tree. If names clash, the closest one upwards in the tree takes precedence.

> See the [Scripting & Data chapter](../scripting/scripting.md) for more info on this topic.

## Creating new components (`ux:Class`)

UX Markup is a composable language where existing components can be combined to create new, more advanced components. Any tree of UX Markup elements can easily be converted into a component using the `ux:Class` attribute. This has multiple usages.

### Styling

To create a consistent look and feel througout your app, Fuse relies on creating subclasses of primitives to assign default properties and behaviors.

For example, here is a simple class (component) that provides a fixed text style:

	<Text ux:Class="HeaderText" FontSize="36" Color="#88f">
		<DropShadow Size="5" Angle="120" />
	</Text>

It can be used like this:

	<HeaderText>This is a header</HeaderText>

Note how Fuse does *not* have any equivalent to CSS, nor does it make any attempt to separate style from structure. However, Fuse goes to great length to separate the visual user experience (defined in UX Markup) from the business logic (defined in e.g. JavaScript).

### Reusable Components

Another important use case of subclassing is building reusable components, optionally with inner logic, public properties and events.

As another example, here is a simple custom button component:

	<Panel ux:Class="MyButton">
		<string ux:Property="Text" />
		<Text Value="{ReadProperty this.Text}" />
		<Rectangle CornerRadius="5" Color="#ccf" />
	</Panel>

It can then be used anywhere in the project like any other component:

	<MyButton Text="Submit" Clicked="{doSomething}" />

> See [Creating Components](../componentization.md) for more info on this topic.

> It is also possible to create new UX components through Uno code. See the [Native Interop chapter](../native-interop/native-interop.md) for more information.

## Navigation

A typical app consists of a set of @Pages through which the user navigates using gestures, tapping controls or using a physical back-button on the device.

Navigation in a Fuse app is controlled via a @Router object that is typically placed directly inside your @App tag. This makes the router available to all nodes in the the class scope using the given name, and automatically hooks it up to the physical Back-button on devices that have them.

	<App>
		<Router ux:Name="router" />
	</App>

The @Router doesn't do much on its own. The @Router works though *router outlets* such as `PageControl` and `Navigator` placed in the subtree, which in turn contains the actual pages. The @Router can then be controlled from javascript.

	<App>
		<Router ux:Name="router">
		<PageControl>
			<Page ux:Name="contacts">
				...
			</Page>
			<Page ux:Name="newsfeed">
				...
			</Page>
			<Page ux:Name="settings">
				...
			</Page>
		</PageControl>
	</App>

> See the [Navigation chapter](../navigation/navigation.md) for more details on this topic.

## Splitting up into multiple UX files

As the project grows, we usually want to split the app up into multiple UX Markup files. In Fuse, splitting UX Markup can be done at any level in the tree you wish, with any component as the root node. However, a natural and recommended place to start is splitting by *page*.

For the above example, this would mean turning each page into a component using `ux:Class`, and moving it to a separate UX file.
This involves a little more code as components need to make their dependencies explicit. The added benefit is components are easier to test, maintain and reuse.

We start by copying the page code out into a separate file, and replace the `ux:Name` with a proper `ux:Class` name. It is common to use upper case on the first letter for class names.

Next, we need to identify and declare the *dependencies* of this page. It might reference the `router` or other objects declared in the scope outside. We declare them inside our class by using the `ux:Dependency` attribute, as seen below.

`ContactsPage.ux`

	<Page ux:Class="ContactsPage">
		<Router ux:Dependency="router" />
		...
	</Page>

And similar for `NewsFeedPage.ux` and `SettingsPage.ux`. Then we change our App UX to:

	<App>
		<Router ux:Name="router" />
		<PageControl>
			<ContactsPage ux:Name="contacts" router="router" />
			<NewsFeedPage ux:Name="newsfeed" router="router" />
			<Settings ux:Name="settings" router="router" />
		</PageControl>
	</App>

This gives identical behavior, while allowing the page components to be reused in other contexts. The only requirement for reusing the component elsewhere is that we can provide the dependencies (i.e. `router`).

> See the [Creating Components article](../componentization.md) for more details on this topic.
