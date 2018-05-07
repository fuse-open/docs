# Facebook login using the Facebook SDK through foreign code

Fuse's foreign code feature allows you to integrate native code with Uno code.
We currently support Java on Android and Objective-C on iOS.  This example
shows how to use this feature to integrate the Facebook SDK with Fuse to get
Facebook's login functionality.

This example is meant to be a free-standing introduction to foreign code, but
it might be useful to consult [the technical documentation](../native-interop/foreign-code.md)
which for example explains the marshalling between Uno types and native types
in more detail. This guide also assumes some fluency in Uno, our C# dialect. See
[the Uno language reference](../uno/uno-lang.md) for more information about Uno.

## Android

We'll be following the [Facebook login Android guide](https://developers.facebook.com/docs/facebook-login/android) closely, so
if you want to follow along in your own code you should make sure that you have
gone through their prerequisites.  We'll use the `LoginManager` class to
initiate login by pressing a Fuse button.

### Triggering and responding to login

The main login functionality is implemented in an Uno method with the following
signature:

```
public void Login(Action<AccessToken> onSuccess, Action onCancelled, Action<string> onError)
```

This method is contained in the `FacebookLogin` class and accepts actions that will
be invoked depending on the outcome of the login.

To be able to write the method body in Java we add the
`[Foreign(Language.Java)]` attribute to it and use `@{` and `@}` in place of
normal braces. The special braces tell Uno to not parse the code as normal Uno
code, and the attribute tells the compiler what language the code is actually
written in (Java in this case). We can only use this version of the method on
Android, so we use `extern(Android)` to only compile it when targeting Android.

```
[Foreign(Language.Java)]
public extern(Android) void Login(Action<AccessToken> onSuccess, Action onCancelled, Action<string> onError)
@{
@}
```

The `AccessToken` class, used as the argument to the `onSuccess` action, is a
thin wrapper around a Java object:

```
public class AccessToken
{
	extern(Android) Java.Object _token;

	public extern(Android) AccessToken(Java.Object token)
	{
		_token = token;
	}
}
```

The Uno class `Java.Object` provides a way to hold on to a Java `Object` on the
Uno side, making it possible to store and pass around results obtained from
Java code in Uno.

Note: The `AccessToken` class is not strictly necessary here, but serves as
documentation on the type of the Java object that we're expecting. It could also
serve as the basis for an implementation providing a way to get at the info
contained in the access token.

We will also be using a piece of state stored in the surrounding Uno object
(`FacebookLogin`). This state is a `Java.Object _callbackmanager` containing a
`CallbackManager` from the Facebook SDK. We will show how this state is
initialised in the next section.

We are now ready to implement the body of the `Login` method in Java.

To start with, we use a macro  to get the `callbackManager` from the `_this` object:

```
@{
	CallbackManager callbackManager = (CallbackManager)@{FacebookLogin:Of(_this)._callbackManager:Get()};
	...
@}
```

The `_this` object in foreign code corresponds to the Uno object that the
method is defined in (assuming it's not a static method).  In words, the macro
(the `@{...}` part) says something like "get the _callbackManager field of
_this, which is an Uno object of type `FacebookLogin`" (see [the foreign code
documentation](../native-interop/foreign-code.md) for more details). `Java.Object`s in Uno are
converted into plain Java `Object`s on the Java side, so we cast it to
`CallbackManager`, the type that we know that it has.

Next, we register our callbacks with the Facebook `LoginManager`:
```
LoginManager.getInstance().registerCallback(callbackManager,
	new FacebookCallback<LoginResult>()
	{
		@Override
		public void onSuccess(LoginResult loginResult)
		{
			AccessToken accessToken = loginResult.getAccessToken();
			Object unoAccessToken = @{AccessToken(Java.Object):New(accessToken)};
			onSuccess.run(unoAccessToken);
		}

		@Override
		public void onCancel()
		{
			onCancelled.run();
		}

		@Override
		public void onError(FacebookException exception)
		{
			onError.run(exception.toString());
		}
	}
);
```

In the `onSuccess` method, we first get the `AccessToken` from the
`LoginResult`, and then create an `Uno` `AccessToken` (the class that we
defined above), `unoAccessToken`, from it using the UXL `:New` macro, which
results in an `UnoObject`. We get an `UnoObject` for Uno object when we are
working on the Java side. `UnoObject` is somewhat like the opposite of
`Java.Object` in Uno: it is a way to hold on to and pass around Uno objects in
Java.

