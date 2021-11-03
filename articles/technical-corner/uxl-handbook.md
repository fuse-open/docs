# UXL Handbook (Here be Dragons)

Typically, you'll be able to do everything you need to build entire front-end components and apps with Fuse with just pure UX and JavaScript. Sometimes, however, you'll need to write cross-platform code in Uno to take advantage of the system's raw processing power or interop with native libraries. In even fewer cases, just using Uno _still_ won't quite get the job done; for instance, if you've got a custom, platform-specific native component you've written and need to use that from Uno and/or expose it to JavaScript. When you absolutely _must_ go deeper, we've got you covered with **UXL**.

**UXL** stands for **U**no e**X**tension **L**ayer. It's a term that covers a wide number of features that are used to hook into the Uno compiler in various ways when you need to do things beyond the standard feature set.

This document is a pretty informal (yet relatively complete) run down of the current status of UXL. Please be aware, though, that these features are very much subject to change, as is this document, hence the "Here be Dragons" title. You've been warned :)

The best way to see how this stuff works is in code so lets start.

## `extern(CONDITION)`

`extern()` allows us to include uno code in our build only if it meets our condition.

For the rest of the document the phrases 'meets the condition' or 'the condition is met' can be read as 'the condition evaluated to true'.

For example the following class will only be included when you are building for iOS:

```csharp
    public extern(iOS) class Test0
    {
        public void SayHi()
        {
            debug_log "hi from ios";
        }
    }
```

We can also use simple boolean logic inside the `extern()` condition

```csharp
    public extern(!iOS) class Test0
    {
        public void SayHi()
        {
            debug_log "hi from anything but iOS :)";
        }
    }
```

Stripping of `extern()` happens very early in the compilation process (while processing the AST) so most syntax bugs will not be caught if the condition is not met. Be sure to test *all targets* for code using UXL.

Because `extern()` stripping is done so early we can 'break' many Uno rules. For example, the above two classes can be in the same namespace without causing a name collision. This means we don't need factory methods for instantiating platform specific instances of a class, we can just conditionally compile them using `extern()`.

`extern()` can also be used on methods and fields

```csharp
    class Test1 // we can use extern on methods too
    {
        public extern(ANDROID) void SayHi()
        {
            debug_log "hi from android";
        }
        public extern(!ANDROID) void SayHi()
        {
            debug_log "hi from not android";
        }
    }
```

And, as with types, we don't get a collision even though the method signatures are identical.

**NOTE:** For you own sanity, don't have different public interfaces for different extern conditions. While it works, it tends to come back to haunt you eventually.

## `if defined()`

`if defined()` does the same thing for statements that `extern()` did for types, methods and fields.

```csharp
    class Test2
    {
        public void SomeMethod()
        {
            if defined(ANDROID)
            {
                debug_log "works for android";
            } else if defined(IOS) {
                debug_log "works for ios";
            } else {
                debug_log "anything else";
            }
        }
    }
```

Note that the syntax is `if defined(CONDITION)` and not `if (defined(CONDITION))`. This by design to highlight the difference of *when* this is being evaluated.

## `extern ""`

`extern ""` (pronounced 'extern string') is a way of writing expressions in the native target language inline in your uno code. For example:

```csharp
    public class Test
    {
        // note that the extern can go before the access modifiers
        extern(MOBILE) // MOBILE is equivalent to (IOS||ANDROID)
        public void SomeMethod()
        {
            extern "Xli::Console::WriteLine(\"I can has null deref exceptions\")";
        }

        extern(JAVASCRIPT)
        public void SomeMethod()
        {
            extern "Console.log(\"I can has Ï€\")";
        }

        extern(!(MOBILE||JAVASCRIPT))
        public void SomeMethod()
        {
            debug_log "why not do this in the first place?!"
        }
    }
```

Any time you use `extern ""` make sure it gets used only in the correct backend. For example in the above code we know the MOBILE platforms imply C++.

`extern ""` is awesome and should be used for all places you need native code unless it makes things too complicated; in those cases we will fall back to target-specific UXL files, but that will discussed in a later section.

