using System;

namespace JustTrack {
    internal interface SDKAgent {
        void NotifyLoadStart();
        void NotifyLoadDone();
        void Initialize(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure);
        void GetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure);
        PreliminaryRetargetingParameters GetPreliminaryRetargetingParameters();
        void RegisterAttributionListener(Action<AttributionResponse> pListener);
        void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener);
        void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener);
        void IntegrateWithAdColony();
        void IntegrateWithAppLovin();
        void IntegrateWithChartboost();
        void IntegrateWithUnityAds();
        void IntegrateWithIronSource(Action pOnSuccess, Action<string> pOnFailure);
        bool ForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue);
        void SetCustomUserId(string pCustomUserId);
        void SetAutomaticInAppPurchaseTracking(bool pEnabled);
        void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId);
        void PublishEvent(UserEventBase pEvent);
        void CreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure);
        void GetUserId(Action<string> pOnSuccess, Action<string> pOnFailure);
        void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure);
        void GetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure);
        bool IsInitialized();
        void LogDebug(string pMessage);
        void LogInfo(string pMessage);
        void LogWarning(string pMessage);
        void LogError(string pMessage);
    }
}
