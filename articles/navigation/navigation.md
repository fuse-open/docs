
# Navigation

This tutorial gives a high level overview of how Fuse's main navigation system works, using classes like `Router`, `Navigator` and `PageControl`.

## Complete app example

This example app show how to use the navigation system in Fuse:

* [Cattr - A basic multi-view app example in Fuse.](https://github.com/fusetools/fuse-samples/tree/master/Samples/cattr)

## Creating pages

When designing a multi-page app, it is recommended to define each page in its own UX file as an `ux:Class`, like this:

`LoginPage.ux`

	<Page ux:Class="LoginPage">
		<Router ux:Dependency="router" />
		<JavaScript File="LoginPage.js" />
		...
	</Page>

`SettingsPage.ux`

	<Page ux:Class="SettingsPage">
		<Router ux:Dependency="router" />
		<JavaScript File="SettingsPage.js" />
		...
	</Page>

And so on, for each page.

Notice how each page can have its own `<JavaScript>` tag to handle internal logic for that page.

Most pages will need access to the app's `Router`, so we declare that as an `ux:Dependency="router"`. This means the JavaScript on that page can access that router.
If our page has other dependencies, we can declare them the same way.


## Assembling the `App`

The basic structure of your main UX file should be something like this:

	<App>
		<Router ux:Name="router" />
		<Navigator DefaultPath="login">
			<LoginPage ux:Template="login" router="router" />
			<HomePage ux:Template="home" router="router" />
			<SettingsPage ux:Template="settings" router="router" />
			<UserProfilePage ux:Template="user" router="router" />
		</Navigator>
	</App>

The `ux:Template` attribute indicates that this is not a real object instance, but rather a template that can be instantiated on demand. The `Navigator` will take care of instantiating as many instances of each template as needed, and recycle instances when they are not needed anymore.

The `DefaultPath` specifies what templates should be instantiated by default when the app starts up and no route is set yet.

Notice how we inject the dependencies just like properties. It is good practice to keep names of dependencies lowercase, like names. This allows you to extract components without renaming your objects, and makes it easy to spot the difference between a dependency and a property.

Note that all dependencies *must* be specified, otherwise you'll get a compile time error.

> The pages do not need to be descendants of the @Page class. Any visual object, such as a @Panel can be used.

## Navigating around (flat navigation)

In the above example, the `login` template (`LoginPage` class) will be our starting page.

Let's give the `LoginPage` a button:

	<Button Text="Login" Clicked="{loginClicked}" />

Hook its `Clicked` event up to this function in `LoginPage.js`:

	function loginClicked() {
		// TODO: validate login credentials

		router.goto("home");
	}

This will do what you expect, the navigator will animate the login screen out, and bam! We're on the `home` page.

> Remember to add `loginClicked` to your `module.exports`.

## Hierarchical navigation

Hierarchical navigation is navigating to ("pushing") a temporary page, and then later "popping" (going back) to wherever you came from, either using an on-screen button, or the device's physical back-button.

Let's say a button is supposed to open the `settings` page, and then the back-button should take us back to wherever we came from:

	router.push("settings");

Easy, right? Now the back button on Android will take us back.

> In local preview, use Cmd/Ctrl+B to emulate the back button.
> On iOS, hold 3 fingers for 1 second to bring up a menu where we can emulate the back button.

We can also trigger a go-back from JavaScript:

	router.goBack();

We can use `push()` multiple times to navigate "deeper". Then you have to tap the back button multiple times to get all the way back out.

We can use `goto()` while inside a deep stack of pushed pages to discard the stack and simply go to that page. Using the back button after that will not take you back.

## Passing parameters to pages

Sometimes you want to use the same page template, but open it with different parameters.

For example, our `user` page could really benefit from knowing the ID of the user to display the profile for.

This is easy by passing argument objects to the router:

	router.push("user", { userId: id });

This will temporarily open a new instance of our `user` template, with the provided object as parameter.

In `UserPage.js` we can read the parameter using the @Observable `Parameter`, which lets you react to changes to the parameters:

	var userId = this.Parameter.map(function(param) {
		return param.userId;
	});
	
	module.exports = {
		userId: userId
	};
	
Make a note of the fact that `this.Parameter` is an @Observable, which means that its value is not necessarily available when the module is being evaluated.
Make sure you either expose it using an observable operator (like in the example above), or add a handler to it using the `onValueChanged` function:

```js
this.Parameter.onValueChanged(module, function(param) {
	//At this point we know then new parameter value.
	
	//module is used to connect the lifetime of the
	//subscription to the lifetime of the module
});
```

Note that `this` in the root scope of `UserPage.js` refers to the current instance of the `UserPage` class.

> Be aware that in JavaScript, the meaning of the `this` keyword varies with function scope. Make sure you grab the reference to the right instance in the root of your module.

Note that the same `UserPage` instance may be reused by the `Navigator` to display different users over time. `Parameter` will only be updated when you need to display a new user.

## Multi-level navigation

Sometimes, it is not enough to just have one level of navigation. Our `home` screen, for example, may contain multiple pages within itself.

No worries, the `Router` can deal with this just as easily. Say that our `HomePage.ux` looks something like this:

	<Page ux:Class="HomePage">
		<Router ux:Dependency="router" />
		<JavaScript File="HomePage.js" />
		<PageControl>
			<SocialFeedPage ux:Name="socialfeed" />
			<DirectChatsPage ux:Name="chats" />
			<PinnedMessagesPage ux:Name="pinned" />
			<RecentNotificationsPage ux:Name="recent" />
		</PageControl>
	</Page>

> Note: PageControl expects to get live page objects, not templates. PageControl keeps all pages alive and only one instance of each, to e.g. allow swipe navigation between them.

Both `Navigator` and `PageControl` are so-called *router outlets*. This means they participate in routing. When using `.goto()` or `.push()`, we can provide a multi-level route, with parameters for each level.

For example, our login-screen might want to send us directly to the `RecentNotificationsPage` instead of `SocialFeedPage`. That's done like this:

	router.goto("home", {}, "recent", { scrollOffset: 300 });

And that's all! First the `Navigator` will get you to the `home` template, and then the `PageControl` within that template will go to the `recent` page. This is a *path*, and can also be specified relatively, as we will see below. This can be compared to the path of an URL, which would look like this: `home/recent?scrollOffset=300`

The `home` template in this case does not take any parameters, so we pass it an empty object (`{}`).

Meanwhile, the `recent` template will get the parameter `{ scrollOffset: 300 }` object to its `Parameter` @Observable, as an example of how we can pass parameters to pages.

### Relative navigation

A neat trick to making custom pages independent of a large navigation tree, is to navigate relative to the closest @Navigator object, instead of the whole tree. This is done using the `gotoRelative` and `pushRelative` functions.

These two functions behave in the same way as their other variants(`goto`, and `push`), except that they expect a `Navigator` or `PageControl` as the first argument. Any path specified after this will be relative to the provided @Navigator/@PageControl.

	<JavaScript>
		module.exports.toSub1 = function() {
			router.pushRelative(secondNav, "option1");
		}
	</JavaScript>
	<Navigator ux:Name="secondNav" DefaultTemplate="optionList">
		<Panel ux:Template="optionList">
			<Button Margin="10" Text="Subpage 1" Clicked="{toSub1}"/>
		</Panel>
		<Panel Color="#FAA" ux:Template="option1" />
	</Navigator>

## Custom animations/transitions

Note that by default `Page` objects have a built-in default `Transition="Default"`, which means the containing control defines a meaningful default transition. For example, `PageControl` by default makes pages slide horizontally to enter and exit, while
in `Navigator` enter sliding in from the left, and exit shrinking and fading out into the background.

If you want to customize the transtion, you probably want to disable the default transition first:

	<Page ux:Class="SettingsPage" Transition="None">

If you don't specify `Transition="None"`, your custom animation will be "on top of"/"in addition to" the default transition.

The `Navigator` works with the standard navigation animation system in Fuse. This means that page transitions are defined by the following triggers:

* `EnteringAnimation` - played backwards for the entering page (the page becoming the active page)
* `ExitingAnimation` - played forwards for pages replaced by a `push`
* `RemovingAnimation` - played forwards for pages removed because of a `goto`

When you `goBack()`, the opposite happens`:

* `EnteringAnimation` - played forwards for the page being removed by a `goBack`
* `ExitingAnimation` - played backwards for the page being restored bt a `goBack`

This system can be a bit tricky to wrap your head around, but it is actually necessary to allow you to fully customize all the four different cases.

For example:

	<Page ux:Class="SettingsPage" Transition="None">
		<EnteringAnimation>
			<Move X="1" RelativeTo="Size" Duration="0.4" Easing="BackOut" />
		</EnteringAnimation>
		<ExitingAnimation>
			<Move Y="1" RelativeTo="Size" Duration="0.4" Easing="CubicIn" />
		</ExitingAnimation>
		...
	</Page>

This will make your settings page fly in from the left, and exit by falling out the bottom of your screen. Feel free
to spice it up by adding `<Change this.Opacity="0" Duration="0.3" ../>`, scaling, rotation or whatever you feel like.

If you have a lot of pages and want them to share the transition code, we can make a common base class:

	<Page ux:Class="FancyTransitionPage" Transition="None">
		<EnteringAnimation>
			<Move X="1" RelativeTo="Size" Duration="0.4" Easing="BackOut" />
		</EnteringAnimation>
		<ExitingAnimation>
			<Move Y="1" RelativeTo="Size" Duration="0.4" Easing="CubicIn" />
		</ExitingAnimation>
	</Page>

And then use that as base class for your pages:

	<FancyTransitionPage ux:Class="SettingsPage">
		...
	</FancyTransitionPage>

Other navigation-based triggers such as `ActivatingAnimation`, `WhileActive` etc. also works as expected in both `PageControl` and `Navigator`.


## And done!

That's it, you now have an elegant, scalable model for navigation and structuring your app.
