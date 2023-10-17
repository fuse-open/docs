# Uno projects (`.unoproj`)

Uno is a portable, lightweight dialect of C# that sits at the heart of Fuse and allows you to export native apps for iOS and android. To learn more about Uno, see the [Native Interop section](../native-interop/native-interop.md).

Fuse projects are basically Uno projects which references the Fuse libraries. The Fuse libraries are written in the Uno language.

An Uno project file (`.unoproj`) is a plain JSON text file, that has this basic stucture:

```json
{
	"references": [
		"Fuse",
		"FuseJS"
	],
	"includes": [
		"*"
	]
}
```

The Uno project file is where you specify what your project depends on, what it consists of, and optionally how to package it up as a native app for iOS or android.

An Uno project can either be a *library project* consisting of assets and components for use in other projects, or an actual *app project*. The presence of an @App class in the project indicates that the project is an app project.

## References

The `references` section is the list of libraries to be referenced in your project. References are pre-compiled Uno projects ready to use, registered with a canonical library name.

To make a Fuse project, you typically have to reference at least the `Fuse` and `FuseJS` libraries. When scaffolding your application, however, Fuse will include a standard set of libraries to get you up and running.

What libraries you reference may affect the *permissions* your app requires.

```json
{
	"references": [
		"Fuse",
		"FuseJS"
	]
}
```

## Macros

