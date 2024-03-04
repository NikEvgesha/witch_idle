# justtrack SDK for Unity

![Logo](https://sdk.justtrack.io/docs/justtrack-sdk/unity/logoGray-f38e4838-7664-45f9-bcf7-5479cc8a54b2.png)

## Installation

The justtrack SDK is available as a Unity package.
You have to add <https://registry.npmjs.org> as the scoped registry to add it to your game.
Navigate to `Window` → `Package Manager`, then select `Advanced Project Settings` from the gear menu.
Now add the justtrack Package Registry to your game:

![Scoped Registry](https://sdk.justtrack.io/docs/justtrack-sdk/unity/scopedRegistry-fe08365d-d1e7-4714-ac55-eefedeeead46.png)

After adding the scoped registry, you need to add the justtrack SDK to your project.
Select `Packages: My Registries` in the drop-down menu, select the `justtrack SDK` and install it into your game.
Use the latest available version if possible.

![Adding the package](https://sdk.justtrack.io/docs/justtrack-sdk/unity/addingPackage-a3f41bb3-1f5d-4273-82fd-0a3f6e1145e8.png)

You can also add the scoped registry and package directly to your `Packages/manifest.json`.
Afterwards, your `manifest.json` should look like this:

```json
{
  "dependencies": {
    // ... other dependencies of your game ...
    "io.justtrack.justtrack-unity-sdk": "4.4.0"
  },
  "scopedRegistries": [
    // ... other scoped registries ...
    {
      "name": "justtrack Package Registry",
      "url": "https://registry.npmjs.org",
      "scopes": [
        "io.justtrack"
      ]
    }
  ]
}
```

### Dependencies

The justtrack SDK uses the [External Dependency Manager for Unity](https://github.com/googlesamples/unity-jar-resolver) from Google to download the justtrack SDK as well as its dependencies.
You can select the correct version for you on <https://developers.google.com/unity/archive#external_dependency_manager_for_unity> or directly download <https://github.com/googlesamples/unity-jar-resolver/raw/v1.2.175/external-dependency-manager-1.2.175.unitypackage> to get the latest version.
The justtrack SDK requires at least version 1.2.167.

### Adding the prefab

After finishing the installation of the justtrack SDK and its dependencies you should see a new menu called `justtrack`.
Navigate to the first scene of your game and then select `Create SDK instance` from the `justtrack` menu.

Select the instance of the prefab and add your API tokens for Android and iOS as applicable.
An API token looks like this:

- `prod-f067440a10b63a5f49d5d9cfa1f98770bd6a329abad730bc2533789b85a124a7`

### Getting an API token

Navigate to your apps on the [justtrack dashboard](https://dashboard.justtrack.io/admin/apps) and view your app.
If you haven't created your app on the dashboard yet, go to [Products](https://dashboard.justtrack.io/admin/products)
and create the product for your app if needed, then go to [Apps](https://dashboard.justtrack.io/admin/apps) and create your app.
This will generate the API token needed to use the justtrack SDK and it should look like this:

![Example app](https://sdk.justtrack.io/docs/justtrack-sdk/unity/exampleApp-2a7aa56c-5f82-4fa9-b5d9-0c54c0bbdf18.png)

Thus, for this example app, you would be using `prod-c6654a0ae88b2f21111b9d69b4539fb1186de983f0ad826f0febaf28e3e3b7ed` as the API token.

## Configuration

The minimal configuration of the SDK consists of setting the correct API token (see the [install instructions](./INSTALL.md)).
If you are using **Appsflyer** as the attribution provider instead of justtrack, you have to set the attribution provider accordingly.
You have to integrate the Appsflyer SDK in your app, otherwise the justtrack SDK can't pick up the Appsflyer ID of the user.

### Getting the justtrack user ID

You can retrieve the justtrack user ID using `JustTrackSDK.GetUserId`:

```cs
using JustTrack;

JustTrackSDK.GetUserId((userId) => {
    Debug.Log(userId); // use the justtrack user ID somehow...
}, (error) => {
    // the SDK is not correctly configured or there was a problem generating the justtrack user ID
});
```

## Get an attribution

The justtrack SDK determines on each start (if necessary and not cached) the identity and attribution of a user.
This includes information about the ad the user clicked on (if any), to which campaign that ad belongs as well as the network and channel of that campaign.
The following example shows you you can access the campaign used to acquire the current user:

```cs
using JustTrack;

JustTrackSDKBehaviour.GetAttribution((attribution) => {
    Debug.Log(attribution.Campaign.Name);
}, (error) => {
    // handle error and wait for the attribution request to be retried automatically
});
```

If the attribution failed because we could not reach the justtrack backend (user might be offline), we will automatically retry it as soon as network connectivity is restored.
In that case, you need to subscribe to the `OnAttributionResponse` callback to be notified about the attribution.
Keep in mind that the callback can also be called if a user is attributed to a retargeting campaign.

```cs
using JustTrack;

JustTrackSDK.OnAttributionResponse += (attribution) => {
    Debug.Log(attribution.Campaign.Name);
};
```

The `attribution` variable will be an instance of the `AttributionResponse` class.
It looks like this:

```cs
namespace JustTrack {
    public class AttributionResponse {
        public string UserId { get; }
        public string InstallId { get; }
        public AttributionCampaign Campaign { get; }
        public string UserType { get; }
        public string Type { get; }
        public AttributionChannel Channel { get; }
        public AttributionNetwork Network { get; }
        public string SourceId { get; }
        public string SourceBundleId { get; }
        public string SourcePlacement { get; }
        public string AdsetId { get; }
        public AttributionRecruiter Recruiter { get; }
        public DateTime CreatedAt { get; }
    }

    public class AttributionCampaign {
        public int Id { get; }
        public string Name { get; }
        public string Type { get; }
    }

    public class AttributionChannel {
        public int Id { get; }
        public string Name { get; }
        public bool Incent { get; }
    }

    public class AttributionNetwork {
        public int Id { get; }
        public string Name { get; }
    }

    public class AttributionRecruiter {
        public string AdvertiserId { get; }
        public string UserId { get; }
        public string PackageId { get; }
        public string Platform { get; }
    }
}
```

## Getting the Advertiser and Test Group Id

The justtrack SDK can provide you with the advertiser id (GAID or IDFA) of the user if the user didn't limit ad tracking.
Additionally, users are divided into three test groups (1, 2, and 3) based on their advertiser id (Android) or IDFV (iOS).
You can retrieve that test group id from the SDK, implement a different logic for one of the test groups, and then compare the (different) performance of that group on the justtrack dashboard.

```cs
using JustTrack;

JustTrackSDK.GetAdvertiserIdInfo((info) => {
  // info.AdvertiserId will contain the advertiser id (lowercase) or null if it is not available
  // info.IsLimitedAdTracking tells you if the user opted out of ad tracking (the advertiser id might be null in that case depending on the Android version)
}, (error) => {
    // we failed to retrieve the advertiser id - this is a different case than the user limiting ad tracking
});
JustTrackSDK.GetTestGroupId((testGroupId) => {
    // testGroupId is either 1, 2, 3, or null
    // if it is null we could not retrieve an advertiser id or IDFV internally
}, (error) => {
    // we failed to determine the test group id. This will happen if retrieving the advertiser id fails
});
```

## Providing your own User Id

If you already have a mechanism to assign a unique id to each user, you can share this information with the justtrack SDK.
This then allows the backend of the justtrack SDK to associate events received from third parties via that user id with the correct user on justtrack side.
You can provide as an id multiple times, the justtrack SDK takes care about sending it to the justtrack backend as needed.

You can supply the own user id at any time while your game is running.
Should your own user id change for some reason, you have to supply the new value to the justtrack SDK again:

```cs
using JustTrack;

string ownUserId = /* get your own user id from your game */;
JustTrackSDK.SetCustomUserId(ownUserId);
```

A custom user id must be shorter than 4096 characters and only consist of printable ASCII characters (U+0020 to U+007E).

## User events

To track how far a user is progressing through your app you can send a user event at specific points to the justtrack backend.
Using this, you can determine whether there are any specific steps at which a lot of users drop out (e.g., at a specific level or before making some in-app purcharse).
The following calls are all equal and publish a user event called `event_name` to the backend:

```cs
using JustTrack;

JustTrackSDK.PublishEvent("event_name");
JustTrackSDK.PublishEvent(new EventDetails("event_name"));
JustTrackSDK.PublishEvent(new CustomUserEvent("event_name"));
JustTrackSDK.PublishEvent(new CustomUserEvent(new EventDetails("event_name")));
```

You can however add additional information to an event to easier analyze it later using the justtrack dashboard.
The `EventDetails` class allows you to amend an event with a *category*, an *element*, and an *action*.
On the dashboard you can then filter for all events with, e.g., a specific *category*, looking at multiple connected events at the same time.
The `CustomUserEvent` class allows you to amend your events with up to three custom dimensions as well as a value and a unit.
The custom dimensions allow you to split events on the dashboard again by some criterium.
For example, in a game a player might acquire an item.
You could be interested in the rarity of each item acquired, e.g., to see how many users acquire a rare item on their first day (as the dashboard allows you to look at different cohorts of users):

```cs
using JustTrack;

void recordItemAcquire(Item item) {
  var details = new EventDetails("progression_item_acquire", "progression", "item", "acquire");
  JustTrackSDK.PublishEvent(new CustomUserEvent(details).SetDimension1(item.Rare ? "rare" : "common"));
}
```

If you need to track how much time a user needs for a level, you can attach a value and a unit to an event.
In the following example, we measure the time it takes a user to complete the level and then attach it to the event before sending it to the backend:

```cs
using JustTrack;

void recordLevelDone(double duration) {
  var details = new EventDetails("progression_level_finish", "progression", "level", "finish");
  JustTrackSDK.PublishEvent(new CustomUserEvent(details).SetValueAndUnit(duration, Unit.Seconds));
}
```

The units supported by the justtrack SDK are **Count**, **Seconds** and **Milliseconds**.
Alternatively, an event can carry a monetary value instead of a value with a unit:

```cs
using JustTrack;

void publishEventWithMoney(EventDetails details, double value, string currency) {
  JustTrackSDK.PublishEvent(new CustomUserEvent(details).SetValueAndCurrency(duration, currency));
}
```

### Standard events

In the previous example we actually did a bad emulation of an event already defined by the justtrack SDK.
The code should actually have looked like this:

```cs
using JustTrack;

void recordLevelDone() {
  JustTrackSDK.PublishEvent(new ProgressionLevelFinishEvent(null, null));
}
```

The first two arguments in the constructor are called `elementName` and `elementId`.
Standard events accept additional dimensions next to the three custom dimensions you can use on custom user events.
For the `ProgressionLevelFinishEvent` these are `elementName` and `elementId`, which you can (but don't have to) use to record about which level you are actually talking.
Thus, a call could look like this (hardcoding the arguments for simplicity):

```cs
using JustTrack;

void recordLevelDone() {
  JustTrackSDK.PublishEvent(new ProgressionLevelFinishEvent("Tutorial", "LVL001"));
}
```

As you can see, we dropped the duration, so it seems like it is no longer available.
This is not a limitation of the standard events (for example, the `LoginProviderauthorizationFinishEvent` is a standard event which can provide a duration, too).
Instead, the justtrack SDK is already measuring the duration for us and will provide it automatically.

### Progression time tracking

The justtrack SDK automatically tracks the time a user spends in each level and quest you are tracking via `ProgressionLevelStartEvent` and `ProgressionQuestStartEvent`.
The tracking ends once you trigger a `ProgressionLevelFinishEvent` or `ProgressionLevelFailEvent` for levels and a `ProgressionQuestFinishEvent` or `ProgressionQuestFailEvent` for quests.
These events are then automatically modified to also carry the total time the user spend with the game open on his device.

Example: A user starts a level at 1pm and plays for 5 minutes.
He then is interrupted by a phone call and your game is running in the background for 10 minutes.
Afterwards he continues to play and finishes the level after another 3 minutes.
Once you trigger the corresponding finish event the SDK computes that the user took 8 minutes to finish the level and sends this value to the backend.
You can then later see on the justtrack dashboard how long users take in general to complete a single level and identify levels which are unreasonably hard or too easy compared to your expectation.

There are two important aspects to this automatic tracking.
First, each time the user finishes or fails a level you have to submit another start event for that level again to restart the tracking.
If a user fails to complete a level first, we would add the time between the start and the fail event and attach it to the fail event.
If the user now retries the level without another start event getting triggered, the next fail or finish event will not have any duration attached.
Thus, there should be one start event for every fail or finish event.
The flow could look something like this:

![Progression Events](https://sdk.justtrack.io/docs/justtrack-sdk/unity/progressionEvents-42c489b0-82f1-4ca7-aa54-b8a293ffffc9.png)

As you can see, each event is carrying some string in the above example.
They represent the element name dimension of the events.
If two progression events carry different element names or IDs, we will treat them like separate levels and not match them.
Thus, if you send a finish event for level 2 two seconds after starting level 1 we will not add a duration of two seconds to that event, but instead look for some other start event in the past for level 2.
Similarly, quests and levels are of course different namespaces and will not be mixed, either.

## IronSource integration

If you are using IronSource to diplay ads to your users, you can integrate the justtrack SDK with the IronSource SDK.
In that case the justtrack SDK will initialize the IronSource SDK on your behalf and pass the justtrack user id to the IronSource SDK.
For each platform, supply the correct app key in the prefab and enable all ad units you need.

![IronSource Integration](https://sdk.justtrack.io/docs/justtrack-sdk/unity/ironSourceIntegration-ce4df4d6-3448-4507-8a01-40dee275d581.png)

```cs
using JustTrack;

JustTrackSDKBehaviour.OnIronSourceInitialized(() => {
  // callback called exactly once as soon as ironSource has been initialized.
  // the callback is also called if ironSource has already been initialized,
  // so you can safely setup banners or other ads in here.

  JustTrackSDKBehaviour.IronSourceInitialized == true; // always true at this point
});
```

You can then access `JustTrack.JustTrackSDKBehaviour.IronSourceInitialized` to check if IronSource already has been initialized or use `JustTrack.JustTrackSDKBehaviour.OnIronSourceInitialized` to schedule a callback once it has been initialized (the callback is also invoked should IronSource already have been initialized, you do not need check for that yourself prior to this.
The callback is always asynchrounously called on the main thread).

## Forwarding ad impressions

The justtrack SDK can forward data about ad impressions to the justtrack backend to track the ad revenue of your app.
If you are using the IronSource integration, this will already happen automatically for IronSource ad impressions.
For other ad network integrations (AdMob for example), you have to provide the ad impression events yourself by calling `ForwardAdImpression`:

```cs
JustTrackSDK.ForwardAdImpression(adFormat, adSdkName, adNetwork, placement, abTesting, segmentName, instanceName, revenue);
```

You need to provide the following parameters to such a call:

- adFormat: The format of the ad.
- adSdkName: The name of the SDK which provided the event, for example "ironsource" or "admob".
- adNetwork: The name of the ad network which provided the ad. May be null.
- placement: The placement of the ad. May be null.
- abTesting: The test group of the user if we are A/B testing. May be null.
- segmentName: The name of the segment of the ad. May be null.
- instanceName: The name of the instance of the ad. May be null.
- revenue: The amount of revenue this ad generated together with a currency. May be zero but must not be negative.

Capitalization of the parameter values is important (i.e., "Ironsource" would not work as ad SDK name).
Upon success, `ForwardAdImpression` returns true, if a parameter was invalid, false is returned.

## Forwarding in-app purchases

If the user performs an in-app product or subscription purchase, you can forward the purchase via the justtrack SDK to our backend.
By default, this is already enabled and every purchase of the user will be forwarded automatically.
You can enable or disable the integration by calling `SetAutomaticInAppPurchaseTracking` or by configuring it in the justtrack settings.

```cs
JustTrackSDK.SetAutomaticInAppPurchaseTracking(forwardPurchasesAutomatically);
```

When enabled, all purchases will be reported automatically to the justtrack backend and, if you have correctly configured purchase validation, show up as revenue for your app.

You have to [configure a service account](https://docs.justtrack.io/product-features/cost-and-revenue-aggregation/in-app-purchase-revenue/google-play-store) and [a shared secret](https://docs.justtrack.io/product-features/cost-and-revenue-aggregation/in-app-purchase-revenue/apple-app-store) to correctly validate purchases.

## Firebase integration

If you are using the Firebase SDK next to the justtrack SDK, you can use the justtrack SDK to send the Firebase app instance id of a user to the justtrack backend.
You can then later send events from the justtrack backend to Firebase to measure events happening outside of your app or game.
To send these events, the justtrack backend needs the Firebase app instance id of the user.
You can send this by calling `JustTrackSDK.SetFirebaseAppInstanceId(firebaseAppInstanceId)`, but of course this requires you to add additional boilerplate code to your app or game.

Go to the Firebase section of the justtrack SDK configuration and search for the "Enable Firebase Integration" setting.
Here you can enable whether the justtrack SDK should automatically perform the call to `SetFirebaseAppInstanceId` with the correct value for you on Android as well as iOS.

![Firebase Integration](https://sdk.justtrack.io/docs/justtrack-sdk/unity/firebaseConfiguration-6406e120-fc75-4a3c-9b69-20de3103853e.png)

## Get an affiliate link

Invite other users to also use our app (i.e., get an affiliate link for the current user):

```cs
var onSuccess = (string link) => {
  ...
};

var onFailure = (string msg) => {
  ...
};

JustTrackSDK.CreateAffiliateLink(channel, onSuccess, onFailure);
```

## SKAdNetwork (iOS)

The justtrack SDK will call `SKAdNetwork.registerAppForAdNetworkAttribution` upon startup.
This will cause the device to send a postback describing the attribution of the app after 24 to 48 hours.
The SDK also registers the justtrack backend as the receiver for the copy of the postback for the advertised app (iOS 15+).

## Ad Tracking Transparency (iOS)

Starting with iOS 14, the justtrack SDK can only access the IDFA if the user allowed the app to track them.
By default the justtrack SDK does not request this permission and can (albeit somewhat limited) still attribute the user to the correct campaign.
You can ask the user for permission to track them to improve the precision of the attribution.

The request for this permission needs to happen *before* the justtrack SDK is initialized (as we otherwise will not get an IDFA for the user).
To help you with this you can configure the justtrack SDK to request this permission on your behalf upon app start.
Go to the iOS settings of the SDK, enable "Request Tracking Permission", and set a message to be displayed to your users (so they know why you need the permission):

![Enable Tracking Permission](https://sdk.justtrack.io/docs/justtrack-sdk/unity/enableTrackingPermission-9f8df63e-8f71-429a-8a01-877f52388abe.png)

If you provide a message, it will overwrite the `NSUserTrackingUsageDescription` property in your `Info.plist` file (see also <https://developer.apple.com/documentation/bundleresources/information_property_list/nsusertrackingusagedescription>).
If you instead set "Ignore Empty Tracking Description" the justtrack SDK assumes you already provide that setting somewhere else and does no longer warn about the missing description.

![Custom Tracking Description](https://sdk.justtrack.io/docs/justtrack-sdk/unity/customTrackingDescription-f5ed7468-f494-46e0-bb6d-1441cc00f754.png)

The justtrack SDK can also forward the answer of the user for the permission request to the Facebook Audience Network (via <https://developers.facebook.com/docs/audience-network/setting-up/platform-setup/ios/advertising-tracking-enabled/>).
If you don't use the Facebook Audience Network (FAN), you can leave the setting at "No Integration".
If you are using the Unity version of the FAN, select "Unity Integration", otherwise, select "Native Integration".
In both cases a small code file will be added to your game (you need to allow injecting the code) which is used to forward the permission upon runtime.

![Configure Facebook Audience Network Integration](https://sdk.justtrack.io/docs/justtrack-sdk/unity/configureFacebookAudienceNetworkIntegration-e69d5327-6717-44ed-bcd9-82c501ac92b6.png)

If you need to integrate with additional frameworks, you can use `JustTrackSDK.OnTrackingAuthorization` to be notified about the decision of the user:

```cs
using JustTrack;

JustTrackSDK.OnTrackingAuthorization((authorized) => {
    if (authorized) {
        // access to the GAID/IDFA has been authorized
    } else {
        // access to the GAID/IDFA has been forbidden
    }
});
```
