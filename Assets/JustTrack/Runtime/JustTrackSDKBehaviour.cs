using System;
using System.Threading;
using UnityEngine;

namespace JustTrack {
    public enum AttributionProvider {
        Appsflyer = 0x0000,
        justtrack = 0xFFFF,
    }

    public class JustTrackSDKBehaviour : MonoBehaviour {
        // Invoked once the attribution has been retrieved from the backend if not null.
        private static Action<AttributionResponse> onInitialized = null;

        // Invoked if an error occurs during attribution if not null.
        private static Action<string> onError = null;

        // Stores the attribution response after the SDK has been initialized.
        private static AttributionResponse attributionResponse = null;

        // Stores the error if getting the attribution fails.
        private static string initError = null;

        // Lock guarding the attribution results and callbacks.
        private static readonly object syncLock = new object();

        static JustTrackSDKBehaviour() {
            // notify as soon as our code is loaded
            JustTrackSDK.AGENT.NotifyLoadStart();
        }

        void Awake() {
            if (unityMainThreadId != -1) {
                // we have been initialized a second time and thus can skip initialization
                return;
            }
            // set the main thread id first - we can't do that in the constructur or similar
            // (unity might load stuff on a background thread), so doing it here is the first
            // place we can do that.
            // This also serves as a check above whether we have already created an SDK instance once
            unityMainThreadId = Thread.CurrentThread.ManagedThreadId;
            // we are done loading the SDK at this point (can't really tell about the whole game here)
            JustTrackSDK.AGENT.NotifyLoadDone();

            var settings = JustTrackSettings.loadFromResources();

            if (settings == null) {
                Debug.LogError("There are no settings defined for the justtrack SDK, can not initialize");
                return;
            }

            gameObject.name = "JustTrackSDKBehaviour";
            if (gameObject.transform.parent != null) {
                Debug.Log("Detaching from parent so we can mark ourselves as DontDestroyOnLoad");
                gameObject.transform.parent = null;
            }
            DontDestroyOnLoad(gameObject);

#if UNITY_EDITOR
                init(settings);
#else
#if UNITY_IOS
                    if (settings.iosTrackingSettings.requestTrackingPermission) {
                        JustTrackSDK.RequestTrackingAuthorization((authorized) => {
                            Reflection.SetAdvertiserTrackingEnabled(authorized);
                            init(settings);
                        });
                    } else {
                        init(settings);
                    }
#else
            init(settings);
#endif
#endif
        }

        private void init(JustTrackSettings settings) {
#if UNITY_IOS
                var apiToken = settings.iosApiToken;
                var attributionProvider = settings.iosTrackingProvider;
                var integrateWithFirebase = settings.iosFirebaseSettings.enableAutomaticIntegration;
                var automaticInAppPurchaseTracking = !settings.iosDisableAutomaticInAppPurchaseTracking;
#else
            var apiToken = settings.androidApiToken;
            var attributionProvider = settings.androidTrackingProvider;
            var integrateWithFirebase = settings.androidFirebaseSettings.enableAutomaticIntegration;
            var automaticInAppPurchaseTracking = !settings.androidDisableAutomaticInAppPurchaseTracking;
#endif
            var enableDebugMode = settings.enableDebugMode;

            string trackingId = "";
            string trackingProvider = "";
            switch (attributionProvider) {
                case AttributionProvider.Appsflyer:
#if JUSTTRACK_APPSFLYER_INTEGRATION
#if UNITY_EDITOR
                            trackingId = "editor-tracking-id";
                            trackingProvider = "editor-tracking-provider";
#else
                            trackingId = Reflection.GetAppsflyerId();
                            trackingProvider = "appsflyer";
                            JustTrackSDK.AGENT.LogDebug("Got appsflyer id " + trackingId);
#endif
#else
                    JustTrackSDK.AGENT.LogError("No support for appsflyer compiled in, but integration via appsflyer was requested");
#endif
                    break;
                case AttributionProvider.justtrack:
                default:
                    break;
            }

            JustTrackSDK.Init(apiToken, trackingId, trackingProvider, null, automaticInAppPurchaseTracking, integrateWithFirebase, enableDebugMode, (response) => {
                lock (syncLock) {
                    attributionResponse = response;
                    if (onInitialized != null) {
                        var toCall = onInitialized;
                        CallOnMainThread(() => {
                            toCall(response);
                        });
                    } else {
                        JustTrackSDK.AGENT.LogDebug("Got justtrack user id " + response.UserId);
                    }
                    onInitialized = null;
                    onError = null;
                }
            }, (error) => {
                lock (syncLock) {
                    initError = error;
                    if (onError != null) {
                        var toCall = onError;
                        CallOnMainThread(() => {
                            toCall(error);
                        });
                    } else {
                        JustTrackSDK.AGENT.LogError("Failed to initialize justtrack SDK: " + error);
                    }
                    onInitialized = null;
                    onError = null;
                }
            });
            InitIntegrations(settings);
            InitIronsource(settings);
        }