There is special support for passing delegates to Foreign methods. If the
delegate is an Uno `Action`, it is automatically converted to a runnable in
Java. If the delegate has parameters or returns a value, it's converted to a
custom class with a `run` method that matches the signature of the delegate,
only with the arguments and return types converted to their Java equivalent. So
in the `onSuccess` method, we simply call the `onSuccess` delegate by using the
`run` method, passing it the `unoAccessToken` that we just created.

We handle the `onCancel` and `onError` cases similarly. Note that the Java
`String` used as an argument to `onError.run` is automatically converted to an
Uno `string` when the function is invoked.

### Setup

#### Initialization

The Facebook documentation instructs us to perform calls at various stages in
the lifecycle of our app. The lifecycle of Fuse applications is not handled
exactly the same way as it is in normal Android apps, so we can't do this
exactly like in Facebook's documentation.

First, we hook our `FacebookLogin` class up to get callbacks when the relevant
events are triggered. This is achieved by using the `Lifecycle` class in
`Fuse.Platform`.  In the constructor of `FacebookLogin`, we do:

```
public FacebookLogin()
{
	Lifecycle.Started += Started;
	Lifecycle.EnteringInteractive += OnEnteringInteractive;
	Lifecycle.ExitedInteractive += OnExitedInteractive;
}
```

Our implemententation of `Started` looks like this:

```
[Foreign(Language.Java)]
extern(Android) void Started(ApplicationState state)
@{
	FacebookSdk.sdkInitialize(Activity.getRootActivity());
	final CallbackManager callbackManager = CallbackManager.Factory.create();
	@{FacebookLogin:Of(_this)._callbackManager:Set(callbackManager)};
	Activity.subscribeToResults(new Activity.ResultListener()
	{
		@Override
		public boolean onResult(int requestCode, int resultCode, Intent data)
		{
			return callbackManager.onActivityResult(requestCode, resultCode, data);
		}
	});
@}
```

On the first line we use `Activity.getRootActivity()` to get the Activity of
the application. This method comes from a Fuse-specific class called
`com.fuse.Activity`. Next, we create a `CallbackManager` (from the Facebook
SDK), and use a UXL macro to save its value in the `_this` object for later
use. We can't directly override the `onActivityResult` method in Fuse as the
Facebook SDK documentation says, but there is a hook, again in
`com.fuse.Activity`, that we can add a listener to to get those callbacks. This
is what the `subscribeToResults` call does.

#### Imports and Gradle dependencies

The foreign Java code in an Uno class all gets compiled to the same file.  If
the Java code needs imports, which it does in this case, we can use the
`ForeignInclude` attribute on that class to add it to the generated Java file.
For our `FacebookLogin` class, it looks like this:

```
[ForeignInclude(Language.Java, "android.content.Intent")]
[ForeignInclude(Language.Java, "com.facebook.*")]
[ForeignInclude(Language.Java, "com.facebook.appevents.AppEventsLogger")]
[ForeignInclude(Language.Java, "com.facebook.login.*")]
[ForeignInclude(Language.Java, "com.fuse.Activity")]
[ForeignInclude(Language.Java, "com.fusefacebook.ActivityResultListener")]
public class FacebookLogin
{
...
}
```

We additionally need to add the Facebook SDK itself. Uno supports the Gradle
build system on Android, provided we build with the `-DGRADLE` flag, and
generates a Gradle build file each time the app is built. We can instruct the
compiler to add items to the `dependencies` and `repositories` sections by
adding the following attributes to the class:

```
[Require("Gradle.Dependency","compile('com.facebook.android:facebook-android-sdk:4.8.+') { exclude module: 'support-v4' }")]
[Require("Gradle.Repository","mavenCentral()")]
```

### Changing the Android manifest

The Facebook SDK requires some changes to the `AndroidManifest.xml` file. This
is another file that Uno generates when we build the app. We can get changes
added to it in a way similar to how we added the Gradle dependencies. This time
the changes are a bit too big to comfortably add as an attribute, so we will be
using an `.uxl` file to to specify them, which looks like this:

