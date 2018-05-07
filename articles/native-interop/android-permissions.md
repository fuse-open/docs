# Android Permissions API

Android has an interesting way of handling permissions. You first have to declare the permission in the `AndroidManifest.xml` file, and then at runtime some permissions need to be 'requested' which gives the user and opportunity to deny that permission after your app is installed.

It was not always this way however. Before Android 6 you only had to declare the permission in the manifest (and the runtime request api was not available).

Fuse unifies these two different declarations to hopefully make android's approach slightly easier to work with.

Let's look and an example and then we will break it down:


    public void TakePicture(object a1, EventArgs a2)
    {
        var permissionPromise = Permissions.Request(Permissions.Android.CAMERA); // [0]
        permissionPromise.Then(OnPermitted, OnRejected); // [1]
    }

    void OnPermitted(PlatformPermission permission)
    {
        debug_log "Woo, we can take the picture now";
    }

    void OnRejected(Exception e)
    {
        debug_log "Blast: " + e.Message;
    }

`[0]` - Here we request the permission by calling the `Request` method and passing in the permission we want to be granted. Simply be referencing that permission Fuse will add it to your `AndroidManifest.xml` file.

`[1]` - We now specify which method to call if the permission is granted or if the permission is rejected.

And that's all there is to it!

## What happens on older Android versions?

If you are on and older version of Android (from before the runtime permission request API) the request will always succeed.

## Can I request multiple permissions at once?

Yes! Just pass an array of `PlatformPermission` objects to the `Request` method and make sure your `OnPermitted` method also takes an array of `PlatformPermission` objects.

## What permissions are requestable by default?

