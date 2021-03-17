# Debugging

This guide outlines how to set up a JavaScript debugger for use with Fuse. It
is possible to debug, both when running preview and when running normal builds,
any target that uses the V8 JavaScript engine. The iOS target, which doesn't
use V8, can also be debugged as long as the device has iOS 8 or newer, but the
debugging procedure is slightly different.

## Local preview, DotNet and Android (V8)

[Click here](https://youtu.be/GloEpBr2lK0) for a video tutorial showing you how to do this.

Targets that use the V8 JavaScript engine include Android, DotNet (including
local preview) on both macOS and Windows, CMake on macOS, and MSVC on Windows.

Fuse uses the V8 JavaScript engine and its debugger protocol. Fuse applications
that are built using the `-DDEBUG_V8` automatically listen for connections on a
local socket, port 5858. While any debugger that uses the V8 debugger protocol
should work in principle, we recommend using [Visual Studio Code](https://code.visualstudio.com/),
which is available both for macOS and Windows.

### Setup

* Download and install [Visual Studio Code](https://code.visualstudio.com/).
* (Android only) Forward port 5858 on your device to your host. This can be
  done using `adb`, which is bundled with Fuse and can be invoked using `uno adb`.
  Using a standard Fuse installation and with a single Android device
  connected, run (in a terminal or command prompt):

  ```sh
  uno adb forward tcp:5858 tcp:5858
  ```

  It's likely that you will have to re-run this command every time you
  reconnect your device. Also remember to remove the forwarding using e.g.
  `uno adb forward --remove-all` if you want to debug another target after you
  are done with Android. Otherwise the debugger port will be occupied.
* Open your project folder in Visual Studio Code.
* Press the Debug icon. Click on the green triangle (the "play" icon) and
  select `Node.js` as the Debug Environment.
* Select `Attach` in the drop-down list next to the green triangle.

### Starting the debugger

* Set a breakpoint in your JavaScript code by adding a `debugger;` statement at
  some location where it will be run. For example, try putting it at the very
  start of your JavaScript code, like the following:

  ```js
  <JavaScript>
    debugger;
    ... more code
  </JavaScript>
  ```

  It can also be useful to break when some event is triggered:

  ```js
  function someEvent() {
    debugger;
    ... more code
  }
  ```

* Build and run your app with `-DDEBUG_V8`. For example, run

  ```sh
  fuse preview . -DDEBUG_V8
  ```

  to preview the project in the current working directory locally or

  ```sh
  uno build android --run -DDEBUG_V8
  ```

    to build and run on Android.

* When your app is running, press the green triangle in Visual Studio Code to
  attach the debugger.
  On Windows, if Windows Firewall asks for access for your app, allow it.
* When you hit the breakpoint that was previously added, the JS code will pause
  and Visual Studio Code will show the location in the code and allow you to
  view local variables, step into your code, continue, and so on.


## iOS

[Click here](https://youtu.be/EDjymiMxHSw) for a video tutorial showing you how to do this.

### Setup

* Make sure your device is using iOS 8 or newer.
* Enable Safari Web Inspector on the device. This is done in the Settings app
  under `Safari/Advanced/Web Inspector`.
* Enable the Develop menu in Safari on the host computer. This is done in
  Safari's Preferences menu under `Advanced/Show Develop menu in menu bar`.

### Starting the debugger

* Build and run your project on the device.
* Open Safari on the host computer and click the Develop menu in the menu bar.
  There should be a sub-menu named after your device, where we can click
  `JSContext`. This will open a Web Inspector instance allowing you to view and
  debug the JavaScript code of your app.
