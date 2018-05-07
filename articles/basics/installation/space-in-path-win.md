# Non-standard paths and Android builds

On Windows, Fuse does not support building for Android if it is installed in a path with spaces or other non-standard characters. This is due to limitations in GNU make, which we depend on for building Android projects. The most common scenario is when you have a space in your user name.

## Workaround

If you can't avoid having a user name with a space in it, please use the following workaround:

- Move the directory `C:\Users\<Your User Name>\AppData\Local\Fusetools\Fuse\App` to a directory without a space, for instance `C:\Fuse\App`.
- Update your `%PATH%` environment variable to point to the `Bin` folder in the new Fuse location, for instance `C:\Fuse\App\Bin`.
- *Optional*: If you already have Android NDK, SDK and/or ant installed, move them to the location you just configured in the previous step, to avoid having to re-install them.
- In a command prompt, run `fuse install android`.
- In a command prompt, run `uno clean` in your project root directory.

PS: Whenever you upgrade Fuse, you have to move the `App` directory and update `%PATH%` again.
