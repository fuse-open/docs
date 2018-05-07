# macOS Installation

## Prerequisites
A modern Mac, running [macOS Yosemite or later](../supported-platforms.md).

Internet access is required during installation.

## Fuse
Download Fuse for macOS from [the Downloads page](/downloads) and run the installer.
After installation, we can find the Fuse application in the "Applications" folder. We can also start it simply by opening a terminal window and typing `fuse`. When Fuse is running we can see its tray icon in the menu bar. Our dashboard should also pop up right after running `fuse` for the first time. The dashboard can always be started from the tray icon.

## Post Installation
For code completion, syntax highlighting and other nice features, take a look at our plugins for [Sublime Text 3](sublime-plugin.md) or [Atom](atom-plugin.md).

To preview your projects on real devices you can use the [Fuse Preview app](../preview-and-export.md#fuse-preview-app). This does not require you to install Xcode or the Android SDKs.

In order to do Android *app exports* and custom previews you will need to install the required third party SDKs. See the [Preview and export](../preview-and-export.md) section.

## Uninstall
If for any reason we need to remove all of the Fuse components, we can use [this uninstall script](https://gist.github.com/Tapped/daa78c08882f33b0c7c3).

## Xcode
In order to do iOS *app exports* and custom previews you will need the latest version of Xcode. Follow [these instruction](https://developer.apple.com/xcode/downloads/), or search for it in the Mac App Store.

## Next Steps
Now that we've got Fuse up and running, it's time to play around with it. Head over to our [quickstart tutorial](../quickstart.md) to get started!
