# iOS Capabilties

Capabilities are App Services which need special declarations in the Xcode project.

Fuse allows you to configure these through the `unoproj` file under the `iOS → SystemCapabilities` section.

The available Capabilities and example values are as follows:

```json
  "iOS": {
    "SystemCapabilities": {
      "ApplicationGroups": ["group.A", "group.B"],
      "DataProtection": true,
      "GameCenter": true,
      "HealthKit": true,
      "HomeKit": true,
      "InAppPurchase": true,
      "InterAppAudio": true,
      "KeychainSharing": ["AAA", "BBB"],
      "Push": true,
      "AssociatedDomains": ["CCC", "DDD"],
      "PersonalVPN": true,
      "WirelessAccessoryConfiguration": true
    }
  },
```

For more info on the valid values for `AssociatedDomains`, `KeychainSharing` & `ApplicationGroups` please see the [Apple documentation](https://developer.apple.com/library/content/documentation/IDEs/Conceptual/AppDistributionGuide/AddingCapabilities/AddingCapabilities.html)

Entitlements are described in the Apple Documentation as:

> .. a single right granted to a particular app, tool, or other executable that gives it additional permissions above and beyond what it would ordinarily have.

In most cases Fuse can accurately create the correct entitlement for your chosen capability and will do so. In the other cases you can configure it as usual in Xcode. To open Xcode at the end of a build simply add `--debug` command line build options, for example: `uno build ios --debug`
