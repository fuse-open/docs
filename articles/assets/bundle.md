# Bundled files

When you build your project or save a file while running preview, the compiler will scan your UX files for referenced files and copy those over to the app bundle.
You may however have files that are only accessed by JavaScript – such as other JavaScript modules, static JSON data, or any other type of file that isn't used directly in UX.
These will not be automatically copied, as the compiler has no way of knowing whether to include them or not.

Bundled files solve this problem by letting you explicitly define files to be included in your uno project file.
It also exposes a JavaScript API that lets you read these files from JavaScript.

To include a bundled file, add it to the `Includes` section of your `.unoproj` in the following format:

```
<filename or glob pattern>:Bundle
```

Here is an example that will include all JavaScript files in the `js/` directory, as well as the file `dogs.json` as bundled files.


```
"Includes": [
	...other includes...
	
	"dogs.json:Bundle",
	"js/*.js:Bundle"
]
```

We can now `require()` bundled JavaScript modules:

```js
var foo = require("./js/foo.js");

foo.bar();
```

We can also read the contents of a bundled file to a string. The below example loads JSON from a bundled file and parses it.

```js
var Bundle = require("FuseJS/Bundle");

Bundle.read("dogs.json")
	.then(function(dogsJson) {
		var parsedDogs = JSON.parse(dogsJson);
		
		// ...do something with parsedDogs...
	});
```

Bundled files can also be referred to by path relative to the project root, and data bound to for example an image:

	<Panel>
		<JavaScript>
		    var Observable = require("FuseJS/Observable");
		    module.exports.cat = Observable("cat.jpg");
		</JavaScript>
		<Image File="{cat}" />
	</Panel>



## Further reading

- @(FuseJS/Bundle)
- [FuseJS Introduction](../fusejs/fusejs.md)
- [Uno Projects](../basics/uno-projects.md)
