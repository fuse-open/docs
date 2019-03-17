# FAQ

## Supported platforms

The full list of supported platforms for both Fuse and exports can be found [here](supported-platforms.md).

## General tips

### Upgrading Fuse

- When you have upgraded Fuse since the last time you built a project, clean the project before building (run `uno clean` in a terminal in the project folder).

---

## Logs

When investigating issues it's often useful to check the logs written by Fuse and the editor plugins. These can be be found in:

- OS X: `~/.fuse/logs`
- Windows: `%localappdata%\Fusetools\Fuse\logs`

### Installer logs
If you need a log from the Windows installer, pass `-l <log file name>` to the installer executable. For instance: `fuse_win_1_4_0_14778 -l logfile.log`.

---

### Why does `Text.Padding` not add padding to my text?

`Padding` applies to the *children* of an element. The text is part of the `<Text>` element itself, not a child of it. If you want padding around a piece of text, put `Padding` in the containing element instead:
```xml
	<Panel Padding="10" Color="#9f9" Alignment="Center">
		<Text Alignment="Center" Value="text"/>
	</Panel>
```

![TextPadding](../../media/faq-text-padding.png)

(Removing `Padding` from the `<Panel>` above, and setting `Margin` on the `<Text>` instead has the same effect).


## Connection issues

### A network error occurred

You get a message saying `fuse: A network error occurred: Could not resolve host '<your hostname>' Please check your network setup and try again.`

#### Solution

There is an issue with your network. Try doing `ping $(hostname)` in a terminal. If you get an error message, you have to fix your network setup before you continue.

---

### FailedToConnectToDaemon

You get a stack trace with `FailedToConnectToDaemon` somewhere in it
 
#### Solution

- First, try to stop the Fuse daemon.
 - Windows: Right click on the Fuse icon in the tray, click "Exit"
 - OS X: Control-click on the Fuse icon in the menu bar, click "Quit"
- A fresh daemon will then start automatically the next time you interact with Fuse.
- If that did not help, please try running `fuse kill-all`. This will terminate all running Fuse processes.
- If that did not help, please try logging out of and back in to Windows / OS X.
- If that did not help, please try to reboot your computer.

---

### Preview - Failed to connect

While previewing on an iOS or Android device, you get the message "Failed to connect"

#### Solution
- Make sure your device has WiFi enabled
- Make sure your device is connected to the same WiFi as the computer running Fuse 
- If your computer is running a firewall (such as Windows Firewall), make sure Fuse is allowed to accept incoming connections
- If you have tethering enabled on your Android device (sharing the mobile network over USB), try disabling it
- If you still have the problem, quit Fuse from the tray / menu bar icon and re-start Fuse

---

## Cannot preview or export to Android

### Symptoms
- The Android build gets stuck at "Trying to uninstall existing version of APK"
- The build finishes with "ERROR: No android devices found."

#### Solution
- Check that the device is connected with a USB cable
- Check that usb debugging is enabled on your device. Doing this can differ between different devices and OS versions so you may have to search for the specific instructions for your device, but the second point [here](http://developer.android.com/tools/device.html#setting-up) is a good place to start
- Check that your device shows up when running `uno adb devices`
- If you are on Windows you should check that you have the latest USB drivers for your device. Read more about this [here](http://developer.android.com/tools/extras/oem-usb.html)

---

## Cannot build for iOS

### Symptoms
- The iOS build stops at ```Code Sign error: No code signing identities found: No valid signing identities (i.e. certificate and private key pair) were found.```

#### Solution
- Start Xcode
- Create a new, blank iOS project and try to build it
- When the error dialogue with ```No provisioning profiles matching an applicable signing identity were found.``` appears, click `Fix Issue` and let the wizard complete
- Close Xcode
- Now re-run your build/preview from Fuse

---

## Sublime plugin does not work

### Symptoms

- You get the message "Error loading: Packages/Fuse/UX.tmLanguage"
- You don't get syntax highlighting in Sublime

#### Solution

- Open the dashboard, click "Sublime Text Setup" and follow the wizard.
- Make sure that Sublime Text is installed in its default location, `/Applications/Sublime\ Text.app`
- If that didn't work, delete the files staring with `Fuse` in `%APPDATA%\Sublime Text 3\Installed Packages` (Windows) or `~/Library/Application Support/Sublime Text 3/Installed Packages` (OS X), and try the wizard again.

## Sublime plugin doesn't find Fuse
### Symptoms

- You get the message "Fuse could not be found"

#### Solution
- Make sure Fuse is installed
- If Fuse was recently installed, try to restart Sublime
- Make sure Fuse is in path. Our installer should take care of this, to verify, open a command line, run `fuse --version`, and check that it prints the Fuse version
- If the previous step does not work, log out and in again and try again
- If Fuse still can't be found by Sublime or on the command line, and you are on Windows, our installer probably failed to update the `PATH` environment variable. Try [adding](http://www.computerhope.com/issues/ch000549.htm#windows8) `<user directory>\AppData\Local\Fusetools\Fuse\App\Bin` to `PATH`, where `<user directory>` is typically `C:\Users\<your username>`.

---

## My Uno code is not run/updated in preview

### Symptoms

- Changes to your Uno code is not reflected in preview
- Your Uno code is not executed in preview

#### Explanation

- Unlike UX/JavaScript, Uno-code is not refreshed when you save and auto-reload preview, you'll have to rebuild your app. Use the Preview menu, or the keyboard shortcut listed there.
- In addition, any Uno-code you have in `ux.uno` files is not run at all in preview.
- All in all, it can be easier to work with Uno code through `uno build` than through preview. A better solution for this is coming soon.

---

## Preview says "Oops! Something went wrong here"

When previewing your app, the "Oops! Something went wrong here" screen appears.

#### Solution

- Refer to the log view at the bottom of the Fuse window.

---

## Local preview does not start on Windows

If the console output contains `GL_VERSION: 1.1.0` and `GL_RENDERER: GDI Generic` the problem is most likely missing / outdated OpenGL drivers. Upgrade to the most recent drivers for your graphics adapter and try again.

This problem can also be triggered by driver issues under Windows 10 specific to the Intel HD Graphics 2000 / 3000 / 4000 graphics adapters. In this case you will not be able to do local preview with instant updates, but we can still test your app on the PC by doing a regular build: `fuse build -t=dotnetexe --run`

We can of course also still use [preview on Android and iOS devices](preview-and-export.md).

## How to report a bug

If you think you've found a bug, we would appreciate you letting us know by submitting issues at the [fuselibs github repository](https://github.com/fuse-open/fuselibs/issues).

The more information we have on an issue, the easier it is for us to solve it. Here is a recommended list of information you should try to include in your bug reports.

1. What version of Fuse are you using? Run `fuse --version` in your terminal/cmd and copy the output.
2. Your operating system and its version.
3. If you are testing on a mobile device - which device is it and which OS version does it run?
4. On which targets does your issue occur; for example if you're testing on both Android and iOS, does your issue manifest on both platforms?
5. See if your issue is triggered both in preview and when exporting a build:
    - Local preview - run `fuse preview`
	- Device preview - run `fuse preview -tios` or `fuse preview -tandroid`
	- Export build - run `fuse build -tios` or `fuse build -tandroid`
6. A link to a minimal reproduction project and instructions on how to trigger the bug (be specific).
    - If the repro case is quite small, you can paste your code in the issue description.
    - Try to make the project as small as possible; only include the code that is needed to make the issue manifest.
