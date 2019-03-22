# Creating native JavaScript Modules in Uno

`Fuse.Reactive.JavaScript` allows you to define native code modules in Uno and use them
from JavaScript. This is how all native modules (like Camera, Vibrate etc.) in FuseJS
are implemented.

A module is a JavaScript object that can be acquired through the `require` function.

## Defining a native module

To define a native module, you create an instance of `Fuse.Scripting.NativeModule`. Remember to also add a reference to *Fuse.Scripting* in your .unoproj-file if it isn't already added.

To the constructor we can pass a list of members. We can also add additional members in the
constructor through the `AddMember()` method.

Currently, two types of members are supported:

* `NativeFunction` - wraps an Uno delegate as a JavaScript function
* `NativePromise` - wraps an Uno `Promise<T>` as an EcmaScript 6 style `Promise`

> Note: For the time being, exposing native modules to JavaScript is a bit hacky. You need to mark your class with the `[UXGlobalModule]` attribute, make it a singleton with a public constructor, and expose it as a global resource using `Resource.SetGlobalKey()`. We are working on a prettier way to solve this.

Here is a simple example, exposing a custom logging function:

```csharp
	using Fuse;
	using Fuse.Scripting;
	using Uno.UX;

	[UXGlobalModule]
	public class MyLogModule : NativeModule
	{
		static readonly MyLogModule _instance;
		
		public MyLogModule()
		{
			// Make sure we're only initializing the module once
			if (_instance != null) return;

			_instance = this;
			Resource.SetGlobalKey(_instance, "MyLogModule");
			AddMember(new NativeFunction("Log", (NativeCallback)Log));
		}

		static object Log(Context c, object[] args)
		{
			foreach (var arg in args)
				debug_log arg;

			return null;
		}
	}
```

We can then use your native module from JavaScript, as shown below.

```xml
	<App>
		<Panel>
			<JavaScript>
				var LogModule = require("MyLogModule");

				LogModule.Log("Hello from JavaScript!");
			</JavaScript>
		</Panel>
	</App>
```

## NativeFunction

Native functions are defined by providing a `Fuse.Scripting.NativeCallback` delegate, which
has the following signature:

```csharp
	public delegate object NativeCallback(Context c, object[] args);
```

Where `args` is a list of JavaScript arguments passed to the function. An argument can be
of one of the following types, depending on what is passed to the function at runtime:

* A number of type `double` or `int` if the argument is a JavaScript number.
  We can the use [`Fuse.Scripting.Marshal`](api:fuse/marshal.json) class to correctly marshal to a specific desired value, e.g.

	`var number = Marshal.ToInt(args[0]);`

* A `bool` if the argument is a JavaScript bool.
* A `string` if the argument is a JavaScript string.
* A [`Fuse.Scripting.Array`](api:fuse/scripting/array.json), if the argument is a JavaScript array.
* A [`Fuse.Scripting.Function`](api:fuse/scripting/function.json), if the argument is a JavaScript function.
* A [`Fuse.Scripting.Object`](api:fuse/scripting/object.json), if the argument is a JavaScript object other than an array or function.
* A [`Fuse.Scripting.External`](api:fuse/scripting/external.json), if the argument is a boxed Uno object.
* `null` if the JavaScript object is `null` or `undefined`.

The return value for your callback follows the same rules, and additionally allows [`Fuse.Scripting.Callback`s](api:fuse/scripting/callback.json) for Uno callbacks that should be marshalled as JS functions.
If you return `null`, the corresponding JS function will return `null`.

## Native EventEmitter modules

We can automatically make our `NativeModule`s instances of the JavaScript class
@EventEmitter by using a @NativeEventEmitterModule, which is a
subclass of `NativeModule`. `EventEmitter`s allow emitting and listening to
named events, and @NativeEventEmitterModules in turn allow emitting such
events from Uno.

The following example illustrates how to create a @NativeEventEmitterModule
that uses an event to echo any message sent to it using a `send` function:

```csharp
	using Fuse;
	using Fuse.Scripting;
	using Uno.UX;

	[UXGlobalModule]
	public class Chat : NativeEventEmitterModule
	{
		static readonly Chat _instance;

		public Chat()
			: base(true, "messageReceived")
		{
			if (_instance != null) return;

			_instance = this;
			Resource.SetGlobalKey(_instance, "Chat");

			AddMember(new NativeFunction("send", (NativeCallback)SendMessage));
		}

		object[] SendMessage(Fuse.Scripting.Context context, object[] args)
		{
			var arg = args.Length > 0 ? args[0] as string : null;

			if (arg == null)
				Emit("messageReceived", "----");
			else
				Emit("messageReceived", arg);

			return null;
		}
	}
```

Note that we have to explicitly call the base class constructor when creating a
@NativeEventEmitterModule. The first argument is a bool that determines
whether any events that happen before the app has been fully initialized should
be cached and replayed. Any other arguments are the allowed event names. In
this case we only have one event called `"messageReceived"`.

We call `Emit("messageReceived", arg)` to pass `arg` to any listeners that
listen to the `"messageReceived"` event on the JavaScript side.

