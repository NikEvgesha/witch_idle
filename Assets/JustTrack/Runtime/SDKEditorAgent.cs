#if UNITY_EDITOR
using System;
using UnityEngine;

namespace JustTrack {
    internal class SDKEditorAgent : SDKBaseAgent {
        private bool EditorInitialized = false;

        internal SDKEditorAgent() {}

        public override void NotifyLoadStart() {
            // nop
        }

        public override void NotifyLoadDone() {
            // nop
        }

        protected override void PerformInit(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure) {
            AttributionResponse fakeResponse = AttributionResponse.CreateFakeResponse();

            EditorInitialized = true;

            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(fakeResponse);
            });
        }

        protected override void PerformGetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(null);
            });
        }

        protected override PreliminaryRetargetingParameters PerformGetPreliminaryRetargetingParameters() {
            return null;
        }

        protected override void PerformRegisterAttributionListener(Action<AttributionResponse> pListener) {
            LogDebug("Registered attribution listener");
        }

        protected override void PerformRegisterRetargetingParameterListener(Action<RetargetingParameters> pListener) {
            LogDebug("Registered retargeting parameter listener");
        }

        protected override void PerformRegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener) {
            LogDebug("Registered preliminary retargeting parameter listener");
        }

        protected override void PerformAdColonyIntegration() {
            LogDebug("Performed AdColony integration");
        }

        protected override void PerformAppLovinIntegration() {
            LogDebug("Performed AppLovin integration");
        }

        protected override void PerformChartboostIntegration() {
            LogDebug("Performed Chartboost integration");
        }

        protected override void PerformUnityAdsIntegration() {
            LogDebug("Performed UnityAds integration");
        }

        protected override void PerformIronSourceIntegration(Action pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
        }

        protected override bool PerformForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue) {
            if (pRevenue == null) {
                pRevenue = new Money(0.0, "USD");
            }
            LogDebug($"Forwarded ad impression for ad unit {pAdFormat}, ad sdk {pAdSdkName}, ad network {pAdNetwork}, placement {pPlacement}, ab testing {pAbTesting}, segment {pSegmentName}, instance {pInstanceName}, and bundle id {pBundleId} with {pRevenue.Value} {pRevenue.Currency} revenue");

            return true;
        }

        protected override void PerformSetCustomUserId(string pCustomUserId) {
            LogDebug($"Setting custom user id {pCustomUserId}");
        }

        protected override void PerformSetAutomaticInAppPurchaseTracking(bool pEnabled) {
            LogDebug($"Setting automatic in-app purchase tracking to {pEnabled}");
        }

        protected override void PerformSetFirebaseAppInstanceId(string pFirebaseAppInstanceId) {
            LogDebug($"Forwarding Firebase app instance id {pFirebaseAppInstanceId}");
        }

        protected override void PerformPublishEvent(UserEventBase pEvent) {
            LogDebug($"Publishing event {pEvent.Name} {pEvent.GetDimension(Dimension.CUSTOM_1)} {pEvent.GetDimension(Dimension.CUSTOM_2)} {pEvent.GetDimension(Dimension.CUSTOM_3)} {pEvent.Value} {pEvent.Unit}");
        }

        protected override void PerformCreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess("https://example.com/affiliate");
            });
        }

        protected override void PerformGetUserId(Action<string> pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess("00000000-0000-0000-0000-000000000000");
            });
        }

        protected override void PerformGetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(AdvertiserIdInfo.CreateFakeAdvertiserIdInfo("00000000-0000-0000-0000-000000000000", false));
            });
        }

        protected override void PerformGetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure) {
            JustTrackSDKBehaviour.CallOnMainThread(() => {
                pOnSuccess(1);
            });
        }

        public override bool IsInitialized() {
            return EditorInitialized;
        }

        public override void LogDebug(string pMessage) {
            Debug.Log(pMessage);
        }

        public override void LogInfo(string pMessage) {
            Debug.Log(pMessage);
        }

        public override void LogWarning(string pMessage) {
            Debug.LogWarning(pMessage);
        }

        public override void LogError(string pMessage) {
            Debug.LogError(pMessage);
        }

    }
}
#endif