`extern ""` can return values:

```csharp
    extern(CPLUSPLUS)
    public int AddOne()
    {
        var result = extern<int>"10 + 1"; // note that the `extern ""` is an expression and doesnt need a `;` inside the string
        return result;
    }
```

The generic-style type specifier can be any valid Uno type.

`extern ""` can also have values passed into it:

```csharp
    extern(CPLUSPLUS)
    public int AddOne(int x)
    {
        return extern<int>(x)"$0 + 1";
    }
```

`$0` is a UXL macro naming the first argument pass in to the form.

`extern ""` can take multiple arguments:

```csharp
    extern(CPLUSPLUS)
    public int AddIntAndFloat(int x, float y)
    {
        return extern<int>(x,y)"$0 + (int)$1";
    }
```

Just like `$0` names the first argument, `$1` names the second, and this applies to as many arguments as you like.

Do be aware that inside the extern syntax, when we say `int`, we are saying `C++ int`, whilst the `<int>` is an Uno `int`.


## UXL Macros
With `$0` and `$1` above we have seen our first UXL macros. There are many more, some of which are covered below.

#### Specify an Uno type with `@{type}` syntax
You may want to define a variable with an Uno type inside the native code. Or maybe cast a native type to an Uno type.

This can be done as follows:

```csharp
    extern(CPLUSPLUS)
    public int AddIntAndFloat(long x, float y)
    {
        extern(x) "@{long} z = $0";
        return extern<long>(x,y)"z + (@{long})$1";
    }
```

Here we can see a few interesting features of `extern`:
- `@{someUnoType}` will expand the native version of that Uno type
- We can declare an `extern ""`'s return type to be void. This is implicit, though, so you never have to write this.
- `extern ""`'s maintain their order within the compiled output. This means we can declare `z` in the first extern and use it in the second.
- You should use the `@{type}` syntax **everywhere** you can. It will catch subtle bugs that are really hard to trace down. For example, Uno's `long` is a C++ `long long`.

`@{type}` syntax works with all Uno types; I used Uno's `long` type in the above code only because:
- We havent covered how to `new` an Uno object using UXL yet, and
- It is easy to forget that primitive types don't necessarily map to the native types with the same names.

#### Refer to static field in UXL
We can access a static field of type easily from UXL. Below we show how to access the `_name` field of the `Test0` class:

```csharp
    public class Test0 {
        public static string _name = "Jeff";
    }

    public class Test1 {
        public string getTest0Name()
        {
            extern "@{string} tmp = @{Test0._name}";
            return extern<string>"tmp";
        }
    }
```

As you can see, the `@{type}` syntax works for classes too.

#### Refer to a field on an instance of a type using the `:Of` macro
UXL cannot infer the type of a C++ variable, so when we need access to instance data we have to tell UXL the type. We use the `:Of` macro for this:

```csharp
    public class Test0 {
        public string _instanceName = "Jeff";
    }

    public class Test1 {
        public string getTest0Name(Test0 x)
        {
            extern(x) "@{Test0} tmp = @0";
            extern"@{string} tmp2 = @{Test0:Of(tmp)._instanceName}"
            return extern<string>"tmp2";
        }
    }
```

This is a somewhat convoluted example but should show the point. Of course, if you really need the value of a field in UXL, then you should pass it in from Uno using:

    extern(x._instanceName)"..some code using $0 here";

#### Call a static method using UXL with the `:Call` macro

```csharp
    public class Test0 {
        public static string AgeJeff(int x) { return "Jeff is " + x;  }
    }

    public class Test1 {
        public string getTest0Name()
        {
            return extern<string>"@{Test0.AgeJeff(int):Call(253)}";
        }
    }
```

To accurately specify the method to call we need to provide the signature (only the types are needed).

Then in the `Call` we provide the actual arguments.

#### Call an instance method using UXL
As you could guess, you use the `:Of` macro in the same way as we did for fields:

```csharp
    public class Test0 {
        public string AgeJeff(int x) { return "Jeff is " + x;  }
    }

    public class Test1 {
        public string getTest0Name(Test0 x)
        {
            return extern<string>(x)"@{Test0:Of($0).AgeJeff(int):Call(253)}";
        }
    }
```

#### Notes on access modifiers

UXL sees all (with caveats): it doesn't obey access modifers. If native can reference it, it can. Obviously the converse is also true, so when compiling to C# you most likely can't access everything.


#### Make a new instance of a class from UXL using the `:New` macro
Making a new instance of the class is very similar to `:Call`. The syntax is:

`@{Test2} someVar = @{Test2(int,float):New(1,1.2f)}`

Where:
- `Test2` is the type,
- `(int,float)` is the type signature of one of the constructors, and
- `(1,1.2f)` are the arguments to the constructor.

As usual, if we can, you should use `new` in Uno and pass it into the UXL as an extern argument, like so:

`extern(new Test2(1,1.2f))"..some native code"`

#### Get the Uno Type object using the `:TypeOf` macro

`@{Android.android.accessibilityservice.AccessibilityService:TypeOf}`

is the equivalent of:

`typeof(Android.android.accessibilityservice.AccessibilityService)`

#### Get or Set a property using the `:Get` and `:Set` macros
Here is how to get the value of a property using UXL:

`@{Uno.Exception:Of($0).Message:Get()}`

Set is very similar:

`@{Uno.Exception:Of($0).Message:Set($1)}`

#### And more...
There are more macros to be seen but Î™ think that an introduction to UXL files is needed first.


## UXL Files and the `[TargetSpecificImplementation]` attribute
`extern ""` syntax is a relatively new feature for Uno. Before this, all the native code had to be defined in a seperate file.
Even now, there are many cases where these files are neccesary.

To tell Uno that a given type has a native implementation we use the `[TargetSpecificImplementation]` attribute.

We also use the attribute on any method we want to implement in native code.

Here is an example class:

```csharp
    [TargetSpecificImplementation]
    class Test3
    {
        [TargetSpecificImplementation]
        public extern float FirstMethod(int a);

        [TargetSpecificImplementation]
        public extern float SecondMethod(int a) // SecondMethod has a default implementation
        {
            return 0f;
        }

        public static float Helper(int a)
        {
            return (float)a*a;
        }

        public float NonStaticHelper(int a)
        {
            return (float)a*a;
        }
    }
```

There are some things to be aware of here:

- Each method that needs a native implementation has the extern modifier
- We can use the abstract-style definition with no body or provide a default implementation

The difference between FirstMethod and SecondMethod is that FirstMethod must be implemented for every backend whereas if SecondMethod isn't implemented for a particular backend it will default to the defined implementation.

To implement these TargetSpecific methods we need to make UXL files.

#### UXL Files
UXL files are XML documents that hold implementations of TargetSpecific methods and variety of other operations and metadata.

Traditionally, we have made seperate UXL files specific to the language being compiled to, so:
- for C++ you would make a `someName.cpp.uxl` file.
- for JavaScript you would make a `someName.js.uxl` file.
- for CIL you would make a `someName.cil.uxl` file.

The compiler doesn't enforce this, though.

Here is a possible UXL file for the `Test3` class we saw above:

```xml
    <Extensions Backend="CPlusPlus">
        <Type Name="Test3">
            <Method Signature="FirstMethod(int):float">
                <Expression>((@{float})$0)</Expression>
            </Method>

            <Method Signature="SecondMethod(int):float">
                <Require Entity="Test3.NonStaticHelper(int)" />
                <Body><![CDATA[
                    @{float} tmp = @{Test3.Helper(int):Call($0)};
                    return tmp + @{Test3:Of($$).NonStaticHelper(int):Call($0)};
                ]]></Body>
            </Method>
        </Type>
    </Extensions>
```

Here we can see some macros we will recognize, and some we may not.

The `$$` macro is the `this` variable of the instance.

The `Extensions` tag specific what backend the contents are valid for.

The `Type` tag specifies the Uno type we are implementing. That type must have a `[TargetSpecificImplementation]` attribute.