The `chat` module can then be used from JavaScript like so:

```js
	var chat = require("Chat");

	function send() {
	    chat.send(new Date().toString());
	}

	chat.on("messageReceived", function(message) {
	    console.log("Message received " + message);
	});
```

See the documentation of @EventEmitter for more information.

## NativePromise

Promises are objects that represent objects that may become available in the future, and can either succeed
or fail in retrieving that object. Examples are results of HTTP requests, dialog boxes or taking pictures
with the camera.

Native modules support wrapping Uno promises in JS promises automatically, through the `NativePromise` class.

The syntax is:

```csharp
	new NativePromise<TUno, TJavaScript>(name, futureFactory [, converter])
```

or

```csharp
	new NativePromise<TUno, TJavaScript>(name, resultFactory [, converter])
```

Where:

* `TUno` is the type the promise produces in Uno
* `TJavaScript` is the type the promise produces in JavaScript. If `converter` is omitted, TUno and TJavaScript must be the same type (string, numeric or boolean).
* `name` is the name of the member in the module
* `futureFactory` is a delegate that produces a `Future<TUno>` from an `object[]`, which is the set of arguments
   passed in from JavaScript (following the same convention as arguments to `NativeFunction`). Signature:

   `public delegate Future<TUno> FutureFactory(object[] args);`

* `resultFactory` is a delegate that produces a `TUno` from an `object[]`.

	`public delegate TUno ResultFactory(object[] args);`

* `converter` (optional) is a method that converts `TUno` to `TJavaScript` given a `Fuse.Scripting.Context`. Signature:

   `public delegate TJavaScript Converter(Context c, TUno value);`

The first version wraps an existing `Future<TUno>`, and optionally converts it to a corresponding JavaScript type.

The second version wraps a simple function that produces `TUno`. This function will be executed in a separate thread
automatically, and the result will be dispatched back to the JavaScript thread when done. This offers a simpler way
of implementing simple promises. To reject the promise, throw an `Exception` from the resultFactory.


Note that `Future<T>` and `Promise<T>` are Uno types currently defined in the `Experimental.Threading` package.

Below is an example of a very simple NativePromise module

```csharp
	using Fuse;
	using Fuse.Scripting;
	using Uno.UX;
	using Uno;

	[UXGlobalModule]
	public class PromiseExample : NativeModule
	{
		static readonly PromiseExample _instance;

		public PromiseExample()
		{
			if (_instance != null) return;
			
			_instance = this;
			Resource.SetGlobalKey(_instance, "PromiseExample");
			
			AddMember(new NativePromise<string, Fuse.Scripting.Object>("promiseMe", PromiseMe, Converter));
		}
		
		static string PromiseMe(object[] args)
		{
			if (args.Length != 1)
			{
				// This will reject the promise with the message of the exception
				throw new Exception("promiseMe() requires exactly 1 parameter.");
			}
			
			// This will resolve the promise
			return args[0] as string;
		}

		static Fuse.Scripting.Object Converter(Context context, string str)
		{
			var wrapperObject = context.NewObject();
			wrapperObject["foo"] = str;
			return wrapperObject;
		}
	}
```

We can use the above code in JavaScript like so:

```js
	var PromiseExample = require("PromiseExample");
	PromiseExample.promiseMe("bar")
		.then(function(result) {
			console.dir(result);
			// { foo: "bar" }
		});

	PromiseExample.promiseMe()
		.then(function() {
			// This will never happen
		}, function(err) {
			console.log("Error: " + err);
			// Error: promiseMe() requires exactly 1 parameter.
		})
```

## NativeEvent

Note: The recommended way to do native events is to use a @NativeEventEmitterModule, described above. `NativeEvent`s are an older feature that is being phased out.

Native events can be used to call back to JavaScript from Uno code. This is done by adding a `NativeEvent` to the `NativeModule`.
The `NativeEvent` can then be called using `yourNativeEvent.RaiseAsync(args)`.

The following example illustrates how you create a `NativeModule` that echoes any message sent to it using a `send` function and an `onMessageReceived` event.

```csharp
	using Fuse;
	using Fuse.Scripting;
	using Uno.UX;

	[UXGlobalModule]
	public class Chat : NativeModule
	{
		static readonly Chat _instance;

		NativeEvent _nativeEvent;

		public Chat()
		{
			if (_instance != null) return;

			_instance = this;
			Resource.SetGlobalKey(_instance, "Chat");

			AddMember(new NativeFunction("send", (NativeCallback)SendMessage));
			_nativeEvent = new NativeEvent("onMessageReceived");
			AddMember(_nativeEvent);
		}

		object SendMessage(Fuse.Scripting.Context context, object[] args)
		{
			var arg = args[0] as string;
			if (arg != null)
				_nativeEvent.RaiseAsync(arg);
			else
				_nativeEvent.RaiseAsync("----");

			return null;
		}
	}
```

The `chat` module can then be used from JavaScript like so:

```js
	var chat = require("Chat");

	function send() {
	    chat.send(new Date().toString());
	}

	chat.onMessageReceived = function(message) {
	    console.log("onMessageReceived " + message);
	};
```
