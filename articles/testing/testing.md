# Testing

A test in UX consists of an element with a `ux:Test` attribute.
The value of this attribute determines the name of the test.

```xml
<Panel ux:Test="MyTest">

</Panel>
```

Any JavaScript code that throws an `Error` will make the test fail.

```xml
<Panel ux:Test="MyTest">
	<JavaScript>
		throw new Error("This will always fail");
	</JavaScript>
</Panel>
```

Pure JavaScript tests are also possible using `JavaScriptTest`:

```xml
<JavaScriptTest ux:Test="MyJsTest" File="myJsTest.js" />
```

To run your tests, open a command-line shell and execute the following:

```
uno test <path-to-unoproj>
```

Where `<path-to-unoproj>` is the path to your Fuse project's `.unoproj` file.