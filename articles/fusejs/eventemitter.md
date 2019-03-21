## EventEmitters

`EventEmitter` is a class that allows us to emit and listen to events.
Many of Fuse's JavaScript modules are instances of the `EventEmitter` class,
meaning that we can use the methods defined in this module to perform
operations on the events that the module defines.

### How to import

```js
	var EventEmitter = require("FuseJS/EventEmitter");
```

This imports `EventEmitter` so we can create our own instances of the class.

### Basic use

The following code creates an `EventEmitter` with two events, adds a listener to
one of the events, and subsequently emits that event.

```js
	var myEmitter = new EventEmitter("myEvent1", "myEvent2");

	myEmitter.on("myEvent1", function(arg) {
		console.log("myEvent1 fired with " + arg);
	});

	myEmitter.emit("myEvent1", "an arg");
```

This code will print `"myEvent1 fired with an arg"`.

#### Creating Observables from events

Sometimes events represent values that change over time. When this is the case
it can be more convenient to use an @Observable instead of ordinary callbacks.
`EventEmitter`s make this easy:

```js
	var obs = myEmitter.observe("myEvent");
```

Any time `"myEvent"` is emitted, `obs` will be updated to contain the arguments
passed to the event. When there are multiple arguments `obs` will be an
observable list.

#### Once listeners

The following code adds a listener to `"myEvent"` that will only be fired once:

```js
	myEmitter.once("myEvent", function() { console.log("This will be called at most once"); });
```

#### Creating promises from events

An alternative to using `once` listeners is to use `promiseOf`, which creates a
JavaScript Promise from an event name:

```js
	var prom = myEmitter.promiseOf("myEvent");
	prom.then(function(args) {
		// "myEvent" was emitted with `args`
	}).catch(function rejected(reason) {
		// an "error" event was emitted with `reason`
	});
```

The `promiseOf` method additionally listens to the `"error"` event (see below),
and rejects the promise if it's emitted.

#### Error handling

All `EventEmitter`s have a special built-in event called `"error"`:

```js
	myEmitter.emit("error", "an error occurred :(");
```

If `"error"` doesn't have any listeners when emitted it will throw an
exception. Other than that, `"error"` works like an ordinary event.

### Members

#### EventEmitter([...eventNames]) constructor

Constructs a new `EventEmitter` that permits the given argument names.

```js
	var myEmitter = new EventEmitter("anEventName", "anotherEventName");
```

#### on(eventName, func([...eventArgs]))

Adds `func` as a listener to the event `eventName`.

```js
	myEmitter.on("anEventName", function(arg) {
		console.log("anEventName fired with " + arg);
	});
```

#### once(eventName, func([...eventArgs]))

Adds `func` as a once listener to the event `eventName`, which means that
`func` will only be called the first time the event is emitted after adding the
listener.

```js
	myEmitter.once("myEvent1", function(arg) {
		console.log("myEvent1 fired with " + arg);
	});
```

#### removeListener(eventName, func([...eventArgs]))

Removes `func` from the listeners of `eventName`.

```js
	var listener = function() { console.log("Hello"); };

	myEmitter.on("anEventName", listener);
	// listener will be called on "anEventName" here
	myEmitter.removeListener("anEventName", listener);
	// listener will no longer be called on "anEventName"
```

#### emit(eventName, [...eventArgs])

Triggers `eventName`, calling all its listeners with `eventArgs`.

```js
	myEmitter.on("anEvent", function(arg) {
		console.log("anEvent fired with " + arg);
	});

	myEmitter.emit("anEvent", "an arg");
```

#### eventNames()

Returns an array of all the permitted event names that this `EventEmitter`
supports, including built-ins.

```js
	var myEmitter = new EventEmitter("a", "b", "c");
	var eventNames = myEmitter.eventNames();

	// eventNames contains "a", "b", "c", "error", "newListener", and "removeListener"
```

#### observe(eventName)

Creates an observable that listens to `eventName`.

```js
	var obs = myEmitter.observe("anEvent");
```

#### promiseOf(eventName)

Creates a promise that listens to `eventName`, and `"error"`.

```js
	var prom = myEmitter.promiseOf("anEvent");
	prom.then(function(args) {
		// "anEvent" was emitted with `args`
	}).catch(function rejected(reason) {
		// an "error" event was emitted with `reason`
	});
```

### Advanced members

The following functions are not necessary for basic usage but are provided for
compatibility and advanced usage scenarios.

#### addListener(eventName, func([...eventArgs]))

A synonym for `on`.

```js
	myEmitter.addListener("anEvent", function(arg) {
		console.log("anEvent emitted")
	});
```

#### prependListener(eventName, func([...eventArgs]))

A version of `on` that causes `func` to be first in the list of listeners for
`eventName`. This means that (barring later calls to `prependListener` or
`prependOnceListener`) `func` will be called _first_ when `eventName` is
emitted.

```js
	myEmitter.prependListener("anEvent", function(arg) {
		console.log("This will be called first when anEvent is emitted");
	});
```

#### prependOnceListener(eventName, func([...eventArgs])

A version of `once` that causes `func` to be first in the list of listeners for
`eventName`. This means that (barring later calls to `prependListener` or
`prependOnceListener`) `func` will be called _first_ when `eventName` is
emitted.

```js
	myEmitter.prependOnceListener("anEvent", function(arg) {
		console.log("This will be called first, and at most once, when anEvent is emitted");
	});
```

#### registerEvent(eventName)

Add `eventName` to the set of permitted events that this `EventEmitter` supports.

```js
	myEmitter.registerEvent("newEventName");
```

#### removeAllListeners([eventName])

Called with an argument, removes all listeners listening to `eventName`:

```js
	myEmitter.removeAllListeners("anEvent");
```

Called without an argument, removes all listeners listening to all events:

```js
	myEmitter.removeAllListeners();
```

This method should normally not be used as it will affect any other parts of
the code that listen to the `EventEmitter`.

### Built-in event names

In addition to the `"error"` event mentioned above,
all instances of `EventEmitter` provide the following events:

#### "newListener"(eventName, listener)

Triggered when a new listener is added to the `EventEmitter`.

```js
	myEmitter.on("newListener", function(eventName, listener) {
		console.log("listener added to " + eventName);
	});
```

#### "removeListener"(eventName, listener)

Triggered when a listener is removed from the `EventEmitter`.

```js
	myEmitter.on("removeListener", function(eventName, listener) {
		console.log("listener removed from " + eventName);
	});
```
