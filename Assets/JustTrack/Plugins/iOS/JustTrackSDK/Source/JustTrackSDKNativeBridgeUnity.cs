#if UNITY_IOS
using UnityEngine;
using System;
using System.Text;
using System.Runtime.InteropServices;
using JustTrack;

internal class JustTrackSDKNativeBridgeUnity : MonoBehaviour {
    #region Declare external C interface

    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_free_string(IntPtr s);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_notify_load_start();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_notify_load_done();
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_init(string apiToken, string trackingId, string trackingProvider, string customUserId, int inactivityTimeFrameHours, int reAttributionTimeFrameDays, int reFetchAttributionDelaySeconds, int attributionRetryDelaySeconds, bool automaticInAppPurchaseTracking);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_get_retargeting_parameters();
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_get_preliminary_retargeting_parameters();
    [DllImport("__Internal")]
    private static extern IntPtr _justtrack_sdk_rp_get_advertiser_id_info();
    [DllImport("__Internal")]
    private static extern int _justtrack_sdk_rp_get_test_group_id();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_custom_user_id(string customUserId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(bool automaticInAppPurchaseTracking);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_set_firebase_app_instance_id(string firebaseAppInstanceId);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_create_affiliate_link(string channel);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_publish_event(string name, string category, string element, string action, string dimensions);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_publish_unit_event(string name, string category, string element, string action, string dimensions, double value, string unit);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_publish_money_event(string name, string category, string element, string action, string dimensions, double value, string currency);
    [DllImport("__Internal")]
    private static extern bool _justtrack_sdk_rp_forward_ad_impression(string adFormat, string adSdkName, string adNetwork, string placement, string abTesting, string segmentName, string instanceName, string bundleId, double revenue, string currency);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_ad_colony();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_app_lovin();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_chartboost();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_unity_ads();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_integrate_with_firebase();
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_log_debug(string message);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_log_info(string message);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_log_warning(string message);
    [DllImport("__Internal")]
    private static extern void _justtrack_sdk_rp_log_error(string message);
    [DllImport("__Internal")]
    private static extern int _justtrack_sdk_rp_request_tracking_authorization();

    private static string ReadStringFromPointer(IntPtr ptr) {
        if (ptr == IntPtr.Zero) {
            return "";
        }

        // Custom implementation for Marshal.PtrToStringUTF8 which is not available
        // on the .NET version we are using:

        int length = 0;
        while (Marshal.ReadByte(ptr, length) != 0) {
            length++;
        }

        byte[] bytes = new byte[length];
        Marshal.Copy(ptr, bytes, 0, length);
        string str = Encoding.UTF8.GetString(bytes);

        // don't forget to free the memory (otherwise we wouldn't need to do this dance)
        _justtrack_sdk_rp_free_string(ptr);

        return str;
    }

    #endregion

    #region Wrapped methods and properties
    private bool initialized = false;

    internal static void NotifyLoadStart() {
        _justtrack_sdk_rp_notify_load_start();
    }

    internal static void NotifyLoadDone() {
        _justtrack_sdk_rp_notify_load_done();
    }

    internal string Init(string apiToken, string trackingId, string trackingProvider, string customUserId, bool automaticInAppPurchaseTracking, Action<string> onSuccess, Action<string> onError) {
        onAttributionDone += onSuccess;
        onAttributionError += onError;
        var userIdPtr = _justtrack_sdk_rp_init(
            apiToken,
            trackingId != null ? trackingId : "",
            trackingProvider != null ? trackingProvider : "",
            customUserId != null ? customUserId : "",
            48,
            14,
            5,
            120,
            automaticInAppPurchaseTracking
        );
        initialized = true;

        return ReadStringFromPointer(userIdPtr);
    }

    internal void GetRetargetingParameters(Action<string> onSuccess, Action<string> onError) {
        onGetRetargetingParametersDone += onSuccess;
        onGetRetargetingParametersError += onError;
        _justtrack_sdk_rp_get_retargeting_parameters();
    }

    internal string GetPreliminaryRetargetingParameters() {
        return ReadStringFromPointer(_justtrack_sdk_rp_get_preliminary_retargeting_parameters());
    }

    internal bool ForwardAdImpression(string adFormat, string adSdkName, string adNetwork, string placement, string abTesting, string segmentName, string instanceNamem, string bundleId, double revenue, string currency) {
        return _justtrack_sdk_rp_forward_ad_impression(adFormat, adSdkName, adNetwork, placement, abTesting, segmentName, instanceNamem, bundleId, revenue, currency);
    }

    internal void SetCustomUserId(string customUserId) {
        _justtrack_sdk_rp_set_custom_user_id(customUserId != null ? customUserId : "");
    }

    internal void SetAutomaticInAppPurchaseTracking(bool automaticInAppPurchaseTracking) {
        _justtrack_sdk_rp_set_automatic_in_app_purchase_tracking(automaticInAppPurchaseTracking);
    }

    internal void PublishEvent(EventDetails name, UserEventDimensions dimensions) {
        _justtrack_sdk_rp_publish_event(
            name.Name,
            name.Category != null ? name.Category : "",
            name.Element != null ? name.Element : "",
            name.Action != null ? name.Action : "",
            dimensions.Encode()
        );
    }

    internal void PublishUnitEvent(EventDetails name, UserEventDimensions dimensions, double value, string unit) {
        _justtrack_sdk_rp_publish_unit_event(
            name.Name,
            name.Category != null ? name.Category : "",
            name.Element != null ? name.Element : "",
            name.Action != null ? name.Action : "",
            dimensions.Encode(),
            value,
            unit
        );
    }

    internal void CreateAffiliateLink(string channel, Action<string> onSuccess, Action<string> onError) {
        onCreateAffiliateLinkDone += onSuccess;
        onCreateAffiliateLinkError += onError;
        _justtrack_sdk_rp_create_affiliate_link(channel);
    }

    internal void SetFirebaseAppInstanceId(string firebaseAppInstanceId) {
        _justtrack_sdk_rp_set_firebase_app_instance_id(firebaseAppInstanceId);
    }

    internal void IntegrateWithAdColony() {
        _justtrack_sdk_rp_integrate_with_ad_colony();
    }

    internal void IntegrateWithAppLovin() {
        _justtrack_sdk_rp_integrate_with_app_lovin();
    }

    internal void IntegrateWithChartboost() {
        _justtrack_sdk_rp_integrate_with_chartboost();
    }

    internal void IntegrateWithUnityAds() {
        _justtrack_sdk_rp_integrate_with_unity_ads();
    }

    internal void IntegrateWithFirebase() {
        _justtrack_sdk_rp_integrate_with_firebase();
    }

    internal void LogDebug(string message) {
        _justtrack_sdk_rp_log_debug(message);
    }

    internal void LogInfo(string message) {
        _justtrack_sdk_rp_log_info(message);
    }

    internal void LogWarning(string message) {
        _justtrack_sdk_rp_log_warning(message);
    }

    internal void LogError(string message) {
        _justtrack_sdk_rp_log_error(message);
    }

    internal string GetAdvertiserIdInfo() {
        return ReadStringFromPointer(_justtrack_sdk_rp_get_advertiser_id_info());
    }

    internal int? GetTestGroupId() {
        int testGroupId = _justtrack_sdk_rp_get_test_group_id();
        if (testGroupId == -1) {
            return null;
        }

        return testGroupId;
    }

    internal void RequestTrackingAuthorization(Action<bool> onAuthorized) {
        onAuthorizationDone += onAuthorized;
        _justtrack_sdk_rp_request_tracking_authorization();
    }

    internal bool IsInitialized() {
        return initialized;
    }

    #endregion

    #region Singleton implementation

    private static JustTrackSDKNativeBridgeUnity _instance;

    internal static JustTrackSDKNativeBridgeUnity Instance {
        get {
            if (_instance == null) {
                var obj = new GameObject("JustTrackSDKNativeBridgeUnity");
                _instance = obj.AddComponent<JustTrackSDKNativeBridgeUnity>();
            }
            return _instance;
        }
    }

    void Awake() {
        if (_instance != null) {
            // we are a duplicate and thus there is no need for this instance.
            // destroy this instance and keep the already existing instance.
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Delegates

    private event Action<string> onAttributionDone;
    private event Action<string> onAttributionError;
    private event Action<string> onGetRetargetingParametersDone;
    private event Action<string> onGetRetargetingParametersError;
    private event Action<string> onCreateAffiliateLinkDone;
    private event Action<string> onCreateAffiliateLinkError;
    private event Action<bool> onAuthorizationDone;

    internal void OnAttributionDone(string response) {
        if (onAttributionDone != null) {
            onAttributionDone.Invoke(response);
        }
        onAttributionDone = null;
        onAttributionError = null;
    }

    internal void OnAttributionError(string error) {
        if (onAttributionError != null) {
            onAttributionError.Invoke(error);
        }
        onAttributionDone = null;
        onAttributionError = null;
    }

    internal void OnGetRetargetingParametersDone(string response) {
        if (onGetRetargetingParametersDone != null) {
            onGetRetargetingParametersDone.Invoke(response);
        }
        onGetRetargetingParametersDone = null;
        onGetRetargetingParametersError = null;
    }

    internal void OnGetRetargetingParametersError(string error) {
        if (onGetRetargetingParametersError != null) {
            onGetRetargetingParametersError.Invoke(error);
        }
        onGetRetargetingParametersDone = null;
        onGetRetargetingParametersError = null;
    }

    internal void OnAttributionListenerReceived(string response) {
        SDKiOSAgent.INSTANCE.OnAttributionListenerReceived(response);
    }

    internal void OnRetargetingParametersListenerReceived(string response) {
        SDKiOSAgent.INSTANCE.OnRetargetingParametersListenerReceived(response);
    }

    internal void OnPreliminaryRetargetingParametersListenerReceived(string response) {
        SDKiOSAgent.INSTANCE.OnPreliminaryRetargetingParametersListenerReceived(response);
    }

    internal void OnValidatePreliminaryRetargetingParametersDone(string response) {
        SDKiOSAgent.INSTANCE.OnValidatePreliminaryRetargetingParametersDone(response);
    }

    internal void OnValidatePreliminaryRetargetingParametersError(string response) {
        SDKiOSAgent.INSTANCE.OnValidatePreliminaryRetargetingParametersError(response);
    }

    internal void OnCreateAffiliateLinkDone(string link) {
        if (onCreateAffiliateLinkDone != null) {
            onCreateAffiliateLinkDone.Invoke(link);
        }
        onCreateAffiliateLinkDone = null;
        onCreateAffiliateLinkError = null;
    }

    internal void OnCreateAffiliateLinkError(string error) {
        if (onCreateAffiliateLinkError != null) {
            onCreateAffiliateLinkError.Invoke(error);
        }
        onCreateAffiliateLinkDone = null;
        onCreateAffiliateLinkError = null;
    }

    internal void OnHandleError(string error) {
        JustTrackSDK.AGENT.LogError("JustTrackSDK caught error: " + error);
    }

    internal void OnTrackingAuthorization(string reply) {
        if (onAuthorizationDone != null) {
            onAuthorizationDone.Invoke(reply == "authorized");
        }
        onAuthorizationDone = null;
    }

    #endregion
}
#endif
