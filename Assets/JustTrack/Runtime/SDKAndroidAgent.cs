#if UNITY_ANDROID
using System;
using System.Threading;
using UnityEngine;

namespace JustTrack {
    internal sealed class SDKAndroidAgent : SDKBaseAgent, IDisposable {
        internal const string PACKAGE = "io.justtrack";
        internal const string STATIC_CLASS = "JustTrack";
        internal const string BUILDER_CLASS = "JustTrackSdkBuilder";
        internal const string INTEGRATION_MANAGER_CLASS = "IntegrationManager";
        internal const string EVENT_CLASS = "UserEvent";
        internal const string MONEY_CLASS = "Money";

        private AndroidJavaObject INSTANCE;

        internal SDKAndroidAgent() {}

        public void Dispose() {
            if (INSTANCE != null) {
                INSTANCE.Dispose();
                INSTANCE = null;
            }
        }

        public override void NotifyLoadStart() {
            new Thread(() => {
                Thread.CurrentThread.IsBackground = true;
                RunAttached(() => {
                    using (var staticClass = new AndroidJavaClass($"{PACKAGE}.{STATIC_CLASS}")) {
                        staticClass.CallStatic("notifyLoadStart");
                    }
                });
            }).Start();
        }

        public override void NotifyLoadDone() {
            using (var staticClass = new AndroidJavaClass($"{PACKAGE}.{STATIC_CLASS}")) {
                staticClass.CallStatic("notifyLoadDone");
            }
        }

