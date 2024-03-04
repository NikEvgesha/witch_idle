using System;

namespace JustTrack {
    internal abstract class SDKBaseAgent : SDKAgent {
        protected SDKBaseAgent() {}

        public abstract void NotifyLoadStart();

        public abstract void NotifyLoadDone();

        private readonly object initInProgressLock = new object();

        public void Initialize(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure) {
            if (String.IsNullOrEmpty(pApiKey)) {
                LogDebug("justtrack API Key is empty, can't initialize");

                return;
            }
            lock(initInProgressLock) {
                if (IsInitialized()) {
                    LogDebug("justtrack SDK was already initialized, skipping duplicate call");

                    return;
                }
                PerformInit(pApiKey, pTrackingId, pTrackingProvider, pCustomUserId, pAutomaticInAppPurchaseTracking, pIntegrateWithFirebase, pEnableDebugMode, (attribution) => {
                    if (pOnSuccess != null) {
                        pOnSuccess(attribution);
                    }
                }, (error) => {
                    if (pOnFailure != null) {
                        pOnFailure(error);
                    }
                });
            }
            CheckAfterInit();
        }

        protected abstract void PerformInit(string pApiKey, string pTrackingId, string pTrackingProvider, string pCustomUserId, bool pAutomaticInAppPurchaseTracking, bool pIntegrateWithFirebase, bool pEnableDebugMode, Action<AttributionResponse> pOnSuccess, Action<string> pOnFailure);

        public void GetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    GetRetargetingParameters(pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformGetRetargetingParameters(pOnSuccess, pOnFailure);
        }

        protected abstract void PerformGetRetargetingParameters(Action<RetargetingParameters> pOnSuccess, Action<string> pOnFailure);

        public PreliminaryRetargetingParameters GetPreliminaryRetargetingParameters() {
            if (!IsInitialized()) {
                return null;
            }

            return PerformGetPreliminaryRetargetingParameters();
        }

        protected abstract PreliminaryRetargetingParameters PerformGetPreliminaryRetargetingParameters();

