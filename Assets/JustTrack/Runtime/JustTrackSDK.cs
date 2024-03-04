using System;
using System.Linq;

namespace JustTrack {
    public class JustTrackSDK {
        private static SDKAgent _AGENT = null;
        internal static SDKAgent AGENT {
            get {
                if (_AGENT == null) {
#if UNITY_EDITOR
                    _AGENT = new SDKEditorAgent();
#elif UNITY_ANDROID
                    _AGENT = new SDKAndroidAgent();
#elif UNITY_IOS
                    _AGENT = SDKiOSAgent.INSTANCE;
#endif
                }

                return _AGENT;
            }
        }

        private static event Action<AttributionResponse> s_OnAttributionResponse;

        public static event Action<AttributionResponse> OnAttributionResponse {
            add {
                if (s_OnAttributionResponse == null || !s_OnAttributionResponse.GetInvocationList().Contains(value)) {
                    s_OnAttributionResponse += value;
                }
            }

            remove {
                if (s_OnAttributionResponse.GetInvocationList().Contains(value)) {
                    s_OnAttributionResponse -= value;
                }
            }
        }

        private static event Action<RetargetingParameters> s_OnRetargetingParameters;

        public static event Action<RetargetingParameters> OnRetargetingParameters {
            add {
                if (s_OnRetargetingParameters == null || !s_OnRetargetingParameters.GetInvocationList().Contains(value)) {
                    s_OnRetargetingParameters += value;
                }
            }

            remove {
                if (s_OnRetargetingParameters.GetInvocationList().Contains(value)) {
                    s_OnRetargetingParameters -= value;
                }
            }
        }

        private static event Action<PreliminaryRetargetingParameters> s_OnPreliminaryRetargetingParameters;

        public static event Action<PreliminaryRetargetingParameters> OnPreliminaryRetargetingParameters {
            add {
                if (s_OnPreliminaryRetargetingParameters == null || !s_OnPreliminaryRetargetingParameters.GetInvocationList().Contains(value)) {
                    s_OnPreliminaryRetargetingParameters += value;
                }
            }

            remove {
                if (s_OnPreliminaryRetargetingParameters.GetInvocationList().Contains(value)) {
                    s_OnPreliminaryRetargetingParameters -= value;
                }
            }
        }

        internal static void Init(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure) {
            AGENT.Initialize(pApiKey, pTrackingId, pTrackingProvider, pCustomUserId, pIntegrateWithFirebase, pAutomaticInAppPurchaseTracking, pEnableDebugMode, pOnSuccess, pOnFailure);
            AGENT.RegisterAttributionListener((attribution) => {
                // already on the main thread
                if (s_OnAttributionResponse != null) {
                    s_OnAttributionResponse.Invoke(attribution);
                }
            });
            AGENT.RegisterRetargetingParameterListener((parameters) => {
                // already on the main thread
                if (s_OnRetargetingParameters != null) {
                    s_OnRetargetingParameters.Invoke(parameters);
                }
            });
            AGENT.RegisterPreliminaryRetargetingListener((parameters) => {
                // already on the main thread
                if (s_OnPreliminaryRetargetingParameters != null) {
                    s_OnPreliminaryRetargetingParameters.Invoke(parameters);
                }
            });
        }

        public static void GetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure) {
            AGENT.GetRetargetingParameters(pOnSuccess, pOnFailure);
        }

        public static PreliminaryRetargetingParameters GetPreliminaryRetargetingParameters() {
            return AGENT.GetPreliminaryRetargetingParameters();
        }

        // Listen for AdColony impressions and automatically forward them to the justtrack backend.
        // You only have to call this if you are using AdColony and didn't enable the integration
        // on the prefab.
        public static void IntegrateWithAdColony() {
            AGENT.IntegrateWithAdColony();
        }

        // Listen for AppLovin impressions and automatically forward them to the justtrack backend.
        // You only have to call this if you are using AppLovin and didn't enable the integration
        // on the prefab.
        public static void IntegrateWithAppLovin() {
            AGENT.IntegrateWithAppLovin();
        }

        // Listen for Chartboost impressions and automatically forward them to the justtrack backend.
        // You only have to call this if you are using Chartboost and didn't enable the integration
        // on the prefab.
        public static void IntegrateWithChartboost() {
            AGENT.IntegrateWithChartboost();
        }

        // Listen for UnityAds impressions and automatically forward them to the justtrack backend.
        // You only have to call this if you are using UnityAds and didn't enable the integration
        // on the prefab.
        public static void IntegrateWithUnityAds() {
            AGENT.IntegrateWithUnityAds();
        }