The Uno project format supports a basic macro system.
This feature is used to implement many of the top-level properties, such as [title](#title) and [version](#version).

For instance, say you set the `version` property as follows:

```json
{
	"version": "0.9.4"
}
```

By default, both [android.versionName](#android-versionname) and [ios.bundleVersion](#ios-bundleversion) are set as follows:

```json
{
	"android": {
		"versionName": "$(version)"
	},
	"ios": {
		"bundleVersion": "$(version)"
	}
}
```

This will make those properties inherit the top-level `version` property.

## Projects

The `projects` section is optional, and allows you to reference other Uno projects in the local file system. By relative paths to other `.unoproj` files.

```json
{
	"projects": [
		"../../SomeOtherProject/SomeOtherProject.unoproj"
	]
}
```

## Includes and excludes

The `includes` and `excludes` properties lets you control which files to include in your project. They are both arrays of strings, as shown in the snippet below:

```json
{
	"includes": [
		"*.ux",
		"js/**/*.js",
		"SomeUnoClass.uno:Source",
		"ForeignCode.java:Java:android"
	],
	"excludes": [
		"js/ExcludeThisFilePlease.js",
		"node_modules/"
	]
}
```

An include/exclude entry can take one of the following forms:

### `GlobPattern`

A glob pattern, as described further down in this document.

```json
{
	"includes": [
		"*.ux",
		"js/**/*.js",
		"Foo.uno"
	]
}
```

The following glob features are supported:

- Brace expansion
- Extended glob matching
- Globstar matching

Patterns that do not contain a `/` are matched recursively:

<table class="table">
	<thead>
		<tr>
			<th>Pattern</th>
			<th>Matches</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>```*```</td>
			<td>Matches all files</td>		
		</tr>
		<tr>
			<td>```*.foo```</td>
			<td>Matches all files whose name ends in ```.foo```</td>
		</tr>
		<tr>
			<td>```*.+(bar|foo)```</td>
			<td>Matches all files whose name ends in either ```.bar``` or ```.foo```</td>
		</tr>
		<tr>
			<td>```foobar```</td>
			<td>Matches all files named ```foobar```</td>
		</tr>
	</tbody>
</table>

Add `/` or `./` to disable recursion:

<table class="table">
	<thead>
		<tr>
			<th>Pattern</th>
			<th>Matches</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>```/*.png```</td>
			<td>Matches all PNG files found directly in the project directory</td>
		</tr>
		<tr>
			<td>```Foo/*.png```</td>
			<td>Matches all PNG files found directly in the ```Foo``` directory</td>
		</tr>
	</tbody>
</table>

Use globstar (`**`) for explicit recursion:

<table class="table">
	<thead>
		<tr>
			<th>Pattern</th>
			<th>Matches</th>
		</tr>
	</thead>
	<tbody>
		<tr>
			<td>```Foo/**/*.uno```</td>
			<td>Matches all Uno files in the ```Foo``` directory and its subdirectories</td>
		</tr>
	</tbody>
</table>

### `filename:type`

This tells the Uno compiler to include a single file, interpreting it as a certain [type](#allowed-include-types).

```json
{
	"includes": [
		"MainView.ux:ux"
	]
}
```

### `filename:type:condition`

In addition to a file of a certain [type](#allowed-include-types), you may specify a condition that will determine if the file will be included or not.

```json
{
	"includes": [
		"AndroidOnly.java:java:android",
		"iOSOnly.hh:objcheader:ios",
		"iOSOnly.mm:objcsource:ios"
	]
}
```

### Allowed include types

- `ux` - Declares a UX markup file (automatic when adding files with the .ux extension).
- `source` - An Uno source file (automatic when adding files with the .uno extension).
- `bundle` - A file that will be included in the app bundle and can then be accessed at run time. This can for example be image files like PNG or JPEG, or data files like JSON or XML. It is also needed for when you need to require stand-alone JavaScript files.
- `csource` - A C source file.
- `cheader` - A C header file.
- `objcsource` - An Objective-C source file.
- `objcheader` - An Objective-C header file.
- `java` - A Java code file with the .java file ending
- `extensions` - A UXL extension file.
- `file` - Used to declare a file as an integral part of the project, but one that does not need to be compiled or bundled in the application.

## All properties

### title

The user-readable title of your app. Defaults to `$(name)`.

*unrealisticallyLongNameForAnApp.unoproj*:

```json
{
	"title": "ShorterName"
}
```

### description

User-readable description of your app. Empty by default.

```json
{
	"description": "An app that helps you organize, track and analyze cat pictures on the internet."
}
```

### copyright

Your app's copyright notice. Defaults to `Copyright (C) <current year> $(publisher)`.

```json
{
	"copyright": "Copyright © 2003-2016 $(publisher)"
}
```

### publisher

The legal entity in charge of publishing the application. Defaults to `$(publisher)`.

```json
{
	"publisher": "VeryBusinessCorp Inc."
}
```

### version

The current version of your app,  Defaults to `0.0.0`.

```json
{
	"version": "1.2.9"
}
```

### versionCode

Some platforms (currently only Android) want an integral version code in addition to a version string. Further documentation can be found [here](http://developer.android.com/tools/publishing/versioning.html#appversioning). Defaults to `0`.

```json
{
	"versionCode": 125
}
```

### rootNamespace

The root namespace to use. Defaults to `$(qidentifier)`.

```json
{
	"rootNamespace": "MyCompany.MyApp"
}
```

### buildDirectory

Where to place temporary files and executables for various build configurations and targets. Defaults to `build`.

```json
{
	"buildDirectory": "build"
}
```

Paths are relative to the project root by default. Note that it can be unwise to use absolute paths, as it can cause issues when building from another machine with a different setup.

### outputDirectory

Where to place temporary files and executables for *the current build target and configuration*.
Defaults to `$(buildDirectory)/@(target)/@(configuration:toLower)`.

```json
{
	"outputDirectory": "$(buildDirectory)/@(target)/@(configuration:toLower)"
}
```

### cacheDirectory

Where to place Uno's cache files. Defaults to `.uno`.

```json
{
	"cacheDirectory": ".uno"
}
```

### mobile

A dictionary of options that apply to all mobile targets.

#### mobile.uriScheme

Specifies an URI scheme that can be used to launch your app.

For instance, setting `"uriScheme": "abcd"` will make your app launch when an `abcd://` URI is launched.

```json
{
	"mobile": {
		"uriScheme": "abcd"
	}
}
```

#### mobile.keepAlive

If set to `true`, the screen won't dim and eventually turn off while your app is open. Defaults to `false`.

```json
{
	"mobile": {
		"keepAlive": false
	}
}
```

#### mobile.showStatusbar

Whether or not to show the status bar. Defaults to `true`.

```json
{
	"mobile": {
		"showStatusbar": true
	}
}
```

#### mobile.runsInBackground

Controls whether or not your app should continue running when the user presses the home button. Defaults to `true`.

```json
{
	"mobile": {
		"runsInBackground": false
	}
}
```

#### mobile.orientations

Specifies which screen orientations are allowed.

Can be one of the following values:

- `Auto` (*default*, all possible orientations)
- `Portrait`
- `PortraitUpsideDown`
- `Landscape`
- `LandscapeLeft`
- `LandscapeRight`

```json
{
	"mobile": {
		"orientations": "Portrait"
	}
}
```

### android

A dictionary of options that apply to Android targets only.

#### android.applicationLabel

A user-readable label for the application, specific to android. This is the Android equivalent of the top-level [title](#title) property. defaults to `$(title)`.

```json
{
	"android": {
		"applicationLabel": "MyFancyApp"
	}
}
```

#### android.description

The same as [description](#description), but specific to android. Defaults to `$(description)`.

```json
{
	"android": {
		"description": "This is an Android-specific description!"
	}
}
```

#### android.versionCode

The same as [versionCode](#versioncode), but specific to android. Defaults to `$(versionCode)`.

```json
{
	"android": {
		"versionCode": 412
	}
}
```

#### android.versionName

The same as [version](#version), but specific to android. Defaults to `$(version)`.

```json
{
	"android": {
		"versionName": "0.5.2"
	}
}
```

#### android.package

The name of the java package to use for Android export. Defaults to `$(qidentifier)`.

```json
{
	"android": {
		"package": "com.mycompany.myapp"
	}
}
```

#### android.previewPackage
The name of the java package to use for Android export in preview mode. Defaults to `android.package`.
It's only used during `fuse preview android`, to differentiate between a normal package and a preview package.
Use this setting if you want to have both a preview version and an exported version of your app installed on the device simultaneously.

```json
{
	"android": {
    	"previewPackage": "com.mycompany.myapp.preview"
    }
}
```

#### android.icons

Instead of providing one uniformly sized icon to be used on all platforms, we can specify different icons for different screen densities, specific to android. These are grouped according to Android's [generalized densities](http://developer.android.com/guide/practices/screens_support.html#range). All of them default to `$(icon)`.

*Note:* This only applies to Android, however we can achieve the same on iOS using [ios.icons](#ios-icons).

```json
{
	"android": {
		"icons": {
			"ldpi": "icon-ldpi-36x36.png",
			"mdpi": "icon-mdpi-48x48.png",
			"hdpi": "icon-hdpi-72x72.png",
			"xhdpi": "icon-xhdpi-96x96.png",
			"xxhdpi": "icon-xxhdpi-144x144.png",
			"xxxhdpi": "icon-xxxhdpi-192x192.png"
		}
	}
}
```

#### android.Key

See [signing](../preview-and-export/signing.md).

#### android.geo.apiKey

Your Google Maps API key, for use with [MapView](../fuse/controls/mapview.md).

```json
{
	"android": {
		"geo": {
			"apiKey": "<your Google Maps API key>"
		}
	}
}
```

#### android.googlePlay.senderID

In order to use the [push notification api](../fuse/pushnotifications/push.md) on Android, you have to specify a project ID from the [Google Developers Console](https://console.developers.google.com/).

```json
{
	"android": {
	    "googlePlay": {
	        "senderID": "111781901112"
	    }
	}
}
```

#### android.ndk.platformVersion

The NDK platform version to use. Defaults to `9`.

```json
{
	"android": {
		"ndk": {
			"platformVersion": 9
		}
	}
}
```

#### android.sdk

Specifies version constraints for the Android SDK to build against.
The following example shows the default values of each property.

```json
{
	"android": {
		"sdk": {
			"buildToolsVersion": "23.0.0",
			"compileVersion": 19,
			"minVersion": 10,
			"targetVersion": 19
		}
	}
}
```

### ios

#### ios.bundleIdentifier

The iOS bundle identifier.

Corresponds to [CFBundleIdentifier](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/20001431-102070).

```json
{
	"ios": {
		"bundleIdentifier": "com.mycompany.myapp"
	}
}
```

#### ios.previewBundleIdentifier
The iOS bundle identifier in preview mode. Defaults to `ios.bundleIdentifier`.
It's only used during `fuse preview ios`, to differentiate between a normal bundle and a preview bundle.
Use this setting if you want to have both a preview version and an exported version of your app installed on the device simultaneously.

```json
{
	"ios": {
    	"previewBundleIdentifier": "com.mycompany.myapp.preview"
    }
}
```

#### ios.bundleName

A user-readable label for the application specific to ios. This is the iOS equivalent of [android.applicationLabel](#android-applicationlabel).
Corresponds to [CFBundleName](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-109585).
Defaults to `$(title)`.

```json
{
	"ios": {
		"bundleName": "MyAwesomeApp"
	}
}
```

#### ios.bundleVersion

The same as [version](#version), but specific to ios.
Defaults to `$(version)`.

Corresponds to [CFBundleVersion](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/20001431-102364).

```json
{
	"ios": {
		"bundleVersion": "0.5.2"
	}
}
```

#### ios.deploymentTarget

The minimum iOS version your app can run on.
Defaults to `11.0`.

```json
{
	"ios": {
		"deploymentTarget": "11.0"
	}
}
```

#### ios.icons

Instead of providing one uniformly sized icon to be used on all platforms, we can specify different icons for different sizes and screen densities, specific to ios.
All of them default to `$(icon)`. These icons are applied to release builds, meaning a preview build will keep the Fuse preview icon.

Corresponds to [CFBundleIconFiles](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-SW10).

*Note:* This only applies to iOS, however we can achieve the same on Android using [android.icons](#android-icons).

```json
{
	"ios": {
		"icons": {
			"iphone_20_2x": "icon-iPhone-20@2x.png",
			"iphone_20_3x": "icon-iPhone-20@3x.png",
			"iphone_29_2x": "icon-iPhone-29@2x.png",
			"iphone_29_3x": "icon-iPhone-29@3x.png",
			"iphone_40_2x": "icon-iPhone-40@2x.png",
			"iphone_40_3x": "icon-iPhone-40@3x.png",
			"iphone_60_2x": "icon-iPhone-60@2x.png",
			"iphone_60_3x": "icon-iPhone-60@3x.png",
			"ipad_20_1x": "icon-iPad-20@1x.png",
			"ipad_20_2x": "icon-iPad-20@2x.png",
			"ipad_29_1x": "icon-iPad-29@1x.png",
			"ipad_29_2x": "icon-iPad-29@2x.png",
			"ipad_40_1x": "icon-iPad-40@1x.png",
			"ipad_40_2x": "icon-iPad-40@2x.png",
			"ipad_76_1x": "icon-iPad-76@1x.png",
			"ipad_76_2x": "icon-iPad-76@2x.png",
			"ipad_83.5_2x": "icon-iPad-83.5@2x.png",
			"ios_marketing_1024_1x": "iOS-Marketing-1024@1x.png"
		}
	}
}
```

#### ios.launchImages

Specifies launch images of different sizes to be used on ios.
Corresponds to [UILaunchImages](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW28).

```json
{
	"ios": {
		"launchImages": {
			"iphone_portrait_iphonex_3x": "...",  // 1125x2436
			"iphone_landscape_iphonex_3x": "...", // 2436x1125
			"iphone_portrait_2x": "...",          // 640x960
			"iphone_portrait_r4": "...",          // 640x1136
			"iphone_portrait_r47": "...",         // 750x1334
			"iphone_portrait_r55": "...",         // 1242x2208
			"iphone_landscape_r55": "...",        // 2208x1242
			"ipad_portrait_1x": "...",            // 768x1024
			"ipad_portrait_2x": "...",            // 1536x2048
			"ipad_landscape_1x": "...",           // 1024x768
			"ipad_landscape_2x": "..."            // 2048x1536
		}
	}
}
```

#### ios.systemCapabilities

Capabilities are App Services which need special declarations in the Xcode project.

In all cases where it is reliable to do so, Fuse will also add the appropriate Entitlements to your project.

For more info on the valid values for `AssociatedDomains`, `KeychainSharing` & `ApplicationGroups` please see the [Apple documentation](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html)

```json
{
	"ios": {
		"systemCapabilities": {
			"applicationGroups": ["group.A", "group.B"],
			"dataProtection": true,
			"gameCenter": true,
			"healthKit": true,
			"homeKit": true,
			"inAppPurchase": true,
			"interAppAudio": true,
			"keychainSharing": ["AAA", "BBB"],
			"push": true,
			"associatedDomains": ["CCC", "DDD"],
			"personalVPN": true,
			"wirelessAccessoryConfiguration": true
		}
	}
}
```

#### ios.plist

A dictionary of entries that will be included in the `Info.plist` file of the iOS bundle. Note that not all property list entries will work – only the ones that are included here.

##### ios.plist.MKDirectionsApplicationSupportedModes

An array of strings, specifying the modes of transportation for which the app is capable of giving map directions.
Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW33).

```json
{
	"ios": {
		"plist": {
			"MKDirectionsApplicationSupportedModes": [
				"MKDirectionsModeCar",
				"MKDirectionsModeBus"
			]
		}
	}
}
```

##### ios.plist.UIRequiredDeviceCapabilities

An array of strings, specifying which device-related capabilities must be available for the app to function.
Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW3).

```json
{
	"ios": {
		"plist": {
			"UIRequiredDeviceCapabilities": [
				"camera-flash",
				"gps"
			]
		}
	}
}
```

##### ios.plist.NSHealthShareUsageDescription

A message that will be shown to the user when the app is requesting HealthKit data.

```json
{
	"ios": {
		"plist": {
			"NSHealthShareUsageDescription": "We need access to your HealthKit data because..."
		}
	}
}
```

##### ios.plist.UIApplicationExitsOnSuspend

When `true`, the app will terminate rather than being sent to the background when the user presses the home button. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW23).

```json
{
	"ios": {
		"plist": {
			"UIApplicationExitsOnSuspend": false
		}
	}
}
```

##### ios.plist.UIFileSharingEnabled

Specifies whether the app can share files through iTunes when connected to a computer.
Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW20).

```json
{
	"ios": {
		"plist": {
			"UIFileSharingEnabled": true
		}
	}
}
```

##### ios.plist.UINewsstandApp

Specifies whether the app presents its contents in the iOS Newsstand app. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW25).

```json
{
	"ios": {
		"plist": {
			"UINewsstandApp": true
		}
	}
}
```

##### ios.plist.UIPrerenderedIcon

```json
{
	"ios": {
		"plist": {
			"UIPrerenderedIcon": true
		}
	}
}
```

##### ios.plist.UISupportedExternalAccessoryProtocols

An array of strings specifying which external accessory protocols your app is capable of communicating over. Defaults to `[]`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW4).

```json
{
	"ios": {
		"plist": {
			"UISupportedExternalAccessoryProtocols": [
				"my-external-accessory-protocol"
			]
		}
	}
}
```

<!--
##### ios.plist.UIViewControllerBasedStatusBarAppearance

TODO: Figure out what this means for Fuse apps.
-->

##### ios.plist.UIViewEdgeAntialiasing

Specifies whether Core Animation layers use antialiasing when drawing a layer that is not aligned to pixel boundaries. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW5).

```json
{
	"ios": {
		"plist": {
			"UIViewEdgeAntialiasing": true
		}
	}
}
```
