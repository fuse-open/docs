# Android.ActivityUtils

The `Android.ActivityUtils` package aims to help with interacting with the root Android `Activity` from Uno.

It currently provides:

- a way to get the root `Activity` as a `Java.Object`
- a way to launch an `Activity` optionally providing a Uno callback and some associated Uno data.

## Using the library

Simply include `Android.ActivityUtils` in the `Packages` list in your `unoproj` file. Something like:

```
{
	"Packages": [
		"Fuse",
		"FuseJS",
		"Android.ActivityUtils",
	],
	"Includes": [
		"*"
	],
}
```

## Retrieving the root Activity

Simply call `ActivityUtils.GetRootActivity()`. This will return the root `Activity` object as a `Java.Object`.

Naturally this will not work on non-android builds where it will instead return `null`.

## Starting an Activity

This is achieved with the `StartActivity` method. This method has 3 overloads:

- `StartActivity(Java.Object _intent)`
- `StartActivity(Java.Object intent, ActivityResultCallback callback)`
- `StartActivity(Java.Object intent, ActivityResultCallback callback, object info)`

The first simply calls android's [startActivity](https://developer.android.com/reference/android/app/Activity.html#startActivity(android.content.Intent)) method on the intent.

The second calls [startActivityForResult](https://developer.android.com/reference/android/app/Activity.html#startActivityForResult(android.content.Intent,%20int)) but will call your callback when the result from the Activity is ready.

The third does the same as the second but let's you pass any uno object to `StartActivity`. This object is not sent, but is instead passed to your callback along with the result when the result is ready.

`ActivityResultCallback` is a delegate defined as `delegate void ActivityResultCallback(int resultCode, Java.Object intent, object info)`

Here is an example of a valid callback method in Uno

```
extern(android) void OnResult(int resultCode, Java.Object intent, object info)
{
	debug_log "\nblam!: "+resultCode+" - "+intent + "\n";
}
```

If you used the second overload of `StartActivity` (the one without the `info` object) then `info` in the callback will be `null`.