        // Configure the IronSource SDK to use the user id from justtrack and forward ad impressions as user events.
        // You only have to call this if you are not using the prefab (and are using IronSource). If you call this yourself, you have
        // to wait for any callback to get called before you initialize IronSource itself.
        public static void IntegrateWithIronSource(Action pOnSuccess, Action<string> pOnFailure) {
            AGENT.IntegrateWithIronSource(pOnSuccess, pOnFailure);
        }

        // Forward an ad impression to the justtrack backend. Depending on the ad SDK we will use this
        // data to display the correct amount of ad revenue your app generated.
        public static bool ForwardAdImpression(AdFormat pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue) {
            string adFormat = AdFormatInternalConversation.ToInternalString(pAdFormat);

            return AGENT.ForwardAdImpression(adFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, pRevenue);
        }

        // Forward an ad impression to the justtrack backend. This version of the method allows you to specify
        // any ad format you want.
        public static bool ForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue) {
            return AGENT.ForwardAdImpression(pAdFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, pRevenue);
        }

        // Forward a custom user id to the server.
        public static void SetCustomUserId(string pCustomUserId) {
            AGENT.SetCustomUserId(pCustomUserId);
        }

        // Enable and disable automatic in-app purchase tracking.
        public static void SetAutomaticInAppPurchaseTracking(bool pEnabled) {
            AGENT.SetAutomaticInAppPurchaseTracking(pEnabled);
        }

        // Forward the firebase app instance id (see https://firebase.google.com/docs/reference/android/com/google/firebase/analytics/FirebaseAnalytics#public-taskstring-getappinstanceid
        // to how to obtain one) to the justtrack backend.
        public static void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId) {
            AGENT.SetFirebaseAppInstanceId(pFirebaseAppInstanceId);
        }

        // Publish a new user event to the server.
        public static void PublishEvent(string pEvent) {
            if (pEvent == null) {
                return;
            }
            AGENT.PublishEvent(new UserEventBase(new EventDetails(pEvent)));
        }

        // Publish a new user event to the server.
        public static void PublishEvent(EventDetails pEvent) {
            if (pEvent == null) {
                return;
            }
            AGENT.PublishEvent(new UserEventBase(pEvent));
        }

        // Publish a new user event to the server.
        public static void PublishEvent(UserEvent pEvent) {
            if (pEvent == null) {
                return;
            }
            AGENT.PublishEvent(pEvent.GetBase());
        }

        // Create a link which can be used to invite another user to your app.
        public static void CreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure) {
            AGENT.CreateAffiliateLink(pChannel, pOnSuccess, pOnFailure);
        }

        // Retrieve the justtrack user id.
        public static void GetUserId(Action<string> pOnSuccess, Action<string> pOnFailure) {
            AGENT.GetUserId(pOnSuccess, pOnFailure);
        }

        public static void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure) {
            AGENT.GetAdvertiserIdInfo(pOnSuccess, pOnFailure);
        }

        public static void GetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure) {
            AGENT.GetTestGroupId(pOnSuccess, pOnFailure);
        }

#if UNITY_IOS
            // Request tracking authorization. If not supported on this device, checks if ad tracking is limited.
            // If authorization was already permitted or denied, the callback is invoked immediately.
            // Otherwise it prompts the user for authorization and returns whether the user allowed tracking.
            public static void RequestTrackingAuthorization(Action<bool> pOnAuthorized) {
#if UNITY_EDITOR
                    // assume we always get the autorization when running inside the editor
                    pOnAuthorized(true);
#else
                    JustTrackSDKNativeBridgeUnity.Instance.RequestTrackingAuthorization(pOnAuthorized);
#endif
            }
#endif

        // Determine if we are allowed to access the IDFA/GAID. On iOS, this calls RequestTrackingAuthorization
        // if the justtrack SDK was configured to request the tracking permission on behalf of the developer.
        // If this is not the case (or we are on Android), this method determines if we are allowed to access
        // the advertiser id and returns this information in the callback.
        public static void OnTrackingAuthorization(Action<bool> pOnAuthorized) {
#if UNITY_IOS
                var settings = JustTrackSettings.loadFromResources();
                if (settings.iosTrackingSettings.requestTrackingPermission) {
                    // if we should request it, we can just make use of the logic there to handle the case where we
                    // already have the permission
                    RequestTrackingAuthorization(pOnAuthorized);
                    return;
                }
#endif
            GetAdvertiserIdInfo((info) => {
                pOnAuthorized(!info.IsLimitedAdTracking);
            }, (error) => {
                // if we can't read the advertiser id, it is not available
                pOnAuthorized(false);
            });
        }

        //Retrieve the current SDK version
        public static string getVersion() {
            return BuildConfig.SDK_VERSION;
        }

        private JustTrackSDK() { }
    }
}