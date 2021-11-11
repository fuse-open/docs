# Uno Projects (`.unoproj`)

Uno is a portable, lightweight dialect of C# that sits at the heart of Fuse and allows you to export native apps for iOS and Android. To learn more about Uno, see the [Native Interop section](../native-interop/native-interop.md).

Fuse projects are basically Uno projects which references the Fuse libraries. The Fuse libraries are written in the Uno language.

An Uno project file (`.unoproj`) is a plain JSON text file, that has this basic stucture:

```json
{
	"Packages": [
		"Fuse",
		"FuseJS"
	],
	"Includes": [
		"*"
	]
}
```

The Uno project file is where you specify what your project depends on, what it consists of, and optionally how to package it up as a native app for iOS or Android.

An Uno project can either be a *library project* consisting of assets and components for use in other projects, or an actual *app project*. The presence of an @App class in the project indicates that the project is an app project.

## Packages

The `Packages` section is the list of packages to be referenced in your project. Packages are pre-compiled Uno projects ready to use, registered with a canonical package name in the Uno package manager.

To make a Fuse project, you typically have to reference at least the `Fuse` and `FuseJS` packages. When scaffolding your application, however, Fuse will include a standard set of packages to get you up and running.

What packages you reference may affect the *permissions* your app requires.

```json
{
	"Packages": [
		"Fuse",
		"FuseJS"
	]
}
```

## Macros