        public void RegisterAttributionListener(Action<AttributionResponse> pListener) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    RegisterAttributionListener(pListener);
                });
                return;
            }

            PerformRegisterAttributionListener(pListener);
        }

        protected abstract void PerformRegisterAttributionListener(Action<AttributionResponse> pListener);

        public void RegisterRetargetingParameterListener(Action<RetargetingParameters> pListener) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    RegisterRetargetingParameterListener(pListener);
                });
                return;
            }

            PerformRegisterRetargetingParameterListener(pListener);
        }

        protected abstract void PerformRegisterRetargetingParameterListener(Action<RetargetingParameters> pListener);

        public void RegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    RegisterPreliminaryRetargetingListener(pListener);
                });
                return;
            }

            PerformRegisterPreliminaryRetargetingListener(pListener);
        }

        protected abstract void PerformRegisterPreliminaryRetargetingListener(Action<PreliminaryRetargetingParameters> pListener);

        public void IntegrateWithAdColony() {
            if (!IsInitialized()) {
                WaitForInititalization(IntegrateWithAdColony);
                return;
            }

            PerformAdColonyIntegration();
        }

        protected abstract void PerformAdColonyIntegration();

        public void IntegrateWithAppLovin() {
            if (!IsInitialized()) {
                WaitForInititalization(IntegrateWithAppLovin);
                return;
            }

            PerformAppLovinIntegration();
        }

        protected abstract void PerformAppLovinIntegration();

        public void IntegrateWithChartboost() {
            if (!IsInitialized()) {
                WaitForInititalization(IntegrateWithChartboost);
                return;
            }

            PerformChartboostIntegration();
        }

        protected abstract void PerformChartboostIntegration();

        public void IntegrateWithUnityAds() {
            if (!IsInitialized()) {
                WaitForInititalization(IntegrateWithUnityAds);
                return;
            }

            PerformUnityAdsIntegration();
        }

        protected abstract void PerformUnityAdsIntegration();

        public void IntegrateWithIronSource(Action pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    IntegrateWithIronSource(pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformIronSourceIntegration(pOnSuccess, pOnFailure);
        }

        protected virtual void PerformIronSourceIntegration(Action pOnSuccess, Action<string> pOnFailure) {
            #if JUSTTRACK_IRONSOURCE_INTEGRATION
                JustTrackSDKBehaviour.GetAttribution((attribution) => {
                    Reflection.IronSourceSetUserId(attribution.UserId);
                    Reflection.IronSourceAddImpressionListener((impressionData) => {
                        var adFormat = AdFormatFromIronsourceConversion.ToAdFormat(impressionData.adUnit);
                        if (adFormat != null) {
                            PerformForwardAdImpression(AdFormatInternalConversation.ToInternalString(adFormat.Value), "ironsource", impressionData.adNetwork, impressionData.placement, impressionData.abTesting, impressionData.segmentName, impressionData.instanceName, "", new Money(impressionData.revenue, "USD"));
                        }
                    });
                    // already on main thread
                    pOnSuccess();
                }, (error) => {
                    // register the impression listener in any case - if we could not fetch an attribution and later get one (because of bad network)
                    // we still want to trigger those / at least record the events.
                    Reflection.IronSourceAddImpressionListener((impressionData) => {
                        var adFormat = AdFormatFromIronsourceConversion.ToAdFormat(impressionData.adUnit);
                        if (adFormat != null) {
                            PerformForwardAdImpression(AdFormatInternalConversation.ToInternalString(adFormat.Value), "ironsource", impressionData.adNetwork, impressionData.placement, impressionData.abTesting, impressionData.segmentName, impressionData.instanceName, "", new Money(impressionData.revenue, "USD"));
                        }
                    });
                    pOnFailure(error);
                });
            #else
                LogError("No support for IronSource available");
                JustTrackSDKBehaviour.CallOnMainThread(pOnSuccess);
            #endif
        }

        public bool ForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue) {
            if (!IsInitialized()) {
                if (pRevenue != null && (pRevenue.Value < 0 || pRevenue.Currency == null || pRevenue.Currency.Length != 3)) {
                    return false;
                }

                WaitForInititalization(() => {
                    ForwardAdImpression(pAdFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, pRevenue);
                });

                return true;
            }

            return PerformForwardAdImpression(pAdFormat, pAdSdkName, pAdNetwork, pPlacement, pAbTesting, pSegmentName, pInstanceName, pBundleId, pRevenue);
        }

        protected abstract bool PerformForwardAdImpression(string pAdFormat, string pAdSdkName, string pAdNetwork, string pPlacement, string pAbTesting, string pSegmentName, string pInstanceName, string pBundleId, Money pRevenue);

        public void SetCustomUserId(string pCustomUserId) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    SetCustomUserId(pCustomUserId);
                });
                return;
            }

            PerformSetCustomUserId(pCustomUserId);
        }

        protected abstract void PerformSetCustomUserId(string pCustomUserId);

        public void SetAutomaticInAppPurchaseTracking(bool pEnabled) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    SetAutomaticInAppPurchaseTracking(pEnabled);
                });

                return;
            }

            PerformSetAutomaticInAppPurchaseTracking(pEnabled);
        }

        protected abstract void PerformSetAutomaticInAppPurchaseTracking(bool pEnabled);

        public void SetFirebaseAppInstanceId(string pFirebaseAppInstanceId) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    SetFirebaseAppInstanceId(pFirebaseAppInstanceId);
                });
                return;
            }

            PerformSetFirebaseAppInstanceId(pFirebaseAppInstanceId);
        }

        protected abstract void PerformSetFirebaseAppInstanceId(string pFirebaseAppInstanceId);

        public void PublishEvent(UserEventBase pEvent) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    PublishEvent(pEvent);
                });
                return;
            }

            PerformPublishEvent(pEvent);
        }

        protected abstract void PerformPublishEvent(UserEventBase pEvent);

        public void CreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    CreateAffiliateLink(pChannel, pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformCreateAffiliateLink(pChannel, pOnSuccess, pOnFailure);
        }

        protected abstract void PerformCreateAffiliateLink(string pChannel, Action<string> pOnSuccess, Action<string> pOnFailure);

        public void GetUserId(Action<string> pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    GetUserId(pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformGetUserId(pOnSuccess, pOnFailure);
        }

        protected abstract void PerformGetUserId(Action<string> pOnSuccess, Action<string> pOnFailure);

        public void GetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    GetAdvertiserIdInfo(pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformGetAdvertiserIdInfo(pOnSuccess, pOnFailure);
        }

        protected abstract void PerformGetAdvertiserIdInfo(Action<AdvertiserIdInfo> pOnSuccess, Action<string> pOnFailure);


        public void GetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure) {
            if (!IsInitialized()) {
                WaitForInititalization(() => {
                    GetTestGroupId(pOnSuccess, pOnFailure);
                });
                return;
            }

            PerformGetTestGroupId(pOnSuccess, pOnFailure);
        }

        protected abstract void PerformGetTestGroupId(Action<int?> pOnSuccess, Action<string> pOnFailure);

        public abstract void LogDebug(string pMessage);
        public abstract void LogInfo(string pMessage);
        public abstract void LogWarning(string pMessage);
        public abstract void LogError(string pMessage);

        private Action onInit = null;
        // Lock guarding the static fields above:
        private readonly object initLock = new object();

        public abstract bool IsInitialized();

        private void WaitForInititalization(Action pOnInit) {
            lock(initLock) {
                if (onInit == null) {
                    onInit = pOnInit;
                } else {
                    onInit += pOnInit;
                }
            }
            CheckAfterInit();
        }

        private void CheckAfterInit() {
            Action toCall = null;
            lock(initLock) {
                if (!IsInitialized()) {
                    return;
                }
                toCall = onInit;
                onInit = null;
            }
            if (toCall != null) {
                toCall();
            }
        }
    }
}