        // Retrieve the attribution produced by the SDK. If the SDK already can provide an attribution, your
        // callbacks are immediately invoked with the attribution result. Otherwise they are stored and called
        // as soon as a result is available.
        // You can call this method as many times as you want - each invocation will add your delegates to
        // the list of delegates to call as soon as the attribution is available.
        public static void GetAttribution(Action<AttributionResponse> pOnInitialized, Action<string> pOnError) {
            lock (syncLock) {
                if (attributionResponse != null) {
                    CallOnMainThread(() => {
                        pOnInitialized(attributionResponse);
                    });
                } else if (initError != null) {
                    CallOnMainThread(() => {
                        pOnError(initError);
                    });
                } else {
                    if (onInitialized == null) {
                        onInitialized = pOnInitialized;
                    } else {
                        onInitialized += pOnInitialized;
                    }
                    if (onError == null) {
                        onError = pOnError;
                    } else {
                        onError += pOnError;
                    }
                }
            }
        }

        private static int unityMainThreadId = -1;

        public static bool IsOnMainThread() {
            return Thread.CurrentThread.ManagedThreadId == unityMainThreadId;
        }

        private static Action actionsOnMainThead = null;
        private static readonly object onMainThreadLock = new object();

        public static void CallOnMainThread(Action pAction) {
            lock (onMainThreadLock) {
                if (actionsOnMainThead == null) {
                    actionsOnMainThead = pAction;
                } else {
                    actionsOnMainThead += pAction;
                }
            }
        }

        void Update() {
            Action action = null;
            lock (onMainThreadLock) {
                action = actionsOnMainThead;
                actionsOnMainThead = null;
            }
            if (action != null) {
                action();
            }
        }

        private void InitIntegrations(JustTrackSettings settings) {
#if UNITY_IOS
            if (settings.iosAdColonyIntegration) {
#else
            if (settings.androidAdColonyIntegration) {
#endif
                JustTrackSDK.IntegrateWithAdColony();
            }
#if UNITY_IOS
            if (settings.iosAppLovinIntegration) {
#else
            if (settings.androidAppLovinIntegration) {
#endif
                JustTrackSDK.IntegrateWithAppLovin();
            }
#if UNITY_IOS
            if (settings.iosChartboostIntegration) {
#else
            if (settings.androidChartboostIntegration) {
#endif
                JustTrackSDK.IntegrateWithChartboost();
            }
#if UNITY_IOS
            if (settings.iosUnityAdsIntegration) {
#else
            if (settings.androidUnityAdsIntegration) {
#endif
                JustTrackSDK.IntegrateWithUnityAds();
            }
        }

        // Set to true once IronSource has been initialized by the SDK.
        public static bool IronSourceInitialized {
            get {
                bool result;
                lock (ironSourceLock) {
                    result = isIronSourceInitialized;
                }
                return result;
            }

            private set {
                Action toCall = null;
                lock (ironSourceLock) {
                    isIronSourceInitialized = value;
                    if (isIronSourceInitialized) {
                        toCall = onIronSourceInitialized;
                        onIronSourceInitialized = null;
                    }
                }
                if (toCall != null) {
                    CallOnMainThread(toCall);
                }
            }
        }

        // Call the action once IronSource has been successfully initialized.
        // If IronSource has already been initialized, the action is called in
        // the near future.
        public static void OnIronSourceInitialized(Action pAction) {
            lock (ironSourceLock) {
                if (isIronSourceInitialized) {
                    CallOnMainThread(pAction);
                    return;
                }
                if (onIronSourceInitialized == null) {
                    onIronSourceInitialized = pAction;
                } else {
                    onIronSourceInitialized += pAction;
                }
            }
        }

        private static bool isIronSourceInitialized = false;
        private static Action onIronSourceInitialized = null;

        // Lock guarding the ironsource initialized flag and callbacks
        private static readonly object ironSourceLock = new object();

        private void InitIronsource(JustTrackSettings settings) {
            // don't try to initialize a second time once we have already initialized ironsource
            // otherwise, we might register listeners twice, causing too many events to be sent
            if (IronSourceInitialized) {
                return;
            }

#if JUSTTRACK_IRONSOURCE_INTEGRATION
#if UNITY_IOS
            var ironSourceSettings = settings.iosIronSourceSettings;
#else
            var ironSourceSettings = settings.androidIronSourceSettings;
#endif

            if (String.IsNullOrEmpty(ironSourceSettings.appKey)) {
                JustTrackSDK.AGENT.LogWarning("No IronSource app key set, not initializing");
                return;
            }

            JustTrackSDK.IntegrateWithIronSource(() => {
                JustTrackSDK.AGENT.LogDebug("Successfully integrated with IronSource");
                Reflection.IronSourceInit(ironSourceSettings);
                IronSourceInitialized = true;
            }, (error) => {
                JustTrackSDK.AGENT.LogError("Failed to integrate IronSource: " + error);
                Reflection.IronSourceInit(ironSourceSettings);
                IronSourceInitialized = true;
            });
#endif
        }
    }
}