        protected override void PerformInit(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure) {
            if (pEnableDebugMode) {
                using var integrationManager = new AndroidJavaClass($"{PACKAGE}.{INTEGRATION_MANAGER_CLASS}");
                integrationManager.CallStatic("enableUnityJavaProxyDebugging");
            }
            using var builder = new AndroidJavaObject($"{PACKAGE}.{BUILDER_CLASS}", CurrentActivity(), pApiKey);
            if (!String.IsNullOrEmpty(pTrackingId)) {
                using var ____ = builder.Call<AndroidJavaObject>("setTrackingId", pTrackingId, pTrackingProvider);
            }
            if (!String.IsNullOrEmpty(pCustomUserId)) {
                using var ____ = builder.Call<AndroidJavaObject>("setCustomUserId", pCustomUserId);
            }
            using var _ = builder.Call<AndroidJavaObject>("setEnableFirebaseIntegration", pIntegrateWithFirebase);
            using var __ = builder.Call<AndroidJavaObject>("setAutomaticInAppPurchaseTracking", pAutomaticInAppPurchaseTracking);
            using var ___ = builder.Call<AndroidJavaObject>("runCallbacksOnMainThread");
            INSTANCE = builder.Call<AndroidJavaObject>("build");
            Action<AndroidJavaObject> onResolve = (attribution) => {
                var response = AttributionResponse.FromAndroidObject(attribution);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pOnSuccess(response);
                });
            };
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getAttribution");
            INSTANCE.Call("toPromise", responseFuture, new Promise(onResolve, pOnFailure));
        }

        protected override void PerformGetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure) {
            Action<AndroidJavaObject> onResolve = (parameters) => {
                var response = parameters == null ? null : RetargetingParameters.FromAndroidObject(parameters);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pOnSuccess(response);
                });
            };
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("getRetargetingParameters");
            INSTANCE.Call("toPromise", responseFuture, new Promise(onResolve, pOnFailure));
        }

        protected override PreliminaryRetargetingParameters PerformGetPreliminaryRetargetingParameters() {
            using var parameters = INSTANCE.Call<AndroidJavaObject>("getPreliminaryRetargetingParameters");

            if (parameters == null) {
                return null;
            }

            return PreliminaryRetargetingParameters.FromAndroidObject(parameters, INSTANCE);
        }

        protected override void PerformRegisterAttributionListener(Action<AttributionResponse> pListener) {
            using var _ = INSTANCE.Call<AndroidJavaObject>("registerAttributionListener", new AttributionListener((attribution) => {
                var response = AttributionResponse.FromAndroidObject(attribution);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pListener(response);
                });
            }));
        }

        protected override void PerformRegisterRetargetingParameterListener(Action<RetargetingParameters> pListener) {
            using var _ = INSTANCE.Call<AndroidJavaObject>("registerRetargetingParametersListener", new RetargetingParametersListener((parameters) => {
                var response = RetargetingParameters.FromAndroidObject(parameters);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pListener(response);
                });
            }));
        }

        protected override void PerformRegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener) {
            using var _ = INSTANCE.Call<AndroidJavaObject>("registerPreliminaryRetargetingParametersListener", new PreliminaryRetargetingParametersListener((parameters) => {
                var response = PreliminaryRetargetingParameters.FromAndroidObject(parameters, INSTANCE);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pListener(response);
                });
            }));
        }

        protected override void PerformAdColonyIntegration() {
            INSTANCE.Call("integrateWithAdColony");
        }

        protected override void PerformAppLovinIntegration() {
            INSTANCE.Call("integrateWithAppLovin");
        }

        protected override void PerformChartboostIntegration() {
            INSTANCE.Call("integrateWithChartboost");
        }

        protected override void PerformUnityAdsIntegration() {
            INSTANCE.Call("integrateWithUnityAds");
        }

        protected override bool PerformForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue) {
            if (pRevenue != null) {
                using var money = new AndroidJavaObject($"{PACKAGE}.{MONEY_CLASS}", pRevenue.Value, pRevenue.Currency);

                return INSTANCE.Call<bool>("forwardAdImpression", pAdFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, money);
            }

            return INSTANCE.Call<bool>("forwardAdImpression", pAdFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, null);
        }

        protected override void PerformSetCustomUserId(string pCustomUserId) {
            using var _ = INSTANCE.Call<AndroidJavaObject>("setCustomUserId", pCustomUserId);
        }

        protected override void PerformSetAutomaticInAppPurchaseTracking(bool pEnabled) {
            INSTANCE.Call("setAutomaticInAppPurchaseTracking", pEnabled);
        }

        protected override void PerformSetFirebaseAppInstanceId(string pFirebaseAppInstanceId) {
            using var _ = INSTANCE.Call<AndroidJavaObject>("setFirebaseAppInstanceId", pFirebaseAppInstanceId);
        }

        protected override void PerformPublishEvent(UserEventBase pEvent) {
            using var eventObject = AndroidEventBuilder.buildEvent(pEvent);
            INSTANCE.Call<AndroidJavaObject>("publishEvent", eventObject);
        }

        protected override void PerformCreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure) {
            using var responseFuture = INSTANCE.Call<AndroidJavaObject>("createAffiliateLink", pChannel);
            INSTANCE.Call("toPromise", responseFuture, new StringPromise(pOnSuccess, pOnFailure));
        }

        protected override void PerformGetUserId(Action<string> pOnSuccess, Action<string> pOnFailure) {
            Action<string> onResolve = (userId) => {
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pOnSuccess(userId);
                });
            };
            using var userIdFuture = INSTANCE.Call<AndroidJavaObject>("getUserId");
            INSTANCE.Call("toPromise", userIdFuture, new StringPromise(onResolve, pOnFailure));
        }

        protected override void PerformGetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure) {
            Action<AndroidJavaObject> onResolve = (info) => {
                var response = AdvertiserIdInfo.FromAndroidObject(info);
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pOnSuccess(response);
                });
            };
            using var infoFuture = INSTANCE.Call<AndroidJavaObject>("getAdvertiserIdInfo");
            INSTANCE.Call("toPromise", infoFuture, new Promise(onResolve, pOnFailure));
        }

        protected override void PerformGetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure) {
            Action<int?> onResolve = (testGroupId) => {
                JustTrackSDKBehaviour.CallOnMainThread(() => {
                    pOnSuccess(testGroupId);
                });
            };
            using var testGroupIdFuture = INSTANCE.Call<AndroidJavaObject>("getTestGroupId");
            INSTANCE.Call("toPromise", testGroupIdFuture, new IntPromise(onResolve, pOnFailure));
        }

        private AndroidJavaObject CurrentActivity() {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                return activity.GetStatic<AndroidJavaObject>("currentActivity");
            }
        }

        public override void LogDebug(string pMessage) {
            if (INSTANCE != null) {
                INSTANCE.Call("logDebug", pMessage);
            } else {
                Debug.Log(pMessage);
            }
        }

        public override void LogInfo(string pMessage) {
            if (INSTANCE != null) {
                INSTANCE.Call("logInfo", pMessage);
            } else {
                Debug.Log(pMessage);
            }
        }

        public override void LogWarning(string pMessage) {
            if (INSTANCE != null) {
                INSTANCE.Call("logWarning", pMessage);
            } else {
                Debug.LogWarning(pMessage);
            }
        }

        public override void LogError(string pMessage) {
            if (INSTANCE != null) {
                INSTANCE.Call("logError", pMessage);
            } else {
                Debug.LogError(pMessage);
            }
        }

        public override bool IsInitialized() {
            return INSTANCE != null;
        }

        private static void RunAttached(Action pAction) {
            if (JustTrackSDKBehaviour.IsOnMainThread()) {
                pAction();
            } else {
                AndroidJNI.AttachCurrentThread();
                try {
                    pAction();
                } finally {
                    AndroidJNI.DetachCurrentThread();
                }
            }
        }
    }
}
#endif