- `ACCESS_CHECKIN_PROPERTIES`
- `ACCESS_COARSE_LOCATION`
- `ACCESS_FINE_LOCATION`
- `ACCESS_LOCATION_EXTRA_COMMANDS`
- `ACCESS_MOCK_LOCATION`
- `ACCESS_NETWORK_STATE`
- `ACCESS_SURFACE_FLINGER`
- `ACCESS_WIFI_STATE`
- `ACCOUNT_MANAGER`
- `ADD_VOICEMAIL`
- `AUTHENTICATE_ACCOUNTS`
- `BATTERY_STATS`
- `BIND_ACCESSIBILITY_SERVICE`
- `BIND_APPWIDGET`
- `BIND_DEVICE_ADMIN`
- `BIND_DREAM_SERVICE`
- `BIND_INPUT_METHOD`
- `BIND_NFC_SERVICE`
- `BIND_NOTIFICATION_LISTENER_SERVICE`
- `BIND_PRINT_SERVICE`
- `BIND_REMOTEVIEWS`
- `BIND_TEXT_SERVICE`
- `BIND_TV_INPUT`
- `BIND_VOICE_INTERACTION`
- `BIND_VPN_SERVICE`
- `BIND_WALLPAPER`
- `BLUETOOTH`
- `BLUETOOTH_ADMIN`
- `BLUETOOTH_PRIVILEGED`
- `BODY_SENSORS`
- `BRICK`
- `BROADCAST_PACKAGE_REMOVED`
- `BROADCAST_SMS`
- `BROADCAST_STICKY`
- `BROADCAST_WAP_PUSH`
- `CALL_PHONE`
- `CALL_PRIVILEGED`
- `CAMERA`
- `CAPTURE_AUDIO_OUTPUT`
- `CAPTURE_SECURE_VIDEO_OUTPUT`
- `CAPTURE_VIDEO_OUTPUT`
- `CHANGE_COMPONENT_ENABLED_STATE`
- `CHANGE_CONFIGURATION`
- `CHANGE_NETWORK_STATE`
- `CHANGE_WIFI_MULTICAST_STATE`
- `CHANGE_WIFI_STATE`
- `CLEAR_APP_CACHE`
- `CLEAR_APP_USER_DATA`
- `CONTROL_LOCATION_UPDATES`
- `DELETE_CACHE_FILES`
- `DELETE_PACKAGES`
- `DEVICE_POWER`
- `DIAGNOSTIC`
- `DISABLE_KEYGUARD`
- `DUMP`
- `EXPAND_STATUS_BAR`
- `FACTORY_TEST`
- `FLASHLIGHT`
- `FORCE_BACK`
- `GET_ACCOUNTS`
- `GET_PACKAGE_SIZE`
- `GET_TASKS`
- `GET_TOP_ACTIVITY_INFO`
- `GLOBAL_SEARCH`
- `HARDWARE_TEST`
- `INJECT_EVENTS`
- `INSTALL_LOCATION_PROVIDER`
- `INSTALL_PACKAGES`
- `INSTALL_SHORTCUT`
- `INTERNAL_SYSTEM_WINDOW`
- `INTERNET`
- `KILL_BACKGROUND_PROCESSES`
- `LOCATION_HARDWARE`
- `MANAGE_ACCOUNTS`
- `MANAGE_APP_TOKENS`
- `MANAGE_DOCUMENTS`
- `MASTER_CLEAR`
- `MEDIA_CONTENT_CONTROL`
- `MODIFY_AUDIO_SETTINGS`
- `MODIFY_PHONE_STATE`
- `MOUNT_FORMAT_FILESYSTEMS`
- `MOUNT_UNMOUNT_FILESYSTEMS`
- `NFC`
- `PERSISTENT_ACTIVITY`
- `PROCESS_OUTGOING_CALLS`
- `READ_CALENDAR`
- `READ_CALL_LOG`
- `READ_CONTACTS`
- `READ_EXTERNAL_STORAGE`
- `READ_FRAME_BUFFER`
- `READ_HISTORY_BOOKMARKS`
- `READ_INPUT_STATE`
- `READ_LOGS`
- `READ_PHONE_STATE`
- `READ_PROFILE`
- `READ_SMS`
- `READ_SOCIAL_STREAM`
- `READ_SYNC_SETTINGS`
- `READ_SYNC_STATS`
- `READ_USER_DICTIONARY`
- `READ_VOICEMAIL`
- `REBOOT`
- `RECEIVE_BOOT_COMPLETED`
- `RECEIVE_MMS`
- `RECEIVE_SMS`
- `RECEIVE_WAP_PUSH`
- `RECORD_AUDIO`
- `REORDER_TASKS`
- `RESTART_PACKAGES`
- `SEND_RESPOND_VIA_MESSAGE`
- `SEND_SMS`
- `SET_ACTIVITY_WATCHER`
- `SET_ALARM`
- `SET_ALWAYS_FINISH`
- `SET_ANIMATION_SCALE`
- `SET_DEBUG_APP`
- `SET_ORIENTATION`
- `SET_POINTER_SPEED`
- `SET_PREFERRED_APPLICATIONS`
- `SET_PROCESS_LIMIT`
- `SET_TIME`
- `SET_TIME_ZONE`
- `SET_WALLPAPER`
- `SET_WALLPAPER_HINTS`
- `SIGNAL_PERSISTENT_PROCESSES`
- `STATUS_BAR`
- `SUBSCRIBED_FEEDS_READ`
- `SUBSCRIBED_FEEDS_WRITE`
- `SYSTEM_ALERT_WINDOW`
- `TRANSMIT_IR`
- `UNINSTALL_SHORTCUT`
- `UPDATE_DEVICE_STATS`
- `USE_CREDENTIALS`
- `USE_SIP`
- `VIBRATE`
- `WAKE_LOCK`
- `WRITE_APN_SETTINGS`
- `WRITE_CALENDAR`
- `WRITE_CALL_LOG`
- `WRITE_CONTACTS`
- `WRITE_EXTERNAL_STORAGE`
- `WRITE_GSERVICES`
- `WRITE_HISTORY_BOOKMARKS`
- `WRITE_PROFILE`
- `WRITE_SECURE_SETTINGS`
- `WRITE_SETTINGS`
- `WRITE_SMS`
- `WRITE_SOCIAL_STREAM`
- `WRITE_SYNC_SETTINGS`
- `WRITE_USER_DICTIONARY`
- `WRITE_VOICEMAIL`
