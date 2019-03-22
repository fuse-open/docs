# Build settings

Since Uno generates projects in different formats depending on the build target
it is sometimes necessary to be able to control various aspects of the
generated project, for instance to include headers and link libraries.  This is
especially true when interoperating with native libraries.
This document is a reference of the available settings.

For an example of how some of these settings can be used together with [foreign
code](../native-interop/foreign-code.md), see the [Facebook login
tutorial](../native-interop/facebook-login.md). Another document that may come in
handy is the [UXL handbook](../technical-corner/uxl-handbook.md), which outlines
features that often necessitate changing some build setting.

## Two ways to change a setting

The settings in this document can either be set as an attribute attached to an
Uno class or method.  The benefit of doing these as attributes is that they
then only take effect when that class is used (since Uno strips out unused
code), which is usually what you want.

To use a setting on an Uno class, add

```csharp
    using Uno.Compiler.ExportTargetInterop;
```

to the top of the file and use e.g. the following code:

```csharp
    [Require("LinkDirectory", "my/path")]
    public class MyClass
    {
        ...
    }
```

When the settings become numerous or require long strings, it can be convenient
to do it in a separate file. We use `.uxl` files with `unoproj` filetype
`Extensions` to do this.

An example of `uxl` file that adds to the Xcode `plist` file is the following:

```xml
    <Extensions Backend="CPlusPlus" Condition="iOS">
        <Require Xcode.Plist.Element>
            <![CDATA[
                <key>NSLocationUsageDescription</key>
                <string></string>
                <key>NSLocationWhenInUseUsageDescription</key>
                <string></string>
                <key>NSLocationAlwaysUsageDescription</key>
                <string></string>
            ]]>
        </Require>
    </Extensions>
```

Here we see we first specify the backend (usually `CPlusPlus`), and
(optionally) a target `Condition`, meaning that the code is only used on `iOS`
in this case. We then `Require` an `Xcode.Plist.Element` and use XML character
data (`CDATA`) to avoid having to escape the XML that we want inserted in the
`Plist` file.

## C++ target settings

Most of our build settings apply only to our C++ targets.  These include `iOS`
and `Android` as well as the `native` desktop targets, while apps built for
desktop preview use the CIL target.

### Including a static library

The following is an example of how a typical library might be included in the
project when targeting iOS. See below for a reference for the settings that we
use.

```csharp
[extern(iOS) Require("LinkDirectory", "@('typicalLibrary/lib':Path)")]
[extern(iOS) Require("IncludeDirectory", "@('typicalLibrary/include':Path)")]
[extern(iOS) Require("LinkLibrary", "typicalLibrary")]
[extern(iOS) Require("Source.Include", "typicalLibrary.h")]
class AClassThatNeedsTypicalLibrary { }
```

Here we're assuming that `typicalLibrary` has a header called
`typicalLibrary.h` under `typicalLibrary/include` (in the directory of our
`unoproj`) and that there is a `libtypicalLibrary.a` file under
`typicalLibrary/lib`.

We use `extern(iOS)` on the attributes to only link the library when targeting
iOS.

We also add the library's files to our `unoproj`:

```json
{
    ...

    "Includes": [
        ...
        "typicalLibrary/include/typicalLibrary.h:File",
        "typicalLibrary/lib/libtypicalLibrary.a:File",
    ]
}
```

Here we use the `File` filetype, which indicates that Uno shouldn't do anything
special with the files. We do this since we're including the files manually by
using attributes.

### Build flags

```csharp
    [Require("LinkDirectory", "my/path")]
```

Adds `my/path` to the library search path. This is equivalent to passing
`-Lmy/path` to the C++ compiler. Using `"@('my/relative/path':Path)"` as the
path means that it's relative.

---

```csharp
    [Require("LinkLibrary", "mylibrary")]
```

Link the application with `mylibrary`. This is equivalent to passing
`-lmylibrary` to the C++ compiler.

Note: For linking libraries on Android, use the `JNI.*` settings below.

---

```csharp
    [Require("IncludeDirectory", "my/path")]
```

Adds `my/path` to the include search path. This is equivalent to passing
`-Imy/path` to the C++ compiler.  Using `"@('my/relative/path':Path)"` as the
path means that it's relative.

### Including foreign source files

We add Objective-C, Java, and C source files to Uno projects as follows:

```json
    {
        ...

        "Includes": [
            "*",
            "Example.hh:ObjCHeader:iOS",
            "Example.mm:ObjCSource:iOS",
            "Example.java:Java:Android"
            "Example.h:CHeader",
            "Example.c:CSource",
        ]
    }
```

For more information, see [the foreign code documentation](../native-interop/foreign-code.md).

### Android-specific settings

#### JNI

Since we treat shared and static native libraries differently on Android we do
not use `LinkLibrary` for both, but instead use the following two elements:

```csharp
    [Require("JNI.SharedLibrary", "mylib.so")]
```

Loads `mylib.so` using `System.loadLibrary` when the application starts, and
includes `mylib.so` in the app.

---

```csharp
    [Require("JNI.SystemLibrary", "systemlib")]
```

