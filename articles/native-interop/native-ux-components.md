
# Custom native control wrappers for UX markup

This tutorial takes you through how to create a custom wrapper for a native Android or iOS control using Uno and foreign code, and bootstrapping it so it can be used from UX Markup. As an example, we'll show how to wrap a slider control.

This tutorial assumes you are familiar with the native language of the platform you are working with, that is, Java for Android and Objective-C for iOS.

## Implementing `LeafView`

There are two types of views we can implement. `ParentView` allows you to wrap a view that contains other views (i.e. panels), while `LeafView` wraps a view that does not contain other views (e.g. buttons, switches, pickers). In this example, we'll focus on `LeafView` as this is what you'll most commonly need.

We start out by creating a class which defines our slider with some boilerplate code for handling properties. We also declare a pair of interfaces that we will use to communicate between native and our slider class.

	namespace Native
	{
		using Uno;
		using Uno.UX;

		using Fuse.Controls;

		public interface ISliderHost
		{
			void OnValueChanged(float newValue);
		}

		public interface ISlider
		{
			float Value { set; }
		}

		public class MySlider : Control, ISliderHost
		{
			float _value;
			[UXOriginSetter("SetValue")]
			public float Value
			{
				get { return _value; }
				set { SetValue(value, this); }
			}

			static readonly Selector _valueName = "Value";
			public void SetValue(float newValue, IPropertyListener origin)
			{
				if (_value != newValue)
				{
					_value = newValue;
					OnPropertyChanged(_valueName, origin);
				}
				if (origin != null)
				{
					var ns = NativeSlider;
					if (ns != null)
						ns.Value = newValue;
				}
			}

			void ISliderHost.OnValueChanged(float newValue)
			{
				SetValue(newValue, null);
			}

			ISlider NativeSlider
			{
				get { return NativeView as ISlider; }
			}
		}

	}

We move on to creating a wrapper for the native view, by extending the `LeafView` class(es). We need to make separate versions for iOS and Android, preferably in separate files. If you only want to support one platform, that's fine. We can then simply skip the steps for the other platform.

There are two versions of the `LeafView` class for iOS and Android respectively, in the namespaces `Fuse.Controls.Native.iOS` and `Fuse.Controls.Native.Android`.

> Remember that to access foreign code (i.e. the `Foreign` attribute), we must also add `using Uno.Compiler.ExportTargetInterop;`

## Implementing `LeafView` for iOS

We create our `LeafView` class in Uno with foreign Objective-C code:

	namespace Native.iOS
	{
		using Uno;
		using Uno.UX;
		using Uno.Compiler.ExportTargetInterop;

		using Fuse.Controls.Native.iOS;

		[Require("Source.Include", "UIKit/UIKit.h")]
		extern(iOS) public class MySlider: LeafView, ISlider
		{
			[UXConstructor]
			public MySlider([UXParameter("Host")]ISliderHost host) : base(Create()) { }

			[Foreign(Language.ObjC)]
			static ObjC.Object Create()
			@{
				::UISlider* slider = [[::UISlider alloc] init];
				[slider setMinimumValue:   0.0f];
				[slider setMaximumValue: 100.0f];
				return slider;
			@}

			public float Value
			{
				get { return GetValue(Handle); }
				set { SetValue(Handle, value); }
			}

			[Foreign(Language.ObjC)]
			static float GetValue(ObjC.Object handle)
			@{
				::UISlider* slider = (::UISlider*)handle;
				return [slider value];
			@}

			[Foreign(Language.ObjC)]
			static void SetValue(ObjC.Object handle, float value)
			@{
				::UISlider* slider = (::UISlider*)handle;
				[slider setValue:value animated:false];
			@}

			void OnValueChanged()
			{
				// TODO: implement value changed callback
			}
		}
	}

> The above example only implements setting the `Value`, it does not hook onto the `UIControlEventValueChanged` to feed events back to Fuse when the user interacts with the control. A complete useful wrapper would need that.

Note how the `LeafView` constructor expects to get an `ObjC.Object` as argument. We feed it by creating a `static` method where we instantiate the `UISlider` and call that as argument to the `base` constructor.

To get access to the `ObjC.Object` representing the `UISlider` from within your class later, we can use the `Handle` property, as seen in how we implement the `Value` property above.

