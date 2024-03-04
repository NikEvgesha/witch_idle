using System;
using UnityEngine;

namespace JustTrack {
    [Serializable]
    public struct IronSourceSettings {
        [SerializeField]
        public string appKey;

        [SerializeField]
        public bool enableBanner;

        [SerializeField]
        public bool enableInterstitial;

        [SerializeField]
        public bool enableRewardedVideo;

        [SerializeField]
        public bool enableOfferwall;
    }

    [Serializable]
    public struct FirebaseSettings {
        [SerializeField]
        public bool enableAutomaticIntegration;
    }

    public enum FacebookAudienceNetworkIntegration {
        NoIntegration     = 0,
        UnityIntegration  = 1,
        NativeIntegration = 2,
    }

    [Serializable]
    public struct iOSTrackingSettings {
        [SerializeField]
        public bool requestTrackingPermission;

        [SerializeField]
        public bool useCustomAdvertisingAttributionReportEndpoint;

        [SerializeField]
        public string trackingPermissionDescription;

        [SerializeField]
        public bool trackingPermissionDescriptionManaged;

        [SerializeField]
        public FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration;
    }

    public class JustTrackSettings : ScriptableObject {
        public const string JustTrackSettingsDirectory = "Assets/JustTrack/Resources";
        public const string JustTrackSettingsResource = "JustTrackSettings";
        public const string JustTrackSettingsPath = JustTrackSettingsDirectory + "/" + JustTrackSettingsResource + ".asset";

        [SerializeField]
        public bool alwaysUpdateInjectedCode;

        [SerializeField]
        public bool ignoreFirebaseIntegration;

        [SerializeField]
        public string androidApiToken;

        [SerializeField]
        public string iosApiToken;

        [SerializeField]
        public AttributionProvider androidTrackingProvider;

        [SerializeField]
        public AttributionProvider iosTrackingProvider;

        [SerializeField]
        public bool androidAdColonyIntegration;

        [SerializeField]
        public bool iosAdColonyIntegration;

        [SerializeField]
        public bool androidAppLovinIntegration;

        [SerializeField]
        public bool iosAppLovinIntegration;

        [SerializeField]
        public bool androidChartboostIntegration;

        [SerializeField]
        public bool iosChartboostIntegration;

        [SerializeField]
        public bool androidUnityAdsIntegration;

        [SerializeField]
        public bool iosUnityAdsIntegration;

        [SerializeField]
        public IronSourceSettings androidIronSourceSettings;

        [SerializeField]
        public IronSourceSettings iosIronSourceSettings;

        [SerializeField]
        public iOSTrackingSettings iosTrackingSettings;

        [SerializeField]
        public FirebaseSettings androidFirebaseSettings;

        [SerializeField]
        public bool iosDisableAutomaticInAppPurchaseTracking;

        [SerializeField]
        public bool androidDisableAutomaticInAppPurchaseTracking;

        [SerializeField]
        public FirebaseSettings iosFirebaseSettings;

        [SerializeField]
        public bool enableExperimentalPodFilePostProcessor;

        [SerializeField]
        public bool enableDebugMode;

        internal static JustTrackSettings loadFromResources() {
            return Resources.Load<JustTrackSettings>(JustTrackSettings.JustTrackSettingsResource);
        }
    }
}