```
<Extensions Backend="CPlusPlus">
  <Set Facebook.AppID="YOUR-FACEBOOK-APP-ID" />
  <Require Condition="Android" Android.ResStrings.Declaration>
    <![CDATA[
      <string name="FACEBOOK_APP_ID">@(Facebook.AppID)</string>
    ]]>
  </Require>
  <Require Condition="Android" AndroidManifest.ApplicationElement>
    <![CDATA[
      <meta-data android:name="com.facebook.sdk.ApplicationId" android:value="@string/FACEBOOK_APP_ID" />
      <activity android:name="com.facebook.FacebookActivity"
              android:configChanges=
                     "keyboard|keyboardHidden|screenLayout|screenSize|orientation"
              android:theme="@android:style/Theme.Translucent.NoTitleBar"
              android:label="com.uno.test" />
    ]]>
  </Require>
</Extensions>
```

This first line creates a variable with our Facebook application ID, which we
refer to later with `@(Facebook.AppID)`.

We also `Require` a few elements, which basically means that we inform the
Uno compiler about things that we want to add in specific places in the
generated build files. For example, the `AndroidManifest.ApplicationElement`
element allows you to add a child to the application element of the
`AndroidManifest.xml`.
For more information about these kinds of settings, see
the [build settings](../native-interop/build-settings.md) documentation.

## iOS