Loads `systemlib` using `System.loadLibrary` when the application starts
(without including it in the app, assuming it's a system library like `log`).

---

```csharp
    [Require("JNI.StaticLibrary", "mylib.a")]
```

Statically links the application with `mylib.a`.

#### Resources

```csharp
    [Require("Android.ResStrings.Declaration", "<string name=\"hello\">Hello!</string>")]
```

Adds `<string name="hello">Hello!</string>` to the string resource file:
`res/values/strings.xml`.

#### Manifest

```csharp
    [Require("AndroidManifest.ActivityElement", "<an-activity-element />")]
```

Adds `<an-activity-element />` to the application's `activity` section in the
`AndroidManifest.xml` file.

---

```csharp
    [Require("AndroidManifest.ApplicationElement", "<an-application-element />")]
```

Adds `<an-application-element />` to the `application` section in the
`AndroidManifest.xml` file.

---

```csharp
    [Require("AndroidManifest.Permission", "android.permission.A_PERMISSION")]
```

Adds `<uses-permission android:name="android.permission.A_PERMISSION" />` to
the `AndroidManifest.xml` file.

---

```csharp
    [Require("AndroidManifest.RootElement", "<a-root-element />")]
```

Adds `<a-root-element />` at the root level below the `manifest` tag in the
`AndroidManifest.xml` file.

---
```csharp
    [Require("AndroidManifest.Activity.ViewIntentFilter",
        "android:host=\"sites.google.com\" android:pathPrefix=\"/site/appindexingex/home/main\" android:scheme=\"https\"")]
```

Adds the specified string to the application view's intent-filter.


#### Gradle

These settings apply when building for Android using our preliminary Gradle
experimental support which is enabled when building with the `-DGRADLE` Uno flag.
See
[this](https://docs.gradle.org/current/userguide/artifact_dependencies_tutorial.html)
for more information about dependency management using Gradle.

---

```csharp
    [Require("Gradle.Dependency", "compile('myDependency') { transitive = true }")]
```

Adds `compile('myDependency') { transitive = true }` to `dependencies` in the
app's generated `build.gradle` file.

---

```csharp
    [Require("Gradle.Dependency.ClassPath", "myDependency")]
```

Adds `classpath 'myDependency'` to `dependencies` in the top-level generated `build.gradle`
file.

---

```csharp
    [Require("Gradle.Dependency.Compile", "myDependency")]
```

Adds `compile 'myDependency'` to `dependencies` in the app's generated `build.gradle`
file.

---

```csharp
    [Require("Gradle.BuildFile.End", "stuff")]
```

Adds `stuff` to the very end of the app's generated `build.gradle` file.

---

```csharp
    [Require("Gradle.Repository", "mavenCentral()")]
```

Adds `mavenCentral()` to `repositories` in the app's generated `build.gradle` file.

### iOS-specific settings


#### Xcode settings

```csharp
    [Require("Xcode.EmbeddedFramework", "my.framework")]
```

Adds `my.framework` as an embedded framework to the generated Xcode project.
Using `"@('my/relative.framework':Path)"` as the path means that it's relative.

---

```csharp
    [Require("Xcode.Framework", "a.framework")]
```

Adds `a.framework` as a framework to the generated Xcode project. If the path
to the framework is not absolute, it is assumed to be relative to
`SDKROOT/System/Library/Frameworks` and can therefore be used for frameworks
from the iPhone SDK.

Using `"@('my/relative.framework':Path)"` as the path means that it's relative.

---

```csharp
    [Require("Xcode.FrameworkDirectory", "@('my/relative/framework/path':Path")]
```

Adds `my/relative/framework/path` to the framework search path. This is
equivalent to passing `-Fmy/relative/framework/path` to the C++ compiler.
Note: When including frameworks using `Xcode.Framework` or
`Xcode.EmbeddedFramework` this flag is automatically set, so it need not be set
in that case.

---

```csharp
    [Require("Xcode.Plist.Element", "<plist-element />")]
```

Adds `<plist-element />` to the application's `plist` file.

---

```csharp
    [Require("Xcode.ShellScript", "someScript")]
```

Adds `someScript` in a `PBXProjShellScriptBuildPhase` in the generated
Xcode project's `.pbxproj` file.


#### CocoaPods

These settings apply when building for iOS using our preliminary CocoaPods
support which is enabled when building with the `-DCOCOAPODS` Uno flag.

```csharp
    [Require("Cocoapods.Platform.Name", "platformName")]
```

Adds

    platform :platformName, ''

to the generated `Podfile`.

---

```csharp
    [Require("Cocoapods.Platform.Version", "platformVersion")]
```

If `Cocoapods.Platform.Name` is set to `platformName`, adds

    platform :platformName, 'platformVersion'

to the generated `Podfile`.

---

```csharp
    [Require("Cocoapods.Podfile.Target", "pod 'Firebase'")]
```

Adds `pod 'Firebase'` to the application's `target` configuration in the
generated `Podfile`.

---

```csharp
    [Require("Cocoapods.Podfile.Pre", "myPreStatement")]
```

Adds `myPreStatement` to the beginning of the generated `Podfile`.

---

```csharp
    [Require("Cocoapods.Podfile.Post", "myPostStatement")]
```

Adds `myPostStatement` to the end of the generated `Podfile`.
