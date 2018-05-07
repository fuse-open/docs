### Asynchronous Map

There are cases where you need to asynchronously fetch data in response to the value of an `Observable` changing without exiting the `Observable` domain.
We can achieve this in an elegant way using a combination of `map()` and `inner()`.

In the below example, we [map](/observable-api#map-func-item) over each item (just one in this case) of our input Observable, returning an Observable which will be updated to contain our fetched data when it becomes available.
This will result in an Observable of an Observable of our future data – *almost* what we want.
We therefore use [inner()](#inner) to "unwrap" the inner Observable in place of the outer one, and voila!
We end up with an Observable that will be updated with our asynchronously fetched data whenever the input changes.

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
	
	
## Using Observables to define view-models

Since the Observable API easily lets us do complex transformations on our data, it is a great way to define view-models. A view-model is a mapping from our original data-model, to one that is more suitable to be displayed on a particular screen. This lets us view the same data in many different ways, without having to store it differently.



### Observable condition

In order to filter a list based on an observable condition you use a combination of `.flatMap` and `.where`.

	var items = conditionObservable.flatMap(function(v) {
		return itemsObservable.where(function(x) { return v != x; });
	});
	