The Uno project format supports a basic macro system.
This feature is used to implement many of the top-level properties, such as [Title](#title) and [Version](#version).

For instance, say you set the `Version` property as follows:

```json
{
	"Version": "0.9.4"
}
```

By default, both [Android.VersionName](#android-versionname) and [iOS.BundleVersion](#ios-bundleversion) are set as follows:

```json
{
	"Android": {
		"VersionName": "$(Version)"
	},
	"iOS": {
		"BundleVersion": "$(Version)"
	}
}
```

This will make those properties inherit the top-level `Version` property.

## Projects

The `Projects` section is optional, and allows you to reference other Uno projects in the local file system. By relative paths to other `.unoproj` files.

```json
{
	"Projects": [
		"../../SomeOtherProject/SomeOtherProject.unoproj"
	]
}
```

## Includes and Excludes

The `Includes` and `Excludes` properties lets you control which files to include in your project. They are both arrays of strings, as shown in the snippet below:

```json
{
	"Includes": [
		"*.ux",
		"js/**/*.js",
		"SomeUnoClass.uno:Source",
		"ForeignCode.java:Java:android"
	],
	"Excludes": [
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
	"Includes": [
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

### `FileName:Type`

This tells the Uno compiler to include a single file, interpreting it as a certain [type](#allowed-include-types).

```json
{
	"Includes": [
		"MainView.ux:UX"
	]
}
```

### `FileName:Type:Condition`

In addition to a file of a certain [type](#allowed-include-types), you may specify a condition that will determine if the file will be included or not.

```json
{
	"Includes": [
		"AndroidOnly.java:Java:Android",
		"iOSOnly.hh:ObjCHeader:iOS",
		"iOSOnly.mm:ObjCSource:iOS"
	]
}
```

### Allowed Include Types

- `UX` - Declares a UX markup file (automatic when adding files with the .ux extension).
- `Source` - An Uno source file (automatic when adding files with the .uno extension).
- `Bundle` - A file that will be included in the app bundle and can then be accessed at run time. This can for example be image files like PNG or JPEG, or data files like JSON or XML. It is also needed for when you need to require stand-alone JavaScript files.
- `CSource` - A C source file.
- `CHeader` - A C header file.
- `ObjCSource` - An Objective-C source file.
- `ObjCHeader` - An Objective-C header file.
- `Java` - A Java code file with the .java file ending
- `Extensions` - A UXL extension file.
- `File` - Used to declare a file as an integral part of the project, but one that does not need to be compiled or bundled in the application.

## All properties

### Title

The user-readable title of your app. Defaults to `$(Name)`.

*unrealisticallyLongNameForAnApp.unoproj*:

```json
{
	"Title": "ShorterName"
}
```

### Description

User-readable description of your app. Empty by default.

```json
{
	"Description": "An app that helps you organize, track and analyze cat pictures on the internet."
}
```

### Copyright

Your app's copyright notice. Defaults to `Copyright (C) <current year> $(Publisher)`.

```json
{
	"Copyright": "Copyright © 2003-2016 $(Publisher)"
}
```

### Publisher

The legal entity in charge of publishing the application. Defaults to `$(Publisher)`.

```json
{
	"Publisher": "VeryBusinessCorp Inc."
}
```

### Version

The current version of your app,  Defaults to `0.0.0`.

```json
{
	"Version": "1.2.9"
}
```

### VersionCode

Some platforms (currently only Android) want an integral version code in addition to a version string. Further documentation can be found [here](http://developer.Android.com/tools/publishing/versioning.html#appversioning). Defaults to `0`.

```json
{
	"VersionCode": 125
}
```

### RootNamespace

The root namespace to use. Defaults to `$(QIdentifier)`.

```json
{
	"RootNamespace": "MyCompany.MyApp"
}
```

### BuildDirectory

Where to place temporary files and executables for various build configurations and targets. Defaults to `build`.

```json
{
	"BuildDirectory": "build"
}
```

Paths are relative to the project root by default. Note that it can be unwise to use absolute paths, as it can cause issues when building from another machine with a different setup.

### OutputDirectory

Where to place temporary files and executables for *the current build target and configuration*.
Defaults to `$(BuildDirectory)/@(Target)/@(Configuration)`.

```json
{
	"OutputDirectory": "$(BuildDirectory)/@(Target)/@(Configuration)"
}
```

### CacheDirectory

Where to place Uno's cache files. Defaults to `.uno`.

```json
{
	"CacheDirectory": ".cache"
}
```

### UnoCoreReference

Whether or not UnoCore should be referenced. You will probably never need this, as it's only used internally. Defaults to `true`.

```json
{
	"UnoCoreReference": true
}
```

### Mobile

A dictionary of options that apply to all mobile targets.

#### Mobile.UriScheme

Specifies an URI scheme that can be used to launch your app.

For instance, setting `"UriScheme": "abcd"` will make your app launch when an `abcd://` URI is launched.

```json
{
	"Mobile": {
		"UriScheme": "abcd"
	}
}
```

#### Mobile.KeepAlive

If set to `true`, the screen won't dim and eventually turn off while your app is open. Defaults to `false`.

```json
{
	"Mobile": {
		"KeepAlive": false
	}
}
```

#### Mobile.ShowStatusbar

Whether or not to show the status bar. Defaults to `true`.

```json
{
	"Mobile": {
		"ShowStatusbar": true
	}
}
```

#### Mobile.RunsInBackground

Controls whether or not your app should continue running when the user presses the home button. Defaults to `true`.

```json
{
	"Mobile": {
		"RunsInBackground": false
	}
}
```

#### Mobile.Orientations

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
	"Mobile": {
		"Orientations": "Portrait"
	}
}
```

### Android

A dictionary of options that apply to Android targets only.

#### Android.ApplicationLabel

A user-readable label for the application, specific to Android. This is the Android equivalent of the top-level [Title](#title) property. defaults to `$(Title)`.

```json
{
	"Android": {
		"ApplicationLabel": "MyFancyApp"
	}
}
```

#### Android.Description

The same as [Description](#description), but specific to Android. Defaults to `$(Description)`.

```json
{
	"Android": {
		"Description": "This is an Android-specific description!"
	}
}
```

#### Android.VersionCode

The same as [VersionCode](#versioncode), but specific to Android. Defaults to `$(VersionCode)`.

```json
{
	"Android": {
		"VersionCode": 412
	}
}
```

#### Android.VersionName

The same as [Version](#version), but specific to Android. Defaults to `$(Version)`.

```json
{
	"Android": {
		"VersionName": "0.5.2"
	}
}
```

#### Android.Package

The name of the java package to use for Android export. Defaults to `$(QIdentifier)`.

```json
{
	"Android": {
		"Package": "com.mycompany.myapp"
	}
}
```

#### Android.PreviewPackage
The name of the java package to use for Android export in preview mode. Defaults to `Android.Package`.
It's only used during `fuse preview android`, to differentiate between a normal package and a preview package.
Use this setting if you want to have both a preview version and an exported version of your app installed on the device simultaneously.

```json
{
	"Android": {
    	"PreviewPackage": "com.mycompany.myapp.preview"
    }
}
```

#### Android.Icons

Instead of providing one uniformly sized icon to be used on all platforms, we can specify different icons for different screen densities, specific to Android. These are grouped according to Android's [generalized densities](http://developer.Android.com/guide/practices/screens_support.html#range). All of them default to `$(Icon)`.

*Note:* This only applies to Android, however we can achieve the same on iOS using [iOS.Icons](#ios-icons).

```json
{
	"Android": {
		"Icons": {
			"LDPI": "Icon-ldpi-36x36.png",
			"MDPI": "Icon-mdpi-48x48.png",
			"HDPI": "Icon-hdpi-72x72.png",
			"XHDPI": "Icon-xhdpi-96x96.png",
			"XXHDPI": "Icon-xxhdpi-144x144.png",
			"XXXHDPI": "Icon-xxxhdpi-192x192.png"
		}
	}
}
```

#### Android.Key

See [signing](../preview-and-export/signing.md).

#### Android.Geo.ApiKey

Your Google Maps API key, for use with [MapView](../fuse/controls/mapview.md).

```json
{
	"Android": {
		"Geo": {
			"ApiKey": "<your Google Maps API key>"
		}
	}
}
```

#### Android.GooglePlay.SenderID

In order to use the [push notification api](../fuse/pushnotifications/push.md) on Android, you have to specify a project ID from the [Google Developers Console](https://console.developers.google.com/).

```json
{
	"Android": {
	    "GooglePlay": {
	        "SenderID": "111781901112"
	    }
	}
}
```

#### Android.NDK.PlatformVersion

The NDK platform version to use. Defaults to `9`.

```json
{
	"Android": {
		"NDK": {
			"PlatformVersion": 9
		}
	}
}
```

#### Android.SDK

Specifies version constraints for the Android SDK to build against.
The following example shows the default values of each property.

```json
{
	"Android": {
		"SDK": {
			"BuildToolsVersion": "23.0.0",
			"CompileVersion": 19,
			"MinVersion": 10,
			"TargetVersion": 19
		}
	}
}
```

### iOS

#### iOS.BundleIdentifier

The iOS bundle identifier.

Corresponds to [CFBundleIdentifier](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/20001431-102070).

```json
{
	"iOS": {
		"BundleIdentifier": "com.mycompany.myapp"
	}
}
```

#### iOS.PreviewBundleIdentifier
The iOS bundle identifier in preview mode. Defaults to `iOS.BundleIdentifier`.
It's only used during `fuse preview ios`, to differentiate between a normal bundle and a preview bundle.
Use this setting if you want to have both a preview version and an exported version of your app installed on the device simultaneously.

```json
{
	"iOS": {
    	"PreviewBundleIdentifier": "com.mycompany.myapp.preview"
    }
}
```

#### iOS.BundleName

A user-readable label for the application specific to iOS. This is the iOS equivalent of [Android.ApplicationLabel](#android-applicationlabel).
Corresponds to [CFBundleName](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-109585).
Defaults to `$(Title)`.

```json
{
	"iOS": {
		"BundleName": "MyAwesomeApp"
	}
}
```

#### iOS.BundleVersion

The same as [Version](#version), but specific to iOS.
Defaults to `$(Version)`.

Corresponds to [CFBundleVersion](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/20001431-102364).

```json
{
	"iOS": {
		"BundleVersion": "0.5.2"
	}
}
```

#### iOS.DeploymentTarget

The minimum iOS version your app can run on.
Defaults to `8.0`.

```json
{
	"iOS": {
		"DeploymentTarget": "8.0"
	}
}
```

#### iOS.Icons

Instead of providing one uniformly sized icon to be used on all platforms, we can specify different icons for different sizes and screen densities, specific to iOS.
All of them default to `$(Icon)`. These icons are applied to release builds, meaning a preview build will keep the Fuse preview icon.

Corresponds to [CFBundleIconFiles](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/CoreFoundationKeys.html#//apple_ref/doc/uid/TP40009249-SW10).

*Note:* This only applies to iOS, however we can achieve the same on Android using [Android.Icons](#android-icons).

```json
{
	"iOS": {
		"Icons": {
			"iPhone_20_2x": "Icon-iPhone-20@2x.png",
			"iPhone_20_3x": "Icon-iPhone-20@3x.png",
			"iPhone_29_2x": "Icon-iPhone-29@2x.png",
			"iPhone_29_3x": "Icon-iPhone-29@3x.png",
			"iPhone_40_2x": "Icon-iPhone-40@2x.png",
			"iPhone_40_3x": "Icon-iPhone-40@3x.png",
			"iPhone_60_2x": "Icon-iPhone-60@2x.png",
			"iPhone_60_3x": "Icon-iPhone-60@3x.png",
			"iPad_20_1x": "Icon-iPad-20@1x.png",
			"iPad_20_2x": "Icon-iPad-20@2x.png",
			"iPad_29_1x": "Icon-iPad-29@1x.png",
			"iPad_29_2x": "Icon-iPad-29@2x.png",
			"iPad_40_1x": "Icon-iPad-40@1x.png",
			"iPad_40_2x": "Icon-iPad-40@2x.png",
			"iPad_76_1x": "Icon-iPad-76@1x.png",
			"iPad_76_2x": "Icon-iPad-76@2x.png",
			"iPad_83.5_2x": "Icon-iPad-83.5@2x.png",
			"iOS-Marketing_1024_1x": "iOS-Marketing-1024@1x.png"
		}
	}
}
```

#### iOS.LaunchImages

Specifies launch images of different sizes to be used on iOS.
Corresponds to [UILaunchImages](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW28).

```json
{
	"iOS": {
		"LaunchImages": {
			"iPhone_Portrait_iPhoneX_3x": "...",  // 1125x2436
			"iPhone_Landscape_iPhoneX_3x": "...", // 2436x1125
			"iPhone_Portrait_2x": "...",          // 640x960
			"iPhone_Portrait_R4": "...",          // 640x1136
			"iPhone_Portrait_R47": "...",         // 750x1334
			"iPhone_Portrait_R55": "...",         // 1242x2208
			"iPhone_Landscape_R55": "...",        // 2208x1242
			"iPad_Portrait_1x": "...",            // 768x1024
			"iPad_Portrait_2x": "...",            // 1536x2048
			"iPad_Landscape_1x": "...",           // 1024x768
			"iPad_Landscape_2x": "..."            // 2048x1536
		}
	}
}
```

#### iOS.SystemCapabilities

Capabilities are App Services which need special declarations in the Xcode project.

In all cases where it is reliable to do so, Fuse will also add the appropriate Entitlements to your project.

For more info on the valid values for `AssociatedDomains`, `KeychainSharing` & `ApplicationGroups` please see the [Apple documentation](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html)

```json
{
	"iOS": {
		"SystemCapabilities": {
			"ApplicationGroups": ["group.A", "group.B"],
			"DataProtection": true,
			"GameCenter": true,
			"HealthKit": true,
			"HomeKit": true,
			"InAppPurchase": true,
			"InterAppAudio": true,
			"KeychainSharing": ["AAA", "BBB"],
			"Push": true,
			"AssociatedDomains": ["CCC", "DDD"],
			"PersonalVPN": true,
			"WirelessAccessoryConfiguration": true
		}
	}
}
```

#### iOS.PList

A dictionary of entries that will be included in the `Info.plist` file of the iOS bundle. Note that not all property list entries will work – only the ones that are included here.

##### iOS.PList.MKDirectionsApplicationSupportedModes

An array of strings, specifying the modes of transportation for which the app is capable of giving map directions.
Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW33).

```json
{
	"iOS": {
		"PList": {
			"MKDirectionsApplicationSupportedModes": [
				"MKDirectionsModeCar",
				"MKDirectionsModeBus"
			]
		}
	}
}
```

##### iOS.PList.UIRequiredDeviceCapabilities

An array of strings, specifying which device-related capabilities must be available for the app to function.
Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW3).

```json
{
	"iOS": {
		"PList": {
			"UIRequiredDeviceCapabilities": [
				"camera-flash",
				"gps"
			]
		}
	}
}
```

##### iOS.PList.NSHealthShareUsageDescription

A message that will be shown to the user when the app is requesting HealthKit data.

```json
{
	"iOS": {
		"PList": {
			"NSHealthShareUsageDescription": "We need access to your HealthKit data because..."
		}
	}
}
```

##### iOS.PList.UIApplicationExitsOnSuspend

When `true`, the app will terminate rather than being sent to the background when the user presses the home button. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW23).

```json
{
	"iOS": {
		"PList": {
			"UIApplicationExitsOnSuspend": false
		}
	}
}
```

##### iOS.PList.UIFileSharingEnabled

Specifies whether the app can share files through iTunes when connected to a computer.
Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW20).

```json
{
	"iOS": {
		"PList": {
			"UIFileSharingEnabled": true
		}
	}
}
```

##### iOS.PList.UINewsstandApp

Specifies whether the app presents its contents in the iOS Newsstand app. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW25).

```json
{
	"iOS": {
		"PList": {
			"UINewsstandApp": true
		}
	}
}
```

##### iOS.PList.UIPrerenderedIcon

```json
{
	"iOS": {
		"PList": {
			"UIPrerenderedIcon": true
		}
	}
}
```

##### iOS.PList.UISupportedExternalAccessoryProtocols

An array of strings specifying which external accessory protocols your app is capable of communicating over. Defaults to `[]`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW4).

```json
{
	"iOS": {
		"PList": {
			"UISupportedExternalAccessoryProtocols": [
				"my-external-accessory-protocol"
			]
		}
	}
}
```

<!--
##### iOS.PList.UIViewControllerBasedStatusBarAppearance

TODO: Figure out what this means for Fuse apps.
-->

##### iOS.PList.UIViewEdgeAntialiasing

Specifies whether Core Animation layers use antialiasing when drawing a layer that is not aligned to pixel boundaries. Defaults to `false`.

Reference can be found [here](https://developer.apple.com/library/ios/documentation/General/Reference/InfoPlistKeyReference/Articles/iPhoneOSKeys.html#//apple_ref/doc/uid/TP40009252-SW5).

```json
{
	"iOS": {
		"PList": {
			"UIViewEdgeAntialiasing": true
		}
	}
}
```
