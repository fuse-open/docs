# Working with foreign code

While we're trying to wrap as much native cross-platform functionality as we can into Fuse, there are naturally going to be features that haven't been implemented yet.
This guide explains how we can use the foreign code feature in Uno to reach into the native functionality of our target platforms.

_Note:_ This feature replaces the old way of doing interop through language bindings. For more information about our reasons for doing so, we have written a [standalone post](https://medium.com/@fusetools/a-sane-way-of-mixing-languages-in-fuse-660b351c2f96) on the subject.


## What is foreign code?

Foreign code allows us to write an Uno method body in a different language. Consider the following example:

```csharp
using Uno.Compiler.ExportTargetInterop;

class Example
{
	[Foreign(Language.Java)]
	public static extern(Android) void Log(string message)
	@{
		android.util.Log.d("ForeignCodeExample", message);
	@}

	[Foreign(Language.ObjC)]
	public static extern(iOS) void Log(string message)
	@{
		NSLog(@"%@", message);
	@}
}
```

Here we're exposing Java and Objective-C's logging functionality so that it's usable in Uno. By tagging the Uno methods with the `Foreign` attribute we can simply write the code we want between the `@{` & `@}` braces in the language that we choose in the attribute.

We use special braces for foreign method bodies so the Uno compiler does not try to read the code as Uno.

We currently support foreign code written in `Objective-C` (on iOS) and `Java` (on Android) using the attribute `[Foreign(Language.ObjC)]` and `[Foreign(Language.Java)]`.

The `Foreign` attribute lives in the `Uno.Compiler.ExportTargetInterop` namespace which can be imported with a `using` at the top of the Uno file.

Note that we have two methods with the same name and signature in the same class. This is allowed since `extern(...)` strips out code that does not match the target platform at an early stage of compilation.


## Type conversion

In foreign code arguments and return values are automatically converted between their Uno representation and their corresponding Java/Objective-C representation.


#### Primitive types

Primitive types (`int`, `float`, `char`, etc) are converted directly to their foreign equivalents with the one exception that Java does not have primitive unsigned bytes, so a `byte` in Java is an `sbyte` in Uno.

So we can write this:

```csharp
[Foreign(Language.Java)]
public static extern(Android) void Foo(double x, long y)
@{
	android.util.Log.d("ForeignCodeExample", "Well look at this: " + x + " and " + y);
@}
```

And call it in Uno like this:

```csharp
Foo(1.2, 12345678);
```

#### Strings

Strings are also handled automatically. Uno strings map to:

- `java.lang.String` in Java
- `NSString*` in Objective-C

Taking the introductory example again, we can write the following:

```csharp
[Foreign(Language.ObjC)]
public static extern(iOS) void Log(string message)
@{
	NSLog(@"%@", message);
@}
```

And call it in Uno like this:

```csharp
Log("Type magic everywhere!");
```

#### Foreign objects passed to Uno

So what happens when we want to return a Java or Objective-C object from a foreign method? The answer is that we get an opaque object representing the foreign object.

An Objective-C object (`id`) is an `ObjC.Object` and a Java object (`Object`) is a `Java.Object` in Uno.

Here's how to get an object from Java:

```csharp
[Foreign(Language.Java)]
public static extern(Android) Java.Object Test(int texName)
@{
    return new android.graphics.SurfaceTexture(texName);
@}
```

And calling it:

```csharp
var stex = Test(1);
```

You may wonder why we don't have special-case support for more types; it's not hard to think of a few more types that would be convenient to get automatic conversions for. The full answer to this can be found in the [Medium article](https://medium.com/@fusetools/a-sane-way-of-mixing-languages-in-fuse-660b351c2f96) mentioned earlier. To summarize:

> Mapping object models in the general case does not work, even in languages as syntactically similar as Java and Uno. The differences necessary in a translation are a mental overhead for the programmer using foreign code. We choose instead to provide a predicable interface and require that working with the internals of a foreign object is done in foreign code.

#### And the other way? (Uno objects passed into foreign code)

We take the same approach. An Uno passed to Java or Objective-C is boxed inside an opaque object.

In Objective-C the type of that box is `id<UnoObject>` and in Java it is `com.uno.UnoObject`


Here is an example of an Uno object being passed in:

```csharp
[Foreign(Language.Java)]
public static extern(Android) void Test(SomeFancyType soFancy)
@{
	// Inside here the type of soFancy is `UnoObject`
	android.util.Log.d("ForeignCodeExample", "Here it is: " + soFancy);
@}
```

Calling this method is straightforward:

```csharp
var v = Test(new SomeFancyType(1, 2, 3));
```

#### Passing `Java.Objects` back to Java and `ObjC.Object` back to Objective-C

We unbox the foreign objects again, so there is no need to worry about boxes-of-boxes-of-boxes.

Just to clarify:
- An `ObjC.Object` passed back to a foreign Objective-C method becomes an `id`.
- A `Java.Object` passed back to a foreign Java method becomes an `Object`.

When that has been done we can cast the object back to its original type.


#### Declaring `Java.Object`s for Java and `ObjC.Object`s for Objective-C

You may want to declare your objects some time in your class...

For Objective-C:

```uno
extern(iOS) ObjC.Object _objcObject;
```

For Java:

```uno
extern(Android) Java.Object _javaObject;
```


## Arrays

There is special support for passing Uno arrays to foreign code. For performance reasons we don't make a copy of the data straight away. Instead, we pass a handle to the Uno array that we can use in our foreign code. If we need a full copy of the data we call `arr.copyArray()` (in Java) or `[arr copyArray]` (in Objective-C) to get the native version of that type. The full details are below.

**Objective-C:**

Arrays are converted to an object of type `id<UnoArray>` which is a wrapper around the Uno array. It can be indexed and updated with the familiar `arr[i]` syntax and has a `count` method (called with `[arr count]`) that returns an `NSUInteger`. Updates to the array it are reflected in the original Uno array --- it's a wrapper, not a copy.  As mentioned, it's also possible to copy the `id<UnoArray>` to an `NSArray*` by calling `[xs copyArray]`.

 Since Objective-C lacks generics, indexing into the `id<UnoArray>` object to get an element returns `id` regardless of the element type of the array on the Uno side. This `id` is a _boxed_ representation of the element type according to the following table:

<a id="foreign-objc-type-table"></a>

| Uno                         | Objective-C           | Boxed array element |
|-----------------------------|-----------------------|---------------------|
| `int`, `bool`, `char`, etc. | `int`, `bool`, `char` | `NSNumber*`         |
| `string`                    | `NSString*`           | `NSString*`         |
| `ObjC.Object`               | `id`                  | `id`                |
| `object`                    | `id<UnoObject>`       | `id<UnoObject>`     |
| `Func<string, int>` etc.    | `^ int(NSString*)`    | `^ int(NSString*)`  |
| `SomeType[]`                | `id<UnoArray>`        | `id<UnoArray>`      |

Most types are already boxed, but note that primitive types like `int`, `bool`, and `char` are boxed as `NSNumber*` when accessed in a wrapped array. This means that to update an Uno array argument `int[] x` on the Objective-C side, we have to write e.g. `x[index] = @42;`. When copying an array, the resulting `NSArray*`'s elements are also boxed following the same rules.

It's possible to circumvent the boxing behaviour by using UXL macros. The following examples contrast the two ways to use arrays in foreign Objective-C code:

```uno
[Foreign(Language.ObjC)]
public static extern(iOS) void ForeignIntArray(int[] xs)
@{
	@{int[]:Of(xs).Set(3, 123)};
	for (int i = 0; i < @{int[]:Of(xs).Length:Get()}; ++i)
	{
		NSLog(@"array[%d]=%d", i, @{int[]:Of(xs).Get(i)});
	}
@}
```

```uno
[Foreign(Language.ObjC)]
public static extern(iOS) void ForeignIntArray(int[] xs)
@{
	xs[3] = @123;
	for (int i = 0; i < [xs count]; ++i)
	{
		NSLog(@"array[%d]=%@", i, xs[i]);
	}
@}
```

**Java:**

Just like in Objective-C, Uno arrays are boxed when passed to Java. You can access a value from the Uno array by calling `get(index)` on it. E.g.

```uno
[Foreign(Language.Java)]
public void Test0(string[] strArr)
@{
	debug_log(strArr.get(0));
@}
```

You can set an element of the array using `set(index, newValue)`

```uno
[Foreign(Language.Java)]
public void Test1(int[] intArr)
@{
	intArr.set(0, 10);
@}
```

Updates to the array it are reflected in the original Uno array --- it's a wrapper, not a copy. As mentioned, it's also possible to copy the uno array to an java array by calling `copyArray`.

```uno
[Foreign(Language.Java)]
public void Test1(int[] intArr)
@{
	int[] intCopy = intArr.copyArray();
	for (int i=0; i < intCopy.length; i++)
		debug_log(intCopy[i])
@}
```

Just like how foreign code knows how to convert basic types to/from java, it also knows how to do the same with arrays. Here are the relationships between the types:

| Uno Type       | Boxed Java Type     | Unboxed Java Type   |
|----------------|---------------------|---------------------|
| bool[]         | com.uno.BoolArray   | bool[]              |
| sbyte[]        | com.uno.ByteArray   | byte[]              |
| char[]         | com.uno.CharArray   | char[]              |
| short[]        | com.uno.ShortArray  | short[]             |
| int[]          | com.uno.IntArray    | int[]               |
| long[]         | com.uno.LongArray   | long[]              |
| float[]        | com.uno.FloatArray  | float[]             |
| double[]       | com.uno.DoubleArray | double[]            |
| string[]       | com.uno.StringArray | String[]            |
| anyOtherType[] | com.uno.ObjectArray | com.uno.UnoObject[] |

Next we may want to pass java arrays to Uno

If we have a boxed Uno array in Java it is easy to return it as an uno array again:

```uno
[Foreign(Language.Java)]
public string[] Test2(string[] wee)
@{
	return wee;
@}
```

But what if we return a Java array? In that case it will be a `Java.Object` just like all other Java objects passed to Uno

```uno
[Foreign(Language.Java)]
public Java.Object Test3()
@{
    int[] myIntArray = {1,2,3};
	return myIntArray;
@}
```

And passing it back to Java is the same too:

```uno
[Foreign(Language.Java)]
public void Test4(Java.Object arr)
@{
    int[] myIntArray = (int[])arr;
	debug_log(myIntArray);
@}
```

There also may be occasions where you want to make an Uno array from Java.

```uno
[Foreign(Language.Java)]
public string[] Test5()
@{
	StringArray freshStringArr = new StringArray(100);
	freshStringArr.set(34, "hi there");
	return freshStringArr;
@}
```

Notice that, because we are returning an Uno array we use the regular `string[]` type.

## Delegates

Foreign code also allows us to pass delegates to foreign methods. The details vary a little between Java & Objective-C:

**Objective-C:**

Delegates get converted to an Objective-C block of the corresponding type. As an example, an argument of the type `Action<string, int>` becomes a block of type `^ void(NSString*, int)`. The argument and return types of the block use the same type conversions as arguments to foreign functions normally do.

Here is a simple example of this in action:

```uno
[Foreign(Language.ObjC)]
public static extern(iOS) double DelegateArgument(Func<int, double> f)
@{
	 // f is of type `^ double(int)` here, so can be called like any function
	 return f(12);
@}
```

It's worth noting that Uno boxes ObjC booleans as `bool` as opposed to Apple's `BOOL`. This has certain implications when managing Uno-created blocks with `bool` values. For instance the block `void(^)(BOOL)` is not the same as the `void(^)(bool)` created by passing in an `Action<bool>`, and cannot be stored in the same properties.

**Java:**

If we define a delegate like this:

```uno
namespace Foo
{
	public delegate void Bar(float x, float y, float z);
}
```

Then it can be used in foreign Java as follows.

```uno
[Foreign(Language.Java)]
public static extern(Android) void ForeignDelegate(Bar x)
@{
	x.run(1.0f, 2.0f, 3.0f);
@}
```

Until version 8 Java didn't have lambdas, and `Runnable` and `Callable`s don't take arguments, so behind the scenes the Uno compiler creates a Java class called `com.foreign.Foo.Bar` with a `public void run(float x, float y, float z) { ... }` method.

The foreign Java type conversions for primitives, strings, objects, and arrays apply to arguments of delegates.

We can also pass Uno `Action`s to Java. Since Java delegates don't support primitives we generate a class for this as well.

The type conversions follow this pattern:

```cpp
Action -> com.foreign.Uno.Action
Action<int> -> com.foreign.Uno.Action_int
Action<int[],int> -> com.foreign.Uno.Action_IntArray_int
```

## Out/ref parameters

Out and ref parameters are supported in foreign Objective-C methods.
The Objective-C type for such a parameter is a pointer to the Objective-C type of the parameter according the [rules for Objective-C/Uno type conversion](#foreign-objc-type-table).

The following two examples show how it works:

```uno
[Foreign(Language.ObjC)]
extern(iOS) void PrimitiveOutParam(ref int m, out int n)
@{
    // m and n are of type `int*` here.
    *m = 222;
    *n = 123;
@}

[Foreign(Language.ObjC)]
extern(iOS) void StringOutParam(ref string m, out string n)
@{
    // m and n are of type `NSString**` here.
    *m = @"Out1";
    *n = @"Out2";
@}
```

We don't support this in foreign Java since Java doesn't have out/ref parameters.

## C pointers

In foreign Objective-C we can use the Uno `IntPtr` type for arguments and
return values that are C or C++ pointers (as opposed to Objective-C objects).
Uno's `IntPtr` corresponds to the `void*` type in foreign code.  Note that
memory-management is _manual_ for such pointers, just like when programming in
C and C++.

## Talking back to Uno

It is not always enough to be able to return values from foreign code to Uno. Sometimes it makes more sense to interact with Uno from inside foreign code. To do this we use _UXL_ (Uno Extension Layer) macros.

Using UXL we can do two main things from foreign code:

- Access Uno fields
- Call Uno methods

To get the full details on UXL, check out our [UXL Handbook](../technical-corner/uxl-handbook.md). For this guide we will show some small examples:


### `Get` and `Set` static Uno fields

We can get or set the value of a property using the `Get` and `Set` macros.

The anatomy of a UXL `Get` expression is as follows:

```uno
v¯¯¯¯¯¯¯¯¯¯v¯ The `@{` `}` syntax means its a UXL macro
@{Foo:Get()}
   ^
 field name
```

And for `Set` expressions it looks like this:

```uno
          v¯¯¯¯¯¯ this is a foreign value
@{Foo:Set(20)}
   ^
 field name
```

Let's see this in some example code:


```uno
using Uno.Compiler.ExportTargetInterop;

public class Example
{
	public static int SomeValue = 7;

	[Foreign(Language.Java)]
	public extern(Android) void Doubler()
	@{
		int originalVal = @{SomeValue:Get()};
		@{SomeValue:Set(originalVal * 2)};
	@}

	[Foreign(Language.ObjC)]
	public extern(iOS) void Doubler()
	@{
		int originalVal = @{SomeValue:Get()};
		@{SomeValue:Set(originalVal * 2)};
	@}
}
```


### `Get` and `Set` Uno instance fields

To call instance methods we need to have a `this` pointer.
To not clash with the target languages' native meaning of `this`, foreign instance methods have access to `_this`, which refers to the object they are called on, wrapped as `UnoObject` or `id<UnoObject>`.

The type of an expression in foreign code can't be automatically inferred by the Uno compiler in general, so we use the `:Of` macro to interact with `_this`:

```csharp
@{MyClass:Of(_this)}
```

The above code says *"the Uno type of `_this` is `MyClass`"*

The following code shows how it works:

```uno
using Uno.Compiler.ExportTargetInterop;

public class Example
{
	public string MyString = "This is all native";

	[Foreign(Language.Java)]
	public extern(Android) void AddExcitement()
	@{
		String originalString = @{Example:Of(_this).MyString:Get()};
		String newString = originalString + "!!!";
		@{Example:Of(_this).MyString:Set(newString)};
	@}

	[Foreign(Language.ObjC)]
	public extern(iOS) void AddExcitement()
	@{
		NSString* originalString = @{Example:Of(_this).MyString:Get()};
		NSString* newString = [originalString stringByAppendingString: @"!!!"];
		@{Example:Of(_this).MyString:Set(newString)};
	@}
}
```

### Calling static Uno methods

UXL's `Call` macro lets us call Uno methods (and other foreign methods) from inside a foreign method.

Let's start with a little anatomy:

```uno
 the Uno method name          The foreign values to be passed as arguments
      v                         v  v
@{Foobernator(int,string):Call(1, "jam")}
               ^    ^
     The Uno types of the arguments
        of the method 'Foobernator'

```

Let's see an example of this:

```uno
using Uno.Compiler.ExportTargetInterop;

public class Example
{
	public static void Angrify(string str)
	{
		debug_log "ARGHHH! " + str + "ARGGHHH!";
	}

	[Foreign(Language.Java)]
	public extern(Android) void Rage()
	@{
		@{Angrify(string):Call("JAVA!")};
	@}

	[Foreign(Language.ObjC)]
	public extern(iOS) void Rage()
	@{
		@{Angrify(string):Call(@"OBJECTIVE-C!")}; // :p
	@}
}
```

### Calling Uno instance method

To call Uno instance methods, we use the `Of` macro with `_this` again:

```uno
using Uno.Compiler.ExportTargetInterop;

class Example
{
	string deviceModel;

	static void Log(string message)
	{
		debug_log(message);
	}

	void SetDeviceModel(string model)
	{
		deviceModel = model;
	}

	[Foreign(Language.Java)]
	extern(Android) void LogDeviceModel()
	{
		String deviceModel = android.os.Build.MODEL;

		// Call instance method
		@{Example:Of(_this).SetDeviceModel(string):Call(deviceModel)};

		// Call static method
		@{Example.Log(string):Call(deviceModel)};
	}

	[Foreign(Language.ObjC)]
	extern(iOS) void LogDeviceModel()
	{
		NSString* deviceModel = [[UIDevice currentDevice] model];

		// Call instance method
		@{Example:Of(_this).SetDeviceModel(string):Call(deviceModel)};

		// Call static method
		@{Example.Log(string):Call(deviceModel)};
	}
}
```


## External source files

A lot of Fuse's philosophy revolves around the idea of 'using the right tool for the right job'. Foreign code makes it easy to call into Java to get something done fast. We also believe that, if you need to write a lot of Java, it's best to do it in Java. To achieve this we have support for adding `.java`, `.mm` & `.hh` files to `.unoproj`s to get them included in the build.

We add Java and Objective-C files like this:

```json
{
	...

	"Includes": [
		"*",
		"Example.hh:ObjCHeader:iOS",
		"Example.mm:ObjCSource:iOS",
		"Example.java:Java:Android"
	]
}
```

This also allows Objective-C code to use UXL macros to talk back to Uno just like in foreign code blocks. For us to use macros in such a file, it has to be an Objective-C++ file (`.mm`) since the macro expansion uses features from both Objective-C and C++ to interoperate with Uno code, and the file has to include `uObjC.Foreign.h`. To include any Uno classes used in the macros, we can use the UXL macro `@{The.Uno.Class:IncludeDirective}`. If we're not using UXL macros it's enough to use `CSource` and `CHeader` file types instead. Processing of Java files is not yet possible, but will be coming soon.

Note that wildcard (`*`) patterns in a project don't automatically set the file type for files that are not Fuse files. With wildcards files in foreign languages will be included in the project, but with the anonymous `File` type. This means that we have to explicitly add foreign files to the project and set the correct file type to ensure that they are processed and added to the project.


### Java

Fuse will parse the `package` statement in included Java files and ensure the folder hierarchy is correct in the project emitted by our compiler, so there is no need to worry about the folder structure when using Java.

We can use the `ForeignInclude` attribute to add imports in Java. It can only be used on classes. The includes affect all foreign methods in the Uno class.

```uno
[ForeignInclude(Language.Java, "java.lang.Runnable", "java.lang.Boolean", "android.app.Activity")]
public class SomeUnoClass : Uno.Application
{
    ...
}
```



### Objective-C

To use external Objective-C headers, we include them in the class, like so:

```uno
[ForeignInclude(Language.ObjC, "Example.hh")]
class Example
{
	...
}
```

_Note:_ Beware of naming collisions! An Objective-C class can't have the same name as an Uno class in the global namespace.

### Swift

It's possible to add Swift files to our `unoproj`s. While we do not yet have support for directly using Swift code blocks in Uno, we can call Swift code from foreign Objective-C.

The following example shows how to use this feature:

`Hello.swift:`
```uno
import Foundation

public class HelloSwiftWorld: NSObject {
    public func hello() {
        print("Hello world!");
    }
}
```

`unoproj`:
```json
{
    "Includes": [
        "Hello.swift:Swift:iOS",
        ...
    ]

}
```

Since Swift can be used from Objective-C, we call into the Swift code by using Foreign Objective-C, for instance as follows:

```uno
[ForeignInclude(Language.ObjC, "@(Project.Name)-Swift.h")]
public class Example
{
    [Foreign(Language.ObjC)]
    public static void DoIt()
    @{
        HelloSwiftWorld* x = [[HelloSwiftWorld alloc] init];
        [x hello];
    @}
}
```

The version of Swift that is used can be configured with the `iOS.SwiftVersion` project property:

```json
"iOS": {
"SwiftVersion": 3.0,
},
```

Swift files currently do not get the foreign macro expansion that `ObjCSource` files get.

## Getting the root Activity

Most Android-related tasks involve the app's root Activity in some way or another. We can get the root activity by calling the following method:

```java
com.fuse.Activity.getRootActivity()
```
