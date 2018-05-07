# Windows Installation

## Prerequisites
A modern PC equipped with an OpenGL 2.1-capable GPU, running [Windows 7 or later](../supported-platforms.md).
Very basic on-board/shared resource GPUs such as the Intel GMA-series are _not_ sufficient.

Internet access is required during installation.



## Fuse
Download Fuse for Windows from [the downloads page](https://www.fusetools.com/downloads) and run the installer.

We can start the Fuse daemon simply by typing `fuse` on the command prompt. **Note: You may have to log out and in again (or simply reboot) to make sure the path for fuse is properly updated.**
When Fuse is running we can see its tray icon in the task bar. Our dashboard should also pop up right after running `fuse` for the first time. The dashboard may always be started from the tray icon.

## Post Installation
For code completion, syntax highlighting and other nice features, take a look at our plugins for [Sublime Text 3](sublime-plugin.md) or [Atom](atom-plugin.md).

To preview your projects on real devices you can use the [Fuse Preview app](../preview-and-export.md). This does not require you to install the Android SDKs (and also lets you preview on iOS devices!)

In order to do Android *app exports* and custom previews you will need to install the required third party SDKs. See the [Preview and export](../preview-and-export.md) section.

## Next Steps
Now that we've got Fuse up and running, it's time to play around with it. Head over to our [quickstart tutorial](../quickstart.md) to get started!