We'll be following the [Facebook login iOS
guide](https://developers.facebook.com/docs/facebook-login/ios) closely, so,
again, if you want to follow along in your own code, make sure that you have
gone through their prerequisites. Here we will be using an `FBSDKLoginManager`
to initiate login by pressing a Fuse button.

### Triggering and responding to login

We use the same hooks as for our Android implementation to get lifecycle
events.  The iOS implementation is a little bit easier, because we do not need
to store any state in the surrounding Uno object.

The login method is implemented using foreign Objective-C as follows:

```
[Foreign(Language.ObjC)]
public extern(iOS) void Login(Action<AccessToken> onSuccess, Action onCancelled, Action<string> onError)
@{
	FBSDKLoginManager* login = [[FBSDKLoginManager alloc] init];
	[login
		logInWithReadPermissions: @[@"public_profile"]
		fromViewController: [[[UIApplication sharedApplication] keyWindow] rootViewController]
		handler: ^(FBSDKLoginManagerLoginResult* result, NSError* error)
		{
			if (error)
			{
				onError([error localizedDescription]);
				return;
			}
			if (result.isCancelled)
			{
				onCancelled();
				return;
			}
			id<UnoObject> unoAccessToken = @{AccessToken(ObjC.Object):New(result.token)};
			onSuccess(unoAccessToken);
		}
	];
@}
```

We first create an `FBSDKLoginManager` and initiate login using that, calling
back to the given delegates as appropriate.

Uno delegates passed to Objective-C are automatically converted to blocks with
the corresponding signature, so they can be called like ordinary functions on
the Objective-C side. The `string` argument to the `onError` callback is automatically
converted from an `NSString*`, so we use `[error localizedDescription]` to get the
error string in the case of an error and pass that to the callback.

We again use the UXL `:New` macro to create an Uno `AccessToken` object used in
the `onSuccess` callback. We can hold on to Uno objects in Objective-C using
the type `id<UnoObject>`.

### Setup

#### Initialization

The initialization on iOS is similar to that on Android, only using the
Objective-C interface to the SDK.

In the application `Started` hook, we do the following:

```
[Foreign(Language.ObjC)]
extern(iOS) void Started(ApplicationState state)
@{
	[[FBSDKApplicationDelegate sharedInstance]
		application: [UIApplication sharedApplication]
		didFinishLaunchingWithOptions: nil];
@}
```

Similarly, we invoke `activateApp` when entering interactive:

```
[Foreign(Language.ObjC)]
static extern(iOS) void OnEnteringInteractive(ApplicationState state)
@{
	[FBSDKAppEvents activateApp];
@}
```

On iOS we also need to pass received URIs to the Facebook SDK for it to know
when a login is successful. We can hook onto such events using
`Fuse.Platform.InterApp.ReceivedURI`, so we add to the constructor:

```
public FacebookLogin()
{
	...
	InterApp.ReceivedURI += OnReceivedUri;
}
```

When we get a callback with an URI that starts with `fb`, we pass it on to
Facebook:

```
static void OnReceivedUri(string uri)
{
	if (uri.StartsWith("fb"))
	{
		OpenFacebookURL(uri);
	}
}

[Foreign(Language.ObjC)]
static extern(iOS) void OpenFacebookURL(string url)
@{
	[[FBSDKApplicationDelegate sharedInstance]
		application: [UIApplication sharedApplication]
		openURL: [NSURL URLWithString:url]
		sourceApplication: @"com.apple.mobilesafari"
		annotation: nil];
@}
```

#### Adding the framework and getting the imports right

This assumes that you have downloaded and extracted the Facebook SDK for iOS in
a folder called `FacebookSDKs-iOS` in the folder that the Uno project is
in.

Uno generates an Xcode project each time we build for iOS. To get the Facebook
SDK framework added to the project, we add the following attributes to the Uno
class:

```
[Require("Xcode.FrameworkDirectory", "@('FacebookSDKs-iOS':Path)")]
[Require("Xcode.Framework", "@('FacebookSDKs-iOS/FBSDKCoreKit.framework':Path)")]
[Require("Xcode.Framework", "@('FacebookSDKs-iOS/FBSDKLoginKit.framework':Path)")]
public class FacebookLogin
{
...
}
```

We use the `@('relative/path':Path)` syntax to specify a path relative to the
current file. We do this since the default is to look for frameworks in the
iPhone SDK system path (which only works for the frameworks that Apple bundle
with the SDK). We also need to require the `Xcode.FrameworkDirectory` to
ensure that Xcode looks for headers in that directory.

(As a side note, it's also possible to embed frameworks by using
`[Require("Xcode.EmbeddedFramework", "...")]`.
For more information about these kinds of settings, see
the [build settings](../native-interop/build-settings.md) documentation.)

Next, we need to make sure that the file generated from our Uno class that uses
the Facebook SDK in foreign code has the required includes. We can achieve that
like this since we're using entities from `CoreKit` and `LoginKit`:

```
[ForeignInclude(Language.ObjC, "FBSDKCoreKit/FBSDKCoreKit.h")]
[ForeignInclude(Language.ObjC, "FBSDKLoginKit/FBSDKLoginKit.h")]
```

#### Plists

The Facebook SDK requires us to add some elements to our Xcode project's
`plist` file.  Since that is generated on build, we do this in UXL, in the same
file that we used to add elements to the AndroidManifest:

```
<Require Condition="iOS" Xcode.Plist.Element>
<![CDATA[
  <key>CFBundleURLTypes</key>
  <array>
    <dict>
      <key>CFBundleURLSchemes</key>
      <array>
        <string>fb@(Facebook.AppID)</string>
      </array>
    </dict>
  </array>
  <key>FacebookAppID</key>
  <string>@(Facebook.AppID)</string>
  <key>FacebookDisplayName</key>
  <string>Fuse Test App</string>

  <key>NSAppTransportSecurity</key>
  <dict>
    <key>NSExceptionDomains</key>
    <dict>
      <key>facebook.com</key>
      <dict>
        <key>NSIncludesSubdomains</key> <true/>
        <key>NSThirdPartyExceptionRequiresForwardSecrecy</key> <false/>
      </dict>
      <key>fbcdn.net</key>
      <dict>
        <key>NSIncludesSubdomains</key> <true/>
        <key>NSThirdPartyExceptionRequiresForwardSecrecy</key>  <false/>
      </dict>
      <key>akamaihd.net</key>
      <dict>
        <key>NSIncludesSubdomains</key> <true/>
        <key>NSThirdPartyExceptionRequiresForwardSecrecy</key> <false/>
      </dict>
    </dict>
  </dict>

  <key>LSApplicationQueriesSchemes</key>
  <array>
    <string>fbauth2</string>
  </array>
  ]]>
</Require>
```

## Get the full project

The full project which works on both iOS and Android can be downloaded [here](https://github.com/fusetools/fuse-samples/tree/feature-NativeFacebookLogin/Samples/NativeFacebookLogin).

The full project also hooks up the login functionality in a
[NativeModule](../native-interop/native-js-modules.md) such that the login
function is available from JavaScript, returning a promise.