Then we have the method implementations. The signature is almost the same as used in the `:Call` macro, except here we also have to specify the return type after a colon (but only if there is a return type).

The implementation for `FirstMethod` is inside the `Expression` tags. This means the code will be inlined and so, like the `extern ""`, we don't need a semicolon.

The implementation for `SecondMethod` is inside the `Body` tags, this means the code will not be inlined in the callsite but will instead be the body of the method in C++. Also, inside body we can have multiple lines. The use of `CDATA` is not mandatory but means you don't have to escape as many characters and generally makes your life as a programmer easier.

In `SecondMethod` we also see something new, the `Require Entity` tag. This requires a brief interlude to talk about how Uno strips code.

### Code Stripping and UXL
Uno tries to remove everything that is not used by some part of the program. To do this the compiler walks the Uno code, adding references to everything that is 'touched'. At the end of the walk, if a method, field or type has a reference count of 0, then it is stripped.

Note that the references are only added if something is used in Uno, **not** if it is used in UXL. This means that if we want to ensure an 'entity' is not stripped we should add a reference to it. We do this with the `Require Entity` tag.

- `<Require Entity="Test3.NonStaticHelper(int)" />` adds a reference to a method
- `<Require Entity="Test3._someField" />` adds a reference to a field
- `<Require Entity="Test3" />` adds a reference to a type

Where we place this `Require Entity` is important because it affects when it will take effect.

- Putting it inside the `Method` block means it will add a reference if that method is not stripped
- Putting it inside the `Type` block means it will add a reference if that type is not stripped
- Putting it inside the `Extensions` block means it will always add a reference

Code stripping is one of Uno's strengths, so be as restrive as possible when choosing where to add the reference. As a rough rule: `Method > Type > Extensions`.

Now, back to the snooker...

### More UXL Tags

#### Set the file extension of the generated native code
If you use Objective C in your UXL you need to ensure that the file extension is `.mm` rather than `.cpp`, or you will get some very bizarre include errors from Xcode when you try and compile.

    <Set Source.FileExtension="mm" />

#### UXL Equivalent of using
Writing out fully qualified types can get tedious. The `Using` tag does the same job s Uno's `using` statement:

```xml
    <Using Namespace="iOS.Foundation" />
```

#### Adding includes using the `Require` tag and the `:Include` macro
In your UXL you may reference other Uno types, for example by calling one of their methods. However for some targets you need to include the header file for that type before you can use it. We do this with the require tag

```xml
    <Require Source.Include="@{Android.Base.AndroidBindingMacros:Include}" />
    <Require Header.Include="@{Android.Base.AndroidBindingMacros:Include}" />
```

In the above we use the `:Include` macro to get the path of the `.h` file we require.

By using `Source.Include` we say the `#include` will be added to the `.cpp` file for the type.

By using `Header.Include` we say the `#include` will be added to the `.h` file for the type.

It is always preferable to use `Source.Include` where you can and `Header.Include` if you must.

We can also include regular C/C++ header files by providing the path explicitly:

```xml
    <Require Header.Include="Uno/Platform2.h" />
    <Require Header.Include="Uno/Platform2.h" />
```

#### Adding arbitrary source to native files using the `Require Source.Declaration` tag

This feature is documented for completeness and as it is _very, very occasionally_ the right thing to use, however, if you are using it, it should be because there is no other option.

```xml
        <Require Source.Declaration><![CDATA[
            void JNICALL Android_NativeActivityHelper_onActionModeFinished(JNIEnv *env, jclass clazz, jobject arg)
            {
                uAutoReleasePool pool;
                @{Activity.OnActionModeFinished:Call(arg)};
            }
        ]]></Require>
```

`Source.Declaration` will add the block of native code to the top of the compiled C++ code of the type. It will be after the includes but before the opening of any namespaces.

`Header.Declaration` does the same but for the `.h` file.

If you add multiple `Source.Declaration` blocks to a single class there is no guarentee what the order will be in the final C++.

