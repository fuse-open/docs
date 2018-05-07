# Start an Android Activity

Many Android APIs want you to communicate with other apps and services via an `Intent`. Creating these is easy, but receiving the results requires adding code to your apps `Activity` which is ugly in terms of code separation.

To make this a little nicer to work with from Uno, we have an API you can use. It is used in the following fashion:

- First add `Android.ActivityUtils` to the 'Packages' section of your `unoproj` file.
- Then make an Android intent object you wish to start. This is best done via foreign code.
- Finally call `ActivityUtils.StartActivity` with the intent object and method to call on completion

For example the code to take a picture on Android looks like this:

	using Uno;
	using Uno.Graphics;
	using Uno.Platform;
	using Uno.Collections;
	using Fuse;
	using Fuse.Controls;
	using Fuse.Triggers;
	using Fuse.Resources;
	using Android;
	using Uno.Compiler.ExportTargetInterop;

	[ForeignInclude(Language.Java,
					"android.app.Activity",
					"android.content.Intent",
					"android.net.Uri",
					"android.os.Bundle",
					"android.provider.MediaStore",
					"java.io.File")]
	public partial class MainView
	{
		void ClickPlay(object a1, EventArgs a2)
		{
			var intent = MakeMyIntent(); // [0]
			if (intent!=null)
			{
				ActivityUtils.StartActivity(intent, OnResult);  // [1]
			} else {
				debug_log "Failed to make intent.";
			}
		}

		extern(android) void OnResult(int resultCode, Java.Object intent, object info)  // [2]
		{
			debug_log "Success!: " + resultCode + " - " + intent + "\n";
		}

		[Foreign(Language.Java)]
		static extern(android) Java.Object MakeMyIntent()
		@{
			try {
				String fileName = "JPEG_TEST";
				String ext = ".jpg";
				File storageDir = com.fuse.Activity.getRootActivity().getExternalFilesDir(null);
				File photoFile = File.createTempFile(fileName, ext, storageDir);
				Intent takePictureIntent = new Intent(MediaStore.ACTION_IMAGE_CAPTURE);
				Bundle bdl = new Bundle();
				bdl.putParcelable(MediaStore.EXTRA_OUTPUT, Uri.fromFile((File)photoFile));
				takePictureIntent.putExtras(bdl);
				return takePictureIntent;
			} catch (Exception ex) {
				return null;
			}
		@}

		extern(!android) void OnResult(int resultCode, object intent)
		{
		}

		static extern(!android) object MakeMyIntent()
		{
			return null;
		}
	}

`[0]` - First we call a foreign method to create the `Intent` object in the usual Android fashion.
`[1]` - Here we call `StartActivity` with our intent and the method to call on completion.
`[2]` - Lastly we have the method that gets called when the result is received.

The info object allows us to associate some Uno data with the request. This data *is not* passed to the `Activity` being started but is stored and passed to the Uno callback when the result arrives.

## Can I start an Activity which doesn't have a result?

Certainly! Simply call `StartActivity` and only pass in the intent object without supplying a callback.
