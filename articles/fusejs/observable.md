# Observables

Observables are a key concept to understand in order to be effective with Fuse. Observables let us automatically update the UI whenever there are changes to the data-model. They are also extremely useful for describing view-models; which means mapping the data-model into a view friendly format. When [data-binding](api:fuse/reactive/databinding) to an `Observable` using the curly brace syntax (`{<some_observable>}`), a subscription is automatically created that will update the UI whenever the `Observable` changes. This article is a quick introduction to understanding and using the FuseJS/Observable API.

Here is a tiny example to give you an idea of what we'll be looking at:

```xml
<App>
	<JavaScript>
		var Observable = require("FuseJS/Observable");
		module.exports.myValue = Observable(1);
	</JavaScript>
	
	<Text Value="{myValue}" />
</App>
```

## Video tutorial

For a nice introduction to working with observables, take a look at this video:

<iframe width="560" height="315" src="https://www.youtube.com/embed/bB9P4mTGtVU" frameborder="0" allowfullscreen></iframe>

## Using observables

An `Observable` can hold a single value, or be treated as a list of values with 0 or more elements. When the value(s) of the `Observable` changes, all _subscribers_ are automatically notified. The concept of subscribers/subscriptions is discussed in detail later in this article.

Start off by importing the observable module, which returns a function:

	var Observable = require("FuseJS/Observable");

Observables are created by calling the `Observable` function directly with zero or more initial values.

	var emptyObservable = Observable();
	var singleValueObservable = Observable(true);
	var listObservable = Observable(1,2,3,4);

	var singleValueObservable = Observable(10); //now has .value == 10
	singleValueObservable.value = 20; //now has .value == 20
	var theValue = singleValueObservable.value; //theValue is now == 20
	
When using an observable as a list, we can add and remove items using the `.add` and `.remove` methods.

	var multiValueObservable = Observable(1,2);
	multiValueObservable.add(5); //now contains 1,2,5
	multiValueObservable.remove(2); //now contains 1,5
	
We can also apply various transformations to observables using a set of methods we collectively call "reactive operators". These operators are methods we can call on observables that return new observables. We cover them in more detail later in this article. The following example squares all the numbers in the `someNumbers` observable using the `.map` operator. Map works by converting all the items in an `Observable` from one form into an other using the supplied function.

	var someNumbers = Observable(1,2,3,4); //now contains 1,2,3,4
	var someNumbersSquared = someNumbers.map(function(x){ return x * x; }); //someNumbersSquared will eventually contain 1,4,9,16
	
Since all the reactive operators return new observables, we can put several of them after one another to create a long chain of reactive operations.

	var someNumbers = Observable(1,2,3,4);
	var someTransformedNumbers = someNumbers
		.map(function(x){ return x * x; })
		.where(function(x){ return x < 10; })
		.map(function(x){ return -x; }); //will eventually contain -1, -4, -9

## State Observables and derived Observables
	