#### UXL Conditions using `Condition=`
UXL has an equivalent to `extern(CONDITION)`:

```xml
    <Require Entity="Uno.Platform2.Display.OnTick(Uno.Platform2.TimerEventArgs)" Condition="MOBILE" />
```

For example, here we only add a reference to the `OnTick` method if we are compiling for mobile devices.

This can also be used on `Type` and `Method` tags so we can provide different implementations for `ANDROID` and `iOS` (for example).

#### Expanding Macros in arbitrary types using the `ProcessFile` tag
There are cases you may find that you want to expand some UXL in a non-UXL block. It may be as simple as putting a `project version=@(Project.Version)` in a readme file, or something more complex like changing the behavior of a Java method if a particular Uno method has been stripped.

To have the file processed by the UXL processor use the following syntax:

- `<ProcessFile HeaderFile="Uno/Platform2.h" />` for native header files
- `<ProcessFile SourceFile="Uno/Platform2.h" />` for native source files
- `<ProcessFile Name="Camera.java" TargetName="src/com/fuse/Native/Camera.java" />` for other kinds of file

Note that in the last case you should define where the file will be put in the output build directory of the project.

#### Copying a file without expanding any macros using the `CopyFile` tag

Much like `ProcessFile`, this copies a file into the build output, but by not having to parse for UXL macros, it is much faster. Use this wherever possible.

```xml
<CopyFile Name="Camera.java" TargetName="src/com/fuse/Native/Camera.java" />
```

## `TargetSpecificType`'s
Whilst the name is quite similar to `TargetSpecificImplementation`, the purpose of `TargetSpecificType`'s is very different.

`TargetSpecificType`'s allow you to pass a native type around in Uno in a type-safe way. You can write Uno methods that operate on that type, and generally use it in a convenient way. You don't get extra overhead either as the final type definition is that of the native type you are wrapping.

Here is the simplest possible example of a `TargetSpecificType`:

```csharp
    [TargetSpecificType]
    public extern(ANDROID) struct UJObject {}
```


```xml
    <Type Name="UJObject" Condition="ANDROID"
        TypeName="jobject"
        Include="jni.h" />
```

Here we are defining an Uno type that will stand in for the JNI `jobject` type. Some things to know:

- `TargetSpecificType`s must be struct's. We used to allow them to be classes, but this is deprecated.
- `TargetSpecificType`s can be boxed by Uno without issues.
- The `Include` defined in the UXL will be added to all files that use the `TargetSpecificType`.
- Structs are value types, so we can't compare them to Uno's `null`.

### Defining Uno Methods on `TargetSpecificType`'s
This is a really great way to move as much logic as possible into Uno. Here is a similified example of one such type:

```csharp
[TargetSpecificType]
public extern(ANDROID) struct AndroidNativeView
{
    public static AndroidNativeView Null
    {
        get { return extern<AndroidNativeView> "NULL"; }
    }

    public static bool operator ! (AndroidNativeView handle)
    {
        return extern<bool> "!$0";
    }

    public static implicit operator bool (AndroidNativeView handle)
    {
        return extern<bool> "$0";
    }

    public static bool operator == (
        AndroidNativeView lhs, AndroidNativeView rhs)
    {
        return IsSameView(lhs, rhs);
    }

    public static bool operator != (
        AndroidNativeView lhs, AndroidNativeView rhs)
    {
        return !(lhs == rhs);
    }

    public override bool Equals(object obj)
    {
        return obj is AndroidNativeView && (AndroidNativeView)obj == this;
    }

    [TargetSpecificImplementation]
    public override extern int GetHashCode();

    [TargetSpecificImplementation]
    private static bool IsSameView(AndroidNativeView handle1, AndroidNativeView handle2)
    {
        return extern<bool> "$0 == $1";
    }
}
```

```xml
<Type Name="AndroidNativeView" Condition="Android"
    TypeName="jobject"
    Include="jni.h">

    <Method Signature="GetHashCode():int">
        <Require Source.Include="Xli/Traits.h" />
        <Expression>::Xli::DefaultTraits::Hash($$)</Expression>
    </Method>
    <Method Signature="IsSameView(AndroidNativeView,AndroidNativeView):bool">
        <Body>
            JniHelper jni;
            jni->IsSameObject($0, $1);
        </Body>
    </Method>
</Type>
```

