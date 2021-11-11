# Signing

When publishing your app to Google Play or the iOS App Store, it needs to be signed before it's sent off.

## Android

A Uno Project set up for signing can look like this:

```json
{
	"Android": {
		"Key": {
			"Alias": "application",
			"AliasPassword": "<alias password>",
			"Store": "release.keystore",
			"StorePassword": "<store password>"
		}
	}
}
```

A file named `release.keystore` is expected to be found in the same folder as the project.

This file can be created by running the following command in your shell (`keytool` is part of the JDK):

```sh
keytool -genkey -v -keystore release.keystore \
    -alias application -keyalg RSA -keysize 2048 -validity 10000
```

The program will prompt you for an alias password and a store password, which should be the same as the respective values in the project file.

Note that only release builds are signed using the specified key. Debug builds are automatically signed using a debug key. To do a release build, use `uno build android --configuration=Release`.

## iOS

Run the following command in your shell:

```sh
uno build ios --configuration=Release --debug
```

Then follow the regular procedure for [adding your signing certificate](https://developer.apple.com/library/ios/documentation/IDEs/Conceptual/AppDistributionGuide/ConfiguringYourApp/ConfiguringYourApp.html#//apple_ref/doc/uid/TP40012582-CH28-SW1) and [submitting your app to the App Store](https://developer.apple.com/library/ios/documentation/LanguagesUtilities/Conceptual/iTunesConnect_Guide/Chapters/SubmittingTheApp.html#//apple_ref/doc/uid/TP40011225-CH33).

### Xcode Development Team IDs

Since Xcode version 8, Apple mandates code-signing even for debug builds.

For this to work you need to set a Development Team ID, and then Xcode will usually handle the rest automatically.

Your Team ID can be found [here](https://developer.apple.com/account/#/membership/).

Uno attempts to automatically find an Xcode Development Team ID by querying the machine's code-signing identities and selecting the first valid one it finds. If this is not the correct one it can be set manually in an `unoproj`s as follows:

```json
{
  "iOS": {
    "DevelopmentTeam": "YOURTEAMID"
  },
  ...
}
```

It can also be set by passing the `--set:Project.iOS.DevelopmentTeam="YOURTEAMID"` flag to `uno build`, e.g.

```sh
uno build ios --set:Project.iOS.DevelopmentTeam="YOURTEAMID"

```

The Development Team ID can also be set user-wide by adding the following to ~/.unoconfig:

```json
iOS.DevelopmentTeam: "YOURTEAMID"
```

The file can be created if it doesn't already exist.

### Troubleshooting

Error messages such as `No matching provisioning profiles found` are usually resolved automatically just by opening the generated Uno project in Xcode once by building with `--debug`. Also make sure that the Development Team ID has been correctly set.

[This](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/Troubleshooting/Troubleshooting.html) page contains more information about troubleshooting certificate issues.
