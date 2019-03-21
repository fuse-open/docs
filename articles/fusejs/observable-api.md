# Observable API

#### Observable value

If the observable holds exactly one value, we can get or set that value using the `.value` property:

```js
	var someString = Observable("foobar");
	console.log(someString.value); // prints "foobar"
	someString.value = "barfoo"; // sets the value, notifies subscribers.
```

When the `.value` property is set, all subscribers are notified.

* Empty observables will have `.value === undefined` and `.length === 0`.

#### Observable lists

If we want to use the observable as a list of values, we manipulate it using methods like `.add(item)` and `.remove(item)`.
We can also query the number of values in the list through the `.length` property:

	var friends = Observable("Jake", "Jane", "Joe");
	console.log(friends.length); // prints 3

	friends.add("Gina");
	console.log(friends.length); // prints 4

See the full list of members to see what's possible with observable lists.

#### Data types with Observables

Observables can be used to supply all the basic types; number, string, boolean, as well as vector types. Number, string and boolean are created using their usual JavaScript literals:

```js
var obsNumber = Observable(10.5);
var obsString = Observable("hello");
var obsBool = Observable(true);
```

For data-bindings requiring vector types (e.g. colors), we can use JavaScript arrays:

```js
var obsRedColor = Observable([1,0,0,1]);
var obsWhiteAndBlack = Observable([1,1,1,1], [0,0,0,1]);
```

#### Observable functions

When an `Observable` is initialized with a function as its only argument, the `.value`
of the observable is computed by evaluating the function.

Reactive dependencies are automatically generated with all other observables touched while
evaluating the function.

Example:

```js
	var firstName = Observable("John");
	var lastName = Observable("Doe");

	var fullName = Observable(function() {
		return firstName.value + " " + lastName.value;
	})
```

If the `firstName` or `lastName` changes, the `fullName` will now update automatically.

Yes, it's magic.

### State Observables and derived Observables

