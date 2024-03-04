using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class IronSourceAdQuality : CodeGeneratedSingleton
{
    private static GameObject adQualityGameObject = new GameObject("IronSourceAdQuality");

#if UNITY_IOS && !UNITY_EDITOR

    [DllImport ("__Internal")]
    private static extern int ironSourceAdQuality_initialize(string appKey, string userId, bool userIdSet, bool testMode, 
                                                        bool debug, int logLevel, bool isInitCallbackSet);
    [DllImport ("__Internal")]
    private static extern int ironSourceAdQuality_changeUserId(string userId);
    [DllImport ("__Internal")]
    private static extern int ironSourceAdQuality_addExtraUserId(string userId);
    [DllImport ("__Internal")]
    private static extern int ironSourceAdQuality_setUserConsent(bool userConsent);

#endif

    protected override bool DontDestroySingleton { get { return true; }	}

    protected override void InitAfterRegisteringAsSingleInstance() {
        base.InitAfterRegisteringAsSingleInstance();
    }

    public static void Initialize(string appKey) {
        Initialize(appKey, new ISAdQualityConfig());
    }
    
    public static void Initialize(string appKey, ISAdQualityConfig adQualityConfig) {
        Initialize(appKey, 
            adQualityConfig.UserId,
            adQualityConfig.UserIdSet,
            adQualityConfig.TestMode, 
            adQualityConfig.LogLevel,
            adQualityConfig.AdQualityInitCallback);
    }

    private static void Initialize(string appKey, string userId, bool userIdSet, bool testMode, 
                                    ISAdQualityLogLevel logLevel, ISAdQualityInitCallback adQualityInitCallback) {

        GetSynchronousCodeGeneratedInstance<IronSourceAdQuality>();
        ISAdQualityInitCallbackWrapper initCallbackWrapper = adQualityGameObject.AddComponent<ISAdQualityInitCallbackWrapper>();
        initCallbackWrapper.AdQualityInitCallback = adQualityInitCallback;
        bool isInitCallbackSet = (adQualityInitCallback != null);

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJNI.PushLocalFrame(100);

        AndroidJNI.PopLocalFrame(IntPtr.Zero);
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJNI.PushLocalFrame(100);
        using(AndroidJavaClass jniAdQualityClass = new AndroidJavaClass("com.ironsource.adqualitysdk.sdk.unity.IronSourceAdQuality")) {
            AndroidJavaClass jLogLevelEnum = new AndroidJavaClass("com.ironsource.adqualitysdk.sdk.ISAdQualityLogLevel");
            AndroidJavaObject jLogLevel = jLogLevelEnum.CallStatic<AndroidJavaObject>("fromInt", (int)logLevel);
            jniAdQualityClass.CallStatic("initialize", appKey, userId, userIdSet, testMode, jLogLevel, isInitCallbackSet);
        }
        AndroidJNI.PopLocalFrame(IntPtr.Zero);
#elif UNITY_IOS && !UNITY_EDITOR
        ironSourceAdQuality_initialize(appKey, userId, userIdSet ,testMode, DEBUG, (int)logLevel, isInitCallbackSet);
#else
        ISAdQualityUtils.LogWarning(TAG, "Ad Quality SDK works only on Android or iOS devices.");
#endif
    }

    public static void ChangeUserId(String userId) {
        #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJNI.PushLocalFrame(100);
            using(AndroidJavaClass jniAdQualityClass = new AndroidJavaClass("com.ironsource.adqualitysdk.sdk.unity.IronSourceAdQuality")) {
                jniAdQualityClass.CallStatic("changeUserId", userId);
            }
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        #elif UNITY_IOS && !UNITY_EDITOR
            ironSourceAdQuality_changeUserId(userId);
        #endif
    }

    [Obsolete("AddExtraUserId method will be remove in next major version")]
    public static void AddExtraUserId(String userId) {
        #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJNI.PushLocalFrame(100);
            using(AndroidJavaClass jniAdQualityClass = new AndroidJavaClass("com.ironsource.adqualitysdk.sdk.unity.IronSourceAdQuality")) {
                jniAdQualityClass.CallStatic("addExtraUserId", userId);
            }
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        #elif UNITY_IOS && !UNITY_EDITOR
            ironSourceAdQuality_addExtraUserId(userId);
        #endif
    }

    public static void SetUserConsent(bool userConsent) {
        #if UNITY_ANDROID && !UNITY_EDITOR
            AndroidJNI.PushLocalFrame(100);
            using(AndroidJavaClass jniAdQualityClass = new AndroidJavaClass("com.ironsource.adqualitysdk.sdk.unity.IronSourceAdQuality")) {
                jniAdQualityClass.CallStatic("setUserConsent", userConsent);
            }
            AndroidJNI.PopLocalFrame(IntPtr.Zero);
        #elif UNITY_IOS && !UNITY_EDITOR
            ironSourceAdQuality_setUserConsent(userConsent);
        #endif
    }

    private const string TAG = "IronSource AdQuality";
    private const bool DEBUG = false;

}
