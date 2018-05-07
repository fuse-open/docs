# Events (UserEvent)

The `UserEvent` class can be used to emit messages (events) from a component (`ux:Class`) to the outside world. These messages (events) can be handled in UX markup and/or JavaScript.

## Creating events

To create an event, place a @UserEvent at the root of our component class to indicate that it can raise a particular event.

Where we place our @UserEvent is important, since only the node it is attached to and its children can raise or handle it.

	<Panel ux:Class="MyComponent">
		<UserEvent ux:Name="myEvent" />
	</Panel>
		
This creates an event named `myEvent`, specific to the `MyComponent` class

## App-global events

To make a @UserEvent that can be raised or handled from anywhere in the app, we simply make the event part of the App-component, by declaring it on the root @App node, like this:

	<App>
	 	<UserEvent ux:Name="myGlobalEvent" />

	 	<!-- The rest of our app goes here -->

	</App>


## Raising events

We can raise events from JavaScript using the `.raise()` method on the `UserEvent` object:

	<Panel ux:Class="MyComponent">
		<UserEvent ux:Name="myEvent" />
		
		<JavaScript>
			setTimeout(function() {
				myEvent.raise();
			}, 5000);
		</JavaScript>
	</Panel>

You can pass arguments when raising an event.

	myEvent.raise({
		userName: "james",
		isAdmin: false
	});

	
## Responding to events

We respond to events using the `OnUserEvent` trigger. We can use this trigger to perform UX actions or animators in response like any other trigger:

	<MyComponent>
		<OnUserEvent EventName="myEvent">
			<!-- Actions/animators go here -->
		</OnUserEvent>
	</MyComponent>

> Note that we are referencing our @UserEvent by name even though it is declared outside of our current scope.
> We can do this because `EventName` refers to the `Name` of the event. The actual instance of @UserEvent will be resolved at runtime.

Alternatively, we can use the `Handler` event to pass the event into JavaScript:

	<JavaScript>
		function eventHandler() {
			//do something
		}
		
		module.exports = { eventHandler: eventHandler };
	</JavaScript>
	
	<MyComponent>
		<OnUserEvent EventName="myEvent" Handler="{eventHandler}"/>
	</MyComponent>
	
Any arguments are passed to the JavaScript event handler as part of the first argument object:

	<JavaScript>
		function eventHandler(args) {
			console.log("Username: " + args.userName + ", Is admin: " + args.isAdmin);
		}
		
		module.exports = { eventHandler: eventHandler };
	</JavaScript>
	
	<OnUserEvent EventName="myEvent" Handler="{eventHandler}" />


## Raising events from UX alone

We can alternatively use the @RaiseUserEvent to raise the event from UX without using JavaScript, and even pass arguments using the `UserEventArg` class:

	<Panel ux:Class="MyComponent">
		<UserEvent ux:Name="myEvent" />
		
		<Clicked>
			<RaiseUserEvent EventName="myEvent">
				<UserEventArg Name="userName" StringValue="james" />
				<UserEventArg Name="isAdmin" BoolValue="false" />
			</RaiseUserEvent>
		</Clicked>
	</Panel>

## Creating custom triggers based on events

When creating reusable components, can subclass the `OnUserEvent` class to make new triggers the user of our components can use. This allows for better abstraction, as the user doesn't need to know you were using the `UserEvent` system at all, nor the internal name of your event. You are then free to refactor the implementation of your component later.

	<OnUserEvent ux:Class="SomethingHappened" EventName="myEvent" />

The user can then use this pattern instead:

	<MyComponent>
		<SomethingHappened Handler="{handleStuff}" />
	</MyComponent>

	
