# Gradle Support

Fuse uses Gradle behind the scenes when building for Android. We provide some options for controlling some of this platform specific feature.

## Specifying Gradle and SDK version

You can specify the build tools version and target version for your project.

For example:

```sh
uno build android --run --set:SDK.BuildToolsVersion="23.0.3" --set:SDK.CompileVersion="23" --set:SDK.TargetVersion="23"`
```

To find out which version of the tools you have installed run:

```sh
uno android
```

In your terminal or command prompt. This will open the Android SDK Manager. From here you can check & install different versions.

## Specifying a dependency

To add a gradle dependency on a project add the following attribute to an Uno class

```csharp
[Require("Gradle.Dependency.Compile", "com.github.lecho:hellocharts-library:1.5.8@aar")]
```

Swapping out the `com.github.lecho:hellocharts-library:1.5.8@aar` with a library of your choice.

## Adding additional repositories

To specify an additional repository please add the following attribute to an Uno class

```csharp
[Require("Gradle.Repository", "maven { url 'https://maven.fabric.io/public' }")]
```

If you need to add a repository to the `model.repositories` section of the `build.gradle` file then use:

```csharp
[Require("Gradle.Module.Repository", "maven { url 'https://maven.fabric.io/public' }")]
```

## A note on Android Studio

While Fuse supports `Gradle`, it does not explicitly support `Android Studio`. By this we mean that, when we ship each release, we will make sure that to the best of our knowledge projects compile correctly with gradle, however we will not make the same claim about Android Studio.

The reason for this is that Android Studio is simply changing too quickly to keep up with, breaking changes are common and it is hard to test these changes with our current release cadence.

We do however enable you to open builds in `Android Studio` by using the `--debug` build flag, just please be aware that you may have to jump through a hoop or two to get things working if there have been updates.
