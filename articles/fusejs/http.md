# HTTP

## fetch()

`fetch` is the main way to do HTTP requests.

This is an example on how to take a JavaScript object, POST it as JSON and receive JSON data which is turned back into a JavaScript object using `fetch`:

```js
	var status = 0;
	var response_ok = false;

	fetch('http://example.com', {
		method: 'POST',
		headers: { "Content-type": "application/json"},
		body: JSON.stringify(requestObject)
	}).then(function(response) {
		status = response.status;  // Get the HTTP status code
		response_ok = response.ok; // Is response.status in the 200-range?
		return response.json();    // This returns a promise
	}).then(function(responseObject) {
		// Do something with the result
	}).catch(function(err) {
		// An error occurred somewhere in the Promise chain
	});
```

[Complete documentation for `fetch` can be found here](https://developer.mozilla.org/en-US/docs/Web/API/GlobalFetch/fetch)

## XMLHttpRequest

FuseJS supports `XMLHttpRequest`, [go here for more information](https://developer.mozilla.org/en-US/docs/Web/API/XMLHttpRequest).