## What values are available in UXL

This section is coming soon.

## Templates

Templates are a way to group UXL tags under one name that can then be used in other UXL. For example, maybe you need to add references to a lot of methods if either of two classes are used. You could write:

```xml
    <Template Name="Example">
        <Require Entity="Uno.Platform2.Application.Start()" />
        <Require Entity="Uno.Platform2.Application.EnterForeground()" />
        <Require Entity="Uno.Platform2.Application.EnterInteractive()" />
        <Require Entity="Uno.Platform2.Application.ExitInteractive()" />
        <Require Entity="Uno.Platform2.Application.EnterBackground()" />
        <Require Entity="Uno.Platform2.Application.Terminate()" />
        <Require Entity="Uno.Platform2.Application.OnReceivedLowMemoryWarning()" />
    </Template>
```

and then simply add...

```xml
    <Require Template="Example" />
```

...to the two classes that need these methods.

The only valid UXL tags that can put in a template are:

- `<CopyFile>`
- `<ProcessFile>`
- `<Require>`

# Common Issues

### I'm getting loads of crazy errors using Objective C inside UXL when compiling to C++
Here is my build log:

```s
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSDictionary.h:5:
In file included from /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObject.h:7:
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObjCRuntime.h:400:1: error: expected unqualified-id
@class NSString, Protocol;
^
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObjCRuntime.h:413:53: error: format argument not an NSString
FOUNDATION_EXPORT void NSLog(NSString *format, ...) NS_FORMAT_FUNCTION(1,2);
                            ~~~~~~~~~~~~~~~~       ^                  ~
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObjCRuntime.h:98:49: note: expanded from macro 'NS_FORMAT_FUNCTION'
       #define NS_FORMAT_FUNCTION(F,A) __attribute__((format(__NSString__, F, A)))
                                                      ^
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObjCRuntime.h:414:63: error: format argument not an NSString
FOUNDATION_EXPORT void NSLogv(NSString *format, va_list args) NS_FORMAT_FUNCTION(1,0);
                             ~~~~~~~~~~~~~~~~                ^                  ~
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObjCRuntime.h:98:49: note: expanded from macro 'NS_FORMAT_FUNCTION'
       #define NS_FORMAT_FUNCTION(F,A) __attribute__((format(__NSString__, F, A)))
                                                      ^
In file included from /Users/erik/src/fuselibs/Tests/NativeTextPrototype/.build/iOS-Debug/src/app/-.CanvasTexture.cpp:8:
In file included from include/iOSCanvasTexture.h:5:
In file included from /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSDictionary.h:5:
In file included from /Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSObject.h:8:
/Applications/Xcode.app/Contents/Developer/Platforms/iPhoneOS.platform/Developer/SDKs/iPhoneOS8.4.sdk/System/Library/Frameworks/Foundation.framework/Headers/NSZone.h:8:1: error: expected unqualified-id
@class NSString;
```

#### Answer:
You are using Objective C in a file that is being compiled as 'regular' C++. For this to work, you need the file extension to be `.mm`. Add the following to your UXL file:

```xml
    <Set Source.FileExtension="mm" />
```

This means if it was in an inline `extern ""` expression that you need to create a small UXL file.

In a future version we will have an Uno based method for doing this.


### Uno can't find my method even though it's definitely there, and even giving the full qualified type isn't working

#### Answer:
Uno namespace resolution may be screwing with you. Remember that Uno resolves namespaces like C#, so [this](https://msdn.microsoft.com/en-us/library/c3ay4x3d.aspx) applies.

TLDR: Try sticking `global::` at the beginning of the fully qualified type.

### Writing out all these paths is very tedious. Is there an equivalent of `using`?

#### Answer:
yes :)

```xml
    <Using Namespace="iOS.Foundation" />
```