The `MySlider` class has the `extern(iOS)` attribute, and will therefore only be available when building for iOS. To make it possible to use this class
in UX markup, it must be valid on all platforms. Therefore we must provide a dummy implementation with just the public interface marked with `extern(!iOS)` within the same namespace:

	namespace Native.iOS
	{
		extern(!iOS) public class MySlider
		{
			[UXConstructor]
			public MySlider([UXParameter("Host")]ISliderHost host) { }
		}
	}

## Implementing `LeafView` for Android

And then, let's write the equivalent Android implementation:

	namespace Native.Android
	{
		using Uno;
		using Uno.UX;
		using Uno.Compiler.ExportTargetInterop;

		using Fuse.Controls.Native.Android;

		extern(Android) public class MySlider : LeafView, ISlider
		{
			ISliderHost _host;
			[UXConstructor]
			public MySlider([UXParameter("Host")]ISliderHost host) : base(Create())
			{
				_host = host;AddChangedCallback(Handle, OnValueChanged);
			}

			float ISlider.Value
			{
				set { SetValue(Handle, value); }
			}

			[Foreign(Language.Java)]
	        static Java.Object Create()
	        @{
	            android.widget.SeekBar seekBar = new android.widget.SeekBar(@(Activity.Package).@(Activity.Name).GetRootActivity());
	            seekBar.setMax(100);
	            return seekBar;
	        @}

	        [Foreign(Language.Java)]
			void AddChangedCallback(Java.Object handle, Action<float> onValueChanged)
			@{
				((android.widget.SeekBar)handle).setOnSeekBarChangeListener(new android.widget.SeekBar.OnSeekBarChangeListener() {
					public void onProgressChanged(android.widget.SeekBar seekBar, int progress, boolean fromUser) {
						onValueChanged.run((float)progress);
					}
					public void onStartTrackingTouch(android.widget.SeekBar seekBar) { }
					public void onStopTrackingTouch(android.widget.SeekBar seekBar) { }
				});
			@}

			[Foreign(Language.Java)]
			void SetValue(Java.Object handle, float value)
			@{
				((android.widget.SeekBar)handle).setProgress((int)value);
			@}

			void OnValueChanged(float newValue)
			{
				_host.OnValueChanged(newValue);
			}
		}
	}

The above implementation also shows how to implement an `OnSeekBarChangeListener` to get events back from android to update the `Value` proeprty.

As for iOS, we also need a dummy implementation for `extern(!Android)` to allow this to be used in UX markup:

	namespace Native.Android
	{
		extern(!Android) public class MySlider
		{
			[UXConstructor]
			public MySlider([UXParameter("Host")]ISliderHost host) { }
		}
	}

### Creating a UX wrapper `Control`

The `LeafView` classes do not inherit @Visual, so they can not be used directly in a UX markup tree. We need to create an intermediary `Control` class that acts as a wrapper. This is nice, because it allows us to distinguish between the public API we expose to UX, and the implementation details of each platform. It also allows us to create a unified wrapper that will work cross-platform.

The `Fuse.Controls.Control` class contains a neat piece of magic. It allows you to specify multiple *templates* for different scenarios, and it will instantiate the correct template at runtime based on context:

* `AndroidTemplate` - will be used on Android, when inside a `NativeViewHost`
* `iOSTemplate` - will be used on iOS, when inside a `NativeViewHost`
* `GraphicsTemplate` - will be used in graphics contexts (when outside `NativeViewHost`), and as a fallback on desktop

So in order to make our `Slider` work on all platforms, we must provide one template for each scenario. We pass `this` to the `Host` attibuite to connect `Native.MySlider` with the native counterpart.

	<Native.MySlider ux:Class="NativeSlider">
		<Native.Android.MySlider ux:Template="AndroidAppearance" Host="this" />
		<Native.iOS.MySlider ux:Template="iOSAppearance" Host="this" />
		<Text ux:Template="GraphicsAppearance">
			MySlider is not available in this context.
		</Text>
	</Native.MySlider>

This hooks everything up, so if we use `<NativeSlider />` in UX markup now, it will use the correct implementation on each platform if we are inside a `NativeViewHost`. It will display an error `Text` message in other scenarios. 

If you want to, we can certainly provide a proper implementation of a slider as `GraphicsTemplate` instead of just displaying an error message.

### And done

Happy coding. This tutorial is work in progress. If you encounter problems, feedback is appreciated.