When working with Observables, it is important to understand the difference between **state Observables** and **derived Observables**. A state Observable is an Observable that is explicitly _made by you_, while a derived Observable is returned by other APIs, such as [reactive operators](articles:fusejs/observable-api.md#reactive-operators).

One thing to note about the example above, that catches a lot of new Fusers off guard, is that `someTransformedNumbers` won't actually contain data immediately after the statement that declares it:

	var someNumbers = Observable(1,2,3,4); // someNumbers is a state Observable
	var someNumbersSquared = someNumbers.map(function(x){ return x * x; }); // someNumbersSquared is a derived Observable
	console.log("SquaredNumbers length: " + someNumbersSquared.length); // this will print 0
	
The difference between state Observables and derived Observables is explained in [Observable API docs](articles:fusejs/observable-api.md#state-observables-and-derived-observables). In short, derived Observables won't propagate data unless there is a subscriber at the end of the chain, while the data in state Observables is available synchronously. Keep reading to learn more about how subscriptions work.

## Subscribing to changes in Observables

A really important thing to understand about Observables is that they need at least one _subscriber_ before they start propagating data. There are a few ways to subscribe to an Observable:

### Data-bind to it from UX Markup

Whenever we data-bind to an `Observable` from UX using the curly brace syntax, we automatically create a subscription to it. This is the most common way of creating subscriptions. For more information about data-binding, take a look at [this article](api:fuse/reactive/databinding).

### .onValueChanged(module, func(item))

In some cases, we're interested in running some imperative code in response to an `Observable` changing. We can do this by subscribing using the `.onValueChanged(module, func(item))` function, which fires the function `func` every time the value of a single value Observable changes. The module parameter lets us connect the lifetime of this subscription to the lifetime of a module. In most cases we just pass the module we're currently in:

```js
var myObservable = Observable(1);
myObservable.onValueChanged(module, function(item) {
	//do something
});
```

### .addSubscriber(func)

We can also explicitly add subscriptions by using the `.addSubscriber(func(item))` method. The drawback of using `.addSubscriber` is that we manually have to remove the subscription when we no longer need it using `.removeSubscriber`. Always prefer using `.onValueChanged` unless we're defining custom reactive operators. We mention it here only for completeness.

### .subscribe(module)

Lastly, there are rare cases when we need to force an Observable to be calculated without data-binding to it from UX Markup. `.subscribe(module)` is similar to `.onValueChanged(module, func(item))` in that it adds a subscription which is tied to the lifetime of a module. Note that it only creates this subscription and does not accept a function as second argument.

### Reactive operators

Reactive operators are methods we can call on observables that produce new observables. These can then be subscribed to or transformed further by additional applications of reactive operators. An important thing to know about these methods, is that they work declaratively. Applying a reactive operator to an `Observable` doesn't actually apply the transformation right away; it merely sets up a "pipeline" that the values will flow through when they are supplied at the source `Observable`.

We say that observables uses a "push based" model. The alternative to a push based model is one that is "pull based". In this scenario, the UI will have to ask its data-source for the latest data when it wants to update itself. In the push based approach, the data-model will push its values towards the UI whenever they change.

A thing to be aware of with this "push based" approach, is that values won't actually be pushed through unless someone has subscribed to be notified of these pushes, as mentioned earlier.

#### What do we mean by an Observable producing values

We usually think of observables as streams of values flowing from a source (or a set of sources) to a destination (or set of destinations). Along the way, the stream can be transformed, filtered and combined with other streams by using reactive operators like `.map`, `.where` and `.combine` (find information about all the operators [here](articles:fusejs/observable-api.md). Here is an example:

```js
var source = Observable(2);

var destination = source.map(function(x) {
	return x * x;
}).where(function(x) {
	return x < 50;
});
```

In the code above, we create a source `Observable` with a single initial value; the number 2. We then use the reactive operator `.map` to transform the value with a "mapping-function" that squares the number. Lastly, we use the reactive operator `.where` to filter the observable so that only values that are below 50 will be propagated further. The `.where` operator is expected to return either `true` or `false`; `true` if we allow the value to propagate, `false` if not.

## Passing observables through properties

Observables can also be passed into custom made components using Properties. We can add a property which accepts Observables by making an `object` property:

```xml
<Panel ux:Class="CoolPanel">
	<object ux:Property="ObservableProperty" />
	<JavaScript>
		var passedInObservable = this.ObservableProperty.inner();
	</JavaScript>
</Panel>
<JavaScript>
	var Observable = require("FuseJS/Observable");
	module.exports = {
		valueToPass: Observable("123")
	};
</JavaScript>
<CoolPanel ObservableProperty="{valueToPass}" />
```

In most cases you'll want to use `inner()` when fetching an Observable passed in through properties. This is because the javascript value `this.Propertyname` is an observable with whatever `Propertyname` contains. If we pass an Observable in, `this.Propertyname` will contain an observable with the observable we passed through.