When working with Observables, it is important to understand the difference between **state Observables** and **derived Observables**. In short, a state Observable is an Observable that is explicitly _made by you_, while a derived Observable is returned by other APIs, such as [reactive operators](articles:fusejs/observable-api.md#reactive-operators).

```js
	var stateObs = Observable("foo"); // stateObs is a state Observable
	var derivedObs = stateObs.map(function(val) { // derivedObs is a derived Observable
		return "derived: " + val;
	});
```

The main difference between the two is that the data in state Observables is available synchronously. It is not necessary to add a subscriber for the data to be available, and we can access it directly:

```js
	var list = Observable(1,2,3,4,5);
	list.forEach(function(i) {
		console.log(i); // loops through the list, prints 1, then 2, 3, 4, 5
	});
```

The data in derived Observables is not available synchronously, so we cannot use a synchronous approach like shown in the example above. Instead, we use [reactive operators](articles:fusejs/observable-api.md#reactive-operators) (such as `.map()`) to react when data eventually becomes available:

```js
	var list = Observable(1,2,3,4,5);
	var asyncList = list.map(function(i) {
		return i * i;
	});
```

In the example above, the `asyncList` derived Observable will eventually contain the squared values for the numbers in `list` state Observable. Note that we cannot iterate over `asyncList` with a `forEach()` statement here, because the data in `asyncList` is populated asynchronously, and only when there is a subscriber added to it. Later in this article, you'll find more details on [subscribing to updates](articles:fusejs/observable-api.md#subscribing-to-updates).

[Reactive operators](articles:fusejs/observable-api.md#reactive-operators) are just one example of APIs that return derived Observables.
A very common use case are properties of `ux:Class` defined using the [ux:Property attribute](articles:ux-markup/properties#using-properties-in-javascript), which are automatically available in JavaScript as derived Observables. Another frequent use case is the Navigation-related `this.Parameter` which we encounter when [passing parameters to pages](articles:navigation/navigation#passing-parameters-to-pages). Just like with `ux:Property`, `this.Parameter` also is an implicit derived Observable, the value of which we can only access using [reactive operators](articles:fusejs/observable-api.md#reactive-operators).

### Synchronous operators

The following is a list of operators that are only appropriate to be used on state Observables, as explained [in the section above](articles:fusejs/observable-api.md#state-observables-and-derived-observables). If needed, some of them could be used on derived Observables too, but only from within [Observable functions](articles:fusejs/observable-api.md#observable-functions).

#### value

Gets or sets the current value of the @Observable.

The `value` property acts as a shorthand for `getAt(0)` and `replaceAt(0)`.
It is most often used with single value Observables, although this is not a requirement.

```js
	if (isSomethingEnabled.value) {
		doSomething();
	}
	isSomethingEnabled.value = false;
```

#### toArray()

Returns a shallow copy of the Observables internal values array.

```js
	var obs = Observable(1,2,3);
	var obsArray = obs.toArray(); //obsArray == [1,2,3]
```

### List operators

#### add(value)

Adds `value` to the observable list of values.

```js
	var colors = Observable("Red", "Green");
	colors.add("Blue");
```

#### addAll(items)

Appends the array `items` to the end of the observable.

```js
	var val = new Observable(1, 2, 3);

	val.addAll([4, 5, 6]);
	
	//val = Observable(1, 2, 3, 4, 5, 6);
```

#### clear()

Removes all values from the @Observable.

```js
	var colors = Observable("Red", "Green");
	colors.clear();
```

#### contains(value)

Returns true if `value` exists in the `var`.

```js
	Observable seasons = Observable("Summer", "Fall", "Winter", "Spring");
	var winterExists = seasons.contains("Winter"); // true
```

#### forEach(func(item))

Invokes `func` on every value in the @Observable.

```js
	var numbers = Observable(10, 2, 50, 3);
	numbers.forEach(function(number) {
		console.log(number + " is a nice number!");
	});
```

#### forEach(func(item,index))

Invokes `func` on every value in the @Observable.

```js
	var numbers = Observable(10, 2, 50, 3);
	numbers.forEach(function(number, index) {
		console.log(number + " has the index: " + index);
	});
```

If `func` accepts two arguments, the second argument is the index of that item in the @Observable.

#### getAt(index)

Returns the value at the given `index`

```js
	var seasons = Observable("Summer", "Fall", "Winter", "Spring");
	console.log(seasons.getAt(2)); //output: "Winter"
```

#### identity()

Equivalent to calling `map` with the identity function, aka `map(function(x) { return x; })`.

Useful in cases where two-way databinding is necessary, but we don't want to clobber the original `Observable`'s data.

```js
	var originalData = Observable("This is my original data");
	var nonClobberedData = originalData.identity(); // Safe for two-way databinding
```
#### indexOf(value)

Returns the index of the first occurrence of `value`.

```js
	var seasons = Observable("Summer", "Fall", "Winter", "Spring");
	var index = seasons.indexOf("Winter"); // 2
```

#### insertAll(index, array)

Inserts the contents of `array` at the specified `index`.

```js
	var clouds = Observable("Cirrus", "Alto", "Stratus");
	clouds.insertAll(1, ["Cumulus", "Mammatus"]);
	
	//clouds = Observable("Cirrus", "Cumulus", "Mammatus", "Alto", "Stratus");
```

#### insertAt(index, value)

Inserts `value` at the specified `index`.

```js
	var words = Observable("foo", "bar");
	words.insertAt(1, "baz");
	
	console.log(words); // Observable("foo", "baz", "bar")
```

#### .length

Returns the number of values in the @Observable

```js
	var fruits = Observable("Orange", "Apple", "Pear");
	console.log(fruits.length); //output: 3
```

#### refreshAll(newValues, compareFunc, updateFunc, mapFunc)

Updates all items in the @Observable with the values from `newValues`.
`compareFunc` is used to determine whether two items are equal. `updateFunc` is used to update an existing item with new values when a match is found by `compareFunc`.
`mapFunc` is called whenever a new item is found and allows it to be mapped to a new object.

```js
	var items = Observable(
		{id: 1, text: "one" },
		{id: 2, text: "two" },
		{id: 3, text: "tres" }
	);
	
	var newItems = [
		{id: 3, text: "three" },
		{id: 4, text: "four" },
		{id: 5, text: "five" }
	];
	
	items.refreshAll(newItems,
		//Compare on ID
		function(oldItem, newItem){
			return oldItem.id == newItem.id;
		},
		// Update text
		function(oldItem, newItem){
			oldItem.text.value = newItem.text;
		},
		// Map to object with an observable version of text
		function(newItem){
			return {
				id: newItem.id,
				text: Observable(newItem.text)
			};
		}
	);
```

#### remove(value)

Removes the first occurrence of `value` from the observable list of values.

```js
	var shapes = Observable("Round", "Square", "Rectangular");
	shapes.remove("Rectangular");
```

#### removeAt(index)

Remove the value at the given `index`.

```js
	var shapes = Observable("Round", "Square", "Rectangular");
	shapes.removeAt(2);
```

#### removeRange(start, count)

Removes `count` items, starting from `start`.

```js
	var letters = Observable("a", "b", "c", "d");
	letters.removeRange(1, 2);
	//letters = Observable("a", "d");
```

#### removeWhere(func)

Removes all values for which `func` is true.

```js
	var hotPlaces = Observable(
		{name: "Oslo", temperature: 30},
		{name: "New York", temperature: 24},
		{name: "California", temperature: 27},
		{name: "Sydney", temperature: 10}
	).removeWhere(function(place){
		return place.temperature < 20;
	}); //Removes Sydney from the list
```

#### replaceAll(array)

Replaces the Observables values with values from the `array`.

```js
	var colors = Observable("Red", "Green", "Blue");
	colors.replaceAll(["Orange", "Cyan", "Pink"]);
```

#### replaceAt(index, value)

Replaces the value at `index` with `value`.

```js
	var ingredients = Observable("sugar", "milk", "potato");
	ingredients.replaceAt(2, "flour"); //Replaces "potato" with "flour"
```

#### tryRemove(value)

Tries to remove the first occurrence of `value` from the observable list of values.
Returns true if successful, and false otherwise.

```js
	var shapes = Observable("Round", "Square", "Rectangular");
	if(shapes.tryRemove("Rectangular")) {
		console.log("success");
	}
```

### Reactive operators

FuseJS comes with set of reactive operators that return derived Observables from other Observables. This means that if the original 
`Observable` changes, any `Observable` that is created as a result of applying a reactive operator will also change automatically. These operators can be used on both state Observables and derived Observables, as explained [earlier in this article](articles:fusejs/observable-api.md#state-observables-and-derived-observables).

Most operators are one-way: a change in the derived Observable will not change the source observable (in fact we should not modify the derived Observable in this case). The `...TwoWay` functions create a two-way binding: a change in the derived Observable will change the source observable.

_Be aware that many of the operators work with objects, as well as values. If you modify the properties of an object in the derived Observable you might be updating the source as well, since they are the same object. It's also possible that your source contains a list of `Observable` objects, in which case changes to them will propagate as normal from that `Observable`. One-way/Two-way refers only to which directions changes in the high level observable, made by the operator, are propagated._

#### any(filter)

Returns a new @Observable containing a boolean representing whether or not the observable it is called on contains any entries that match the `filter`.

```js
	var vehicles = Observable(
		{type: "car", name: "SuperSpeeder 2000"},
		{type: "car", name: "Initial Dash 2k00"},
		{type: "boat", name: "Floaty McFloatface"}
	);
	
	var hasBoats = vehicles.any({type: "boat"}); //true
	var hasAircraft = vehicles.any({type: "aircraft"}); //false
	var hasCar = veichles.any(function(x) { return x.type === "car"; }) //true
```



#### combine([obs, ...], func)

Invokes `func` every time the current observable or any of the `obs` observables (collectively "the dependencies") change. 

The arguments will hold the current `.value` of each of the dependencies, with `this.value` as the first argument, followed by the value of each successive observable in order.

```js
	var foo = Observable(1);
	var boo = Observable(2);
	var moo = Observable(3);

	var res = foo.combine(boo, moo, function(f, b, m) {
		// f holds the current .value of foo
		// b holds the current .value of boo
		// m holds the current .value of moo

		return f+b+m; // the resulting observable will yield the value 6
	})
```

If the `func` provided to `combine` returns a value, that value will replace the `.value` of the resulting observable. If the function returns nothing (`undefined`),
the `func` is expected to take care of updating the resulting observable by modifying the `this` parameter in the `func`.

Note that `combine` fires on every change, even if no value is available for each of the dependencies. To avoid that, see `combineLatest`.

Note that `combine` only provides the first value (`.value`) of each of the dependencies, even if some of these are arrays. To get all the values in each
of the dependencies that, see `.combineArrays`.

### combineLatest([obs, ...], func)

Same as `combine`, but does not fire `func` until at least one value is available (`.length > 0`) for all of the dependencies.

### combineArrays([obs, ...], func)

Same as `combine`, but provides an array for each dependency with all the values of each of the observable instead of just the first value of each observable.

```js
	var foo = Observable(1);
	var boo = Observable("a", "b", c);
	var moo = Observable(3);

	var res = foo.combineArrays(boo, moo, function(f, b, m) {
		// f holds [1]
		// b holds ["a", "b", "c"]
		// m holds [3]

		return [f[0], m[0], b.length]; // the resulting observable will hold the values (1, 3, 3)
	})
```

If the `func` provided to `combineArrays` returns an array, the elements from the array will replace all elements in the resulting observable. If the function returns nothing (`undefined`),
the `func` is expected to take care of updating the resulting observable by modifying the `this` parameter in the `func`.

#### count()

Returns the number of values in the @Observable as an observable number. Whenever an item is added or removed from the @Observable, the `count` changes.

```js
	books = Observable(
		"UX and you",
		"Observing the observer",
		"Documenting the documenter"
	);
	
	numBooks = books.count(); //result: 3
```

#### count(condition)

If `condition` is a function, it returns an observable number of values for which `condition` is true. If `condition` is an object, it returns an observable containing the amount of values whom properties equal properties in `condition`.

```js
	var tasks = Observable(
		{ title: "Learn Fuse", isDone: true },
		{ title: "Learn about Observables", isDone: true },
		{ title: "Make awesome app", isDone: false }
	);
	var tasksDone = tasks.count(function(x){
		return x.isDone;
	}); // 2
	var tasksDone = tasks.count({isDone: true}); //2
```

#### expand(func)

When an @Observable contains only a single array, expand will return an @Observable containing the values from that array.

```js
Observable([1,2,3]).expand() -> Observable(1,2,3)
```

#### filter(condition)

Returns an observable that will only propagate values that pass the given `condition`, otherwise it retains its previous value.

This method only considers the first (single) value of an observable.

<!-- TODO: Make example -->
	
#### first()

Returns a new @Observable containing the value of the first entry in the observable it is called on.

```js
	var values = Observable(1, 2, 3);
	var firstEntry = values.first(); //Observable(1)
```

#### flatMap(func(item))

Calls `func` for every item in the @(Observable), then merges the items of all the `func` calls into one @(Observable) array.

```js
	var numbers = Observable(
		[1, 2, 3],
		[4, 5, 6]
	);
	var counts = numbers.flatMap(function(item) {
		return [item, item+1];
	});
	//counts == Observable([1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7])
```

#### inner()

Returns a new Observable that reflects the inner value when two Observables are nested (the `.value` of the Observable is an Observable).
If the Observable it is called on is _not_ nested then `.inner()` simply reflects its value.

If the current Observable is not nested, the returned observable reflects the value of the current observable.

```js
	var foo = Observable(Observable(4))
	var bar = foo.inner(); // bar.value = 4
	foo.value.value = 9;   // bar.value = 9
	foo.value = 3;         // bar.value = 3
```

This is particularly useful when dealing with Observables passed via `ux:Property`.

#### innerTwoWay()

A version of `inner()` that creates a two-way binding with the output. Values modified in the returned @Observable will update the value in the source observable.

```js
	var outerObs = Observable(Observable("John"));
	var innerObs = outerObs.innerTwoWay();
	innerObs.value = "Jake"; // outerObs.value.value = "Jake"
```

`.innerTwoWay()` works on both single-value Observables and on Observable lists.

```js
	var outerObs = Observable(Observable("1","3","5"));
	var innerObs = outerObs.innerTwoWay();
	innerObs.removeAt(0); // outerObs.value.toArray() = ["3","5"]
```

#### last()

Returns a new @Observable containing the value of the last entry in the observable it is called on.

```js
	var values = Observable(1, 2, 3);
	var lastEntry = values.last(); //Observable(3)
```

#### map(func(item)))

Invokes `func` on every value in the @Observable returning a new @Observable with the results.

```js
	var numbers = Observable(1, 4, 9);
	var roots = numbers.map(function(number) {
		return Math.sqrt(number);
	});
```

The values of `roots` becomes the square root of the numbers in `numbers`. The values in `numbers` remain unchanged.

#### map(func(item, index)))

Invokes `func` on every value in the @Observable returning a new @Observable with the results.

```js
	var numbers = Observable("this", "item", "is");
	var roots = numbers.map(function(item, index) {
		return item + " has the index nr: " + index;
	});
```

When Observable.map is used with a function which takes two arguments, the second argument is the index of that item in the @Observable.

#### mapTwoWay(mapFunc(sourceValue), unmapFunc(partValue,sourceValue))

A version of map that creates a two-way binding with the output. Values modified in the returned @Observable will update the value in the source observable.

`mapFunc(sourceValue)` converts a value from source observable: it returns the mapped value for the output.

`unmapFunc(partValue, sourceValue)` converts a value form the output back to the form for the input. The current  `sourceValue` is provided in case the output was only a partial value.

Here is an example for  simple mapping to convert radians to degrees and back (this example does not need the `sourceValue`):

```js
	var angles = Observable(0, Math.PI, 2*Math.PI)
	exports.angleDegrees = angles.mapTwoWay( function(value) {
		return value * 180 / Math.PI 
	}, function( value, sourceValue ) {
		return value * Math.PI / 180 
	})
```

Here is an example the returns a field in an object and recreates the full object in the `umapFunc`. If this is all you're doing then use `pickTwoWay` instead, which does exactly this (in this example it would be `users.pickTwoWay("name")`.

```js
	var users = Observable(
		{ id: "tom", name: "Tommy" },
		{ id: "sal", name: "Sally" }
	)
	
	exports.names = users.mapTwoWay( function(user) {
		return user.name
	}, function( name, sourceUser ) {
		sourceUser.name = name
		return sourceUser
	})
```


#### not()

Returns an @Observable that has the inverse value of the @Observable you are calling it on. If the @Observable is `true`, the returned one will be `false`, and vice versa.

```js
	falseValue = Observable(false);
	trueValue = falseValue.not();
```

#### pick(index)

Returns a new @Observable containing the item at the index, or named property, `index` of every entry in the observable it is called on. 

```js
	var values = Observable([1, 2], [3, 4], [5, 6]);
	var picked_values = values.pick(1); //Observable(2, 4, 6);
```

#### pickTwoWay(index)

Like `pick`, but creates a two-way relationship. Modifications to the output map will update the value in the source map.

#### slice([begin[, end]])

Returns a new observable that reflects a slice of the current observable.

See the <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/Array/slice">documentation for `Array.slice()`</a> for explaination 
of the arguments to this function.

```js
	var foo = Observable(1,2,3,4);
	var bar = foo.slice(1,2) // bar = Observable[2, 3]
```

Hints:

* To observe the *first* element of an observable, use `.slice(0, 1)`. 
* To observe the *last* element of an observable, use `.slice(-1)`.

#### where(condition)

`condition` can be either a function or an object.
If `condition` is a function, it will return a new @Observable with only the values for which `condition` returns true.
However, if `condition` is an object, it will return a new @Observable with only the values where parameters match parameters in `condition`, checked using strict equality (`===`).

The new @Observable observes the old @Observable, and will therefore update whenever a value changes in the original observable.

```js
	fruits = Observable(
		{ name: "Apple" , color: "red"    },
		{ name: "Lemon" , color: "yellow" },
		{ name: "Pear"  , color: "green"  },
		{ name: "Banana", color: "yellow" });

	goodFruits = fruits.where({color: "yellow"});
	
	goodFruits = fruits.where(function(e){
		return e.color === "yellow";
	});
```

> Note! You used to be able to return an Observable function with the .where operator in order to make the condition itself observable.
> This is no longer allowed. The best practice for achieving the same effect is to instead use [flatMap()](#flatmap-func-item).

In order to filter a list based on an observable condition you use a combination of `.flatMap` and `.where`.

```js
	var items = conditionObservable.flatMap(function(v) {
		return itemsObservable.where(function(x) { return v != x; });
	});
```

We use `.flatMap()` instead of `.map()` here because we return observables from the mapping function.

> Note! The above approach is fast, but has one caveat: it does not conserve the items order in the observable in the case where the observable condition changes. This means that the `items` Observable effectively gets cleared and filled from scratch. There is a second pattern for observing a condition with `.where` which does not have this problem, but it can potentially be quite slow on large lists. It is included here for completeness:

```js
var filteredItems = conditionObservable.flatMap(function(cond){
	return items.where(function(item){
		return cond.value;
	});
});
```

Take a look [here](https://github.com/fusetools/fuse-samples/tree/master/Samples/FilterOnObservableCondition) for a complete sample.

This lets you create an Observable which pushes changes whenever the condition or the data changes.

### Subscribing to updates

#### onValueChanged(module,func)

To manually react to changes in a single value @Observable, we can use the `onValueChanged` method. It automatically creates a subscription for us and ties its lifetime to that of `module`.
`func` will be called whenever the single value @Observable changes.

```js
	someObservable.onValueChanged(module, function(x) {
		console.log("We got a new value: " + x);
	});
```

#### addSubscriber(func)

Lets you manually create a subscription for an @Observable. Note that you have to manually remove the subscription using `removeSubscriber` at the appropriate time. For that reason, you should almost always prefer using `onValueChanged` unless you are implementing a custom observable operator.

#### removeSubscriber(func)

When you are done consuming the values from the Observable it is important to clean it up by removing the subscription. If we forget to do this we risk accumulating memory garbage over time. This is only needed if you manually created a subscription using `addSubscriber`. Remember that `onValueChanged` handles this cleanup automatically for us.

```js
	username.removeSubscriber(usernameLogger);
```


### .subscribe(module)

When you only need to use an @Observable in JavaScript and data-binding to it from UX isn't desired, you can use `.subscribe(module)` to create a subscription that is tied to the lifetime of a module. This is similar to `onValueChanged`, except we don't have the possibility to specify a callback function.

```js
	someObservable.subscribe(module);
```

### Asynchronous data fetching with Observables

There are cases where you need to asynchronously request some data in response to the value of an `Observable` changing, and get the result as another Observable.
We can achieve this in an elegant way using a combination of `map()` and `inner()`.

In the below example, we [map](#map-func-item) over each item (just one in this case) of our input Observable, returning an Observable which will be updated to contain our fetched data when it becomes available.
This will result in an Observable of an Observable of our future data – *almost* what we want.
We therefore use [inner()](#inner) to "unwrap" the inner Observable in place of the outer one, and voila!
We end up with an Observable that will be updated with our asynchronously fetched data whenever the input changes.

```js
	var inputUrl = Observable("https://example.com/");
	
	var output = inputUrl.map(function(url) {
	    var resultObservable = Observable("Placeholder value");
	    
	    fetch(url)
	        .then(function(response) { return response.text(); })
	        .then(function(result) {
	            resultObservable.value = result;
	        });
	    
	    return resultObservable;
	}).inner();
```

### Failures

An Observables may be in a failed state. This means that it cannot provide a value, and that it's current value has been cleared. Observables are marked as failed when their ultimate provider, such as a network request, is unable to provide a value.

The failed status is cleared as soon as a new value is written to the observable (this includes a call to `clear`).

#### failed(message)

Marks the Observable as failed. The `message` should be a string giving some textual information about what has failed.

#### getFailure()

Returns the current failure message. This will be `undefined` if the Observable is not in a failed state.

#### isFailed([obs, ...])

Returns a new Observable containg a boolean value that is `true` if the source observable is failed.

You may provide any number of additional Observables as arguments. The boolean value will be `true` whenever any of these observables, including the source object, is failed.

#### failedMap(failedMapFunc [, notFailedMapFunc ])

Returns a new Observable that maps a failed state to a new value.

If the source observable fails then `failedMapFunc(message)` will be called. The return value will be used as the value of the output Observable.

If the source observable is not failed, or recovers from a failed state, then `notFailedMapFunc()` will be called. The return value will be used as the value of the output Observable. If this function is not provided, or it returns `undefined`, then the output Observable will be cleared.

#### onFailed(module, onFailedCallback [, onFailedResolvedCallback])

To imperatively react to a failure state we can use the `onFailed` function. It automatically creates a subscription for us and ties its lifetime to that of `module`.

`onFailedCallback(message)` will be called whenver the Observable fails. The `message` is the one provided to the `failed` function.

`onFailedResolvedCallback` will be called whenver the Observable has its failed status reset.

When you called `onFailed` one of the two functions, depending on the current state, will be called.


### Other

<!-- TODO: write doc for depend, I don't know what this do [karsten]
#### $(Observable.depend:depend())
-->

#### toString()

Returns a string representation of an observable and its contents.

```js
	var testObservable = Observable(1, "two", "3");
	testObservable.toString(); // "(observable) 1,two,3"
```