using GameAnalyticsSDK;
using System;
using JustTrack;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Facebook.Unity;
using System.Globalization;

namespace Sunday
{
    public class SundaySDKInitializer
    {
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntimeMethodLoad()
        {
            var sdkGo = new GameObject("SundaySDK");
            sdkGo.AddComponent<SundaySDKBehaviour>();
        }
    }

    public class SundaySDKBehaviour : MonoBehaviour
    {
        public delegate void RewardedVideoResultDelegate(bool isSuccess);
        
        public static SundaySDKBehaviour Instance { get; private set; }
        
        DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        public static bool firebaseInitialized { protected set; get; } = false;
        
        public static Action OnAdStart;
        public static Action OnAdStop;
        public static Action<int> OnTestGroupFound;
        public static Action<SundayAdError> OnInterstitialFailed;
        public static Action<SundayAdError> OnRewardedFailed;

        public int ActiveTestGroupIndex { get => activeTestGroupIndex; private set => activeTestGroupIndex = value; }
        private int activeTestGroupIndex = 1;
        [HideInInspector] public SundayJusttrackIronsourceSettings activeTestGroupSettings;

        public static string activeInterstitialVideoPlacementName;
        public static float lastInterstitialShowTime;

        public static bool interstitialUnlocked { get; private set; } = false;
        
        private bool isShowingInterstitial = false;
        private bool isShowingRewarded = false;

        public string activeRewardedVideoPlacementName;
        private float lastRewardedVideoEndTime = float.MaxValue;
        private RewardedVideoResultDelegate activeRewardedVideoResultDelegate;

        private GameObject FakeRewardedCanvas;

        public IronSourceImpressionData lastActiveImpressionData;

        public Action<bool> onAdAvailibityStatusChanged;
        
        public bool IsShowingAd =>
            isShowingInterstitial || isShowingRewarded;
        
        public bool IsInterstitialAvailable => 
            activeTestGroupSettings.isInterstitialEnabled
            && !IsShowingAd
            && IronSource.Agent != null
            && IronSource.Agent.isInterstitialReady();

        public bool IsRewardedVideoAvailable =>
            activeTestGroupSettings.isRewardedVideoEnabled
            && !IsShowingAd
            && IronSource.Agent != null
            && IronSource.Agent.isRewardedVideoAvailable();

        public JustTrackSDKBehaviour JustTrackSDKBehaviourInstance { get; private set; }

        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("SundaySDK: Multiple instances detected, destroying self.");
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                Initialize();
            }
        }

        private void Initialize()
        {
            
            InitFirebase();
            
            //Initialize with test group 1 for sanity
            activeTestGroupSettings = Settings.Instance.testGroup1Settings;

            Instantiate(Resources.Load<GameObject>("BannerWhiteSpace"));
    
            InitializeFacebookSDK();
            
            bool isGameAnalyticsValid = true;
#if UNITY_ANDROID
            isGameAnalyticsValid = (Settings.Instance.gameAnalyticsAndroidGameKey != string.Empty) && (Settings.Instance.gameAnalyticsAndroidSecretKey != string.Empty);
#elif UNITY_IOS
            isGameAnalyticsValid = (Settings.Instance.gameAnalyticsIOSGameKey != string.Empty) && (Settings.Instance.gameAnalyticsIOSSecretKey != string.Empty);
#endif

            interstitialUnlocked = PlayerPrefs.GetInt("SundaySDK_InterstitialUnlocked") == 1;

            if(isGameAnalyticsValid)
            {
                //GameAnalytics
                var gaGo = new GameObject("GameAnalytics");
                //gaGo.transform.parent = transform; //commented because it creates warnings
                gaGo.AddComponent<GameAnalyticsSDK.Events.GA_SpecialEvents>();
                gaGo.AddComponent<GameAnalytics>();
                DontDestroyOnLoad(gaGo);
                GameAnalytics.Initialize();
            }
            
            bool isJustTrackValid = true;
#if UNITY_ANDROID
            isJustTrackValid = Settings.Instance.justTrackAndroidToken != string.Empty;
#elif UNITY_IOS
            isJustTrackValid = Settings.Instance.justTrackIOSToken != string.Empty;
#endif

            if (isJustTrackValid)
            {
                var jtGo = new GameObject("JustTrack");
                jtGo.transform.parent = transform;
                JustTrackSDKBehaviourInstance = jtGo.AddComponent<JustTrackSDKBehaviour>();
#if UNITY_EDITOR
                ActiveTestGroupIndex = Settings.Instance.editorTestGroupSettings;
                RunAfterTestGroupFoundOperations();
#else
                JustTrackSDK.GetTestGroupId((groupId) =>
                    {
                        if (!groupId.HasValue)
                        {
                            ActiveTestGroupIndex = 1;
                        }
                        else
                        {
                            ActiveTestGroupIndex = groupId.Value;
                        }

                        RunAfterTestGroupFoundOperations();
                    }, (e) =>
                    {
                        ActiveTestGroupIndex = 1;
                        RunAfterTestGroupFoundOperations();
                    }
                );
#endif
            }
        }

        private void InitializeFacebookSDK()
        {
            bool isFacebookSDKValid = Settings.Instance.facebookAppID != string.Empty && Settings.Instance.facebookClientToken != string.Empty;

            if (isFacebookSDKValid)
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    FB.Init(() =>
                    {
                        FB.ActivateApp();
                    });
                }
            }
        }

        private void RunAfterTestGroupFoundOperations()
        {
            switch (ActiveTestGroupIndex)
            {
                case 1:
                    activeTestGroupSettings = Settings.Instance.testGroup1Settings;
                    break;
                case 2:
                    activeTestGroupSettings = Settings.Instance.testGroup2Settings;
                    break;
                case 3:
                    activeTestGroupSettings = Settings.Instance.testGroup3Settings;
                    break;
                default:
                    activeTestGroupSettings = Settings.Instance.testGroup1Settings;
                    break;
            }

            var settings = Resources.Load<JustTrackSettings>(JustTrackSettings.JustTrackSettingsResource);
#if UNITY_ANDROID
            settings.androidIronSourceSettings.appKey = activeTestGroupSettings.ironsourceShouldInitialize ? Settings.Instance.ironSourceAndroidAppKey : "";
            settings.androidIronSourceSettings.enableBanner = activeTestGroupSettings.isBannerEnabled;
            settings.androidIronSourceSettings.enableInterstitial = activeTestGroupSettings.isInterstitialEnabled;
            settings.androidIronSourceSettings.enableRewardedVideo = activeTestGroupSettings.isRewardedVideoEnabled;
            settings.androidIronSourceSettings.enableOfferwall = activeTestGroupSettings.isOfferwallEnabled;
#elif UNITY_IOS
            settings.iosIronSourceSettings.appKey = activeTestGroupSettings.ironsourceShouldInitialize ? Settings.Instance.ironSourceIOSAppKey : "";
            settings.iosIronSourceSettings.enableBanner = activeTestGroupSettings.isBannerEnabled;
            settings.iosIronSourceSettings.enableInterstitial = activeTestGroupSettings.isInterstitialEnabled;
            settings.iosIronSourceSettings.enableRewardedVideo = activeTestGroupSettings.isRewardedVideoEnabled;
            settings.iosIronSourceSettings.enableOfferwall = activeTestGroupSettings.isOfferwallEnabled;
#endif
                    
            OnTestGroupFound?.Invoke(ActiveTestGroupIndex);
            
            GameAnalytics.SetCustomDimension01(ActiveTestGroupIndex.ToString());
                
            if (activeTestGroupSettings.ironsourceShouldInitialize)
            {
                string adQualityKey = Settings.Instance.ironSourceAdQualityKey;
                if (adQualityKey != "")
                {
                    IronSourceAdQuality.Initialize(adQualityKey);
                    JustTrackSDK.OnTrackingAuthorization(IronSourceAdQuality.SetUserConsent);
                }
            
                JustTrackSDKBehaviour.OnIronSourceInitialized(() =>
                {
                    if (activeTestGroupSettings.isInterstitialEnabled)
                    {
                        IronSource.Agent.loadInterstitial();
                        SundaySDK.Tracking.TrackInterstitial(SundaySDK.Tracking.AdEvent.Load, "IronSrc", "IronSrc", activeInterstitialVideoPlacementName);
                    }

                    if (activeTestGroupSettings.isRewardedVideoEnabled)
                    {
                        IronSource.Agent.loadRewardedVideo();
                        SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Load, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
                    }

                    if (activeTestGroupSettings.isBannerEnabled)
                    {
                        var bannerSettings = Settings.Instance.bannerSize;
                        if (activeTestGroupSettings.isAdaptiveBannerEnabled)
                        {
                            bannerSettings.SetAdaptive(true);
                        }
                        IronSource.Agent.loadBanner(bannerSettings, Settings.Instance.bannerPosition);
                        IronSource.Agent.displayBanner();
                    }
                });
            }
            
        }
        
        public void ShowRewardedVideo(string placementName = "DefaultRewardedVideo", RewardedVideoResultDelegate rewardedVideoResultDelegate = null)
        {
            if (IsShowingAd) return;

#if UNITY_EDITOR
            isShowingRewarded = true;
            lastRewardedVideoEndTime = float.MaxValue;
            
            activeRewardedVideoPlacementName = placementName;
            activeRewardedVideoResultDelegate = rewardedVideoResultDelegate;
            
            if (activeTestGroupSettings.isRewardedVideoEnabled)
            {
                RewardedVideoAdOpenedEvent();
                ShowFakeRewardedUI();
            }
            else
            {
                activeRewardedVideoResultDelegate?.Invoke(false);
                activeRewardedVideoResultDelegate = null;
                lastActiveImpressionData = null;
            }
#else
            activeRewardedVideoPlacementName = placementName;
            activeRewardedVideoResultDelegate = rewardedVideoResultDelegate;

            if (!IsRewardedVideoAvailable)
            {
                activeRewardedVideoResultDelegate?.Invoke(false);
                activeRewardedVideoResultDelegate = null;
                return;
            }

            isShowingRewarded = true;
            lastRewardedVideoEndTime = float.MaxValue;
            
            IronSource.Agent.showRewardedVideo(placementName);
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Show, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
#endif
        }

        private void ShowFakeRewardedUI()
        {
            FakeRewardedCanvas = new GameObject("FakeRewardedCanvas");
            Canvas canvas = FakeRewardedCanvas.AddComponent<Canvas>();
            FakeRewardedCanvas.AddComponent<GraphicRaycaster>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = Int16.MaxValue;
            FakeRewardedCanvas.transform.SetParent(transform);

            GameObject panel = DefaultControls.CreatePanel(new DefaultControls.Resources());
            panel.transform.SetParent(FakeRewardedCanvas.transform);
            
            RectTransform rectTrans =  panel.GetComponent<RectTransform>();
            rectTrans.offsetMax = new Vector2(0, rectTrans.offsetMax.y);
            rectTrans.offsetMax = new Vector2(rectTrans.offsetMax.x, 0);
            
            CreateButton("Success", new Vector3(100, 200, 0), Color.green, () =>
            {
                RewardedVideoAdClosedEvent();
                RewardedVideoAdRewardedEvent(null);
                Destroy(FakeRewardedCanvas);
                
            }).transform.SetParent(panel.transform);
           
            
            CreateButton("Close", new Vector3(Screen.width - 100, 200, 0), Color.red, () =>
            {
                RewardedVideoAdClosedEvent();
                Destroy(FakeRewardedCanvas);
                
            }).transform.SetParent(panel.transform);
        }

        private Button CreateButton(string text, Vector3 pos, Color col, UnityAction call)
        {
            Button button =
                DefaultControls.CreateButton(new DefaultControls.Resources()).GetComponent<Button>();

            button.onClick.AddListener(call);

            button.transform.GetComponentInChildren<Text>().text = text;

            button.transform.position = pos;

            button.transform.GetComponent<Image>().color = col;

            return button;
        }

        private void Update()
        {
            if (isShowingRewarded && Time.unscaledTime > lastRewardedVideoEndTime + SundaySDK.REWARDED_AD_TIMEOUT_DURATION && activeRewardedVideoResultDelegate != null)
            {
                isShowingRewarded = false;
                activeRewardedVideoResultDelegate?.Invoke(false);
                activeRewardedVideoResultDelegate = null;
                lastRewardedVideoEndTime = float.MaxValue;
                lastActiveImpressionData = null;
            }
        }

        private void OnEnable()
        {
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;

            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += onAdAvailibityStatusChanged;

            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;

            IronSourceEvents.onImpressionSuccessEvent += ImpressionSuccessEvent;
            IronSourceEvents.onImpressionDataReadyEvent += ImpressionDataReadyEvent;
        }

        private void ImpressionSuccessEvent(IronSourceImpressionData isid)
        {

        }
        
        private void ImpressionDataReadyEvent(IronSourceImpressionData isid)
        {
            lastActiveImpressionData = isid;
            
            if (Settings.Instance.adImpressionForwarding)
            {
                SendFirebaseAdEventWithImpressionData(isid);
            }
        }

        private void InitFirebase()
        {
            if (Settings.Instance.isFirebaseEnabled)
            {
                FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread((System.Threading.Tasks.Task<DependencyStatus> task) => {
                    dependencyStatus = task.Result;
                    if (dependencyStatus == DependencyStatus.Available) 
                    {
                        Debug.Log("Enabling data collection.");
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                        Debug.Log("Set user properties.");
                        // Set the user's sign up method.
                        FirebaseAnalytics.SetUserProperty(
                            FirebaseAnalytics.UserPropertySignUpMethod,
                            "Google");
                        // Set default session duration values.
                        FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
                        FirebaseApp app = FirebaseApp.DefaultInstance;
                        firebaseInitialized = true;
                    } 
                    else 
                    {
                        Debug.LogError(
                            "Could not resolve all Firebase dependencies: " + dependencyStatus);
                    }
                });
            }
        }

        private void SendFirebaseAdEventWithImpressionData(IronSourceImpressionData data)
        {
            if (!firebaseInitialized)
            {
                return;
            }

            if (data == null)
            {
                return;
            }

            double revenue = FixDouble(data.revenue);
            Parameter[] adParameters = {
                new Parameter("ad_platform", "ironSource"),
                new Parameter("ad_source", string.IsNullOrEmpty(data.adNetwork) ? data.adNetwork : "Unknown"),
                new Parameter("ad_unit_name", string.IsNullOrEmpty(data.instanceName) ? data.instanceName : "Unknown"),
                new Parameter("ad_format", data.adUnit),
                new Parameter("currency","USD"),
                new Parameter("value", revenue)
            };
            
            FirebaseAnalytics.LogEvent("ad_impression", adParameters);
        }

        private double FixDouble(double? d)
        {
            double returnedValue = d ?? 0.0;

            decimal roundThreshold = (decimal)0.00005;
            string convertedValue = returnedValue.ToString() ?? string.Empty;
            if (convertedValue.EndsWith("E-05") || convertedValue.EndsWith("e-05"))
            {
                string formattedValue = returnedValue.ToString("###.######");
                decimal x = Convert.ToDecimal(formattedValue);

                decimal fixedValue = Math.Round(x, 5);
                if (fixedValue < roundThreshold)
                {
                    fixedValue = fixedValue + roundThreshold;
                }

                fixedValue = Math.Round(fixedValue, 4, MidpointRounding.AwayFromZero);
                return Convert.ToDouble(fixedValue);
            }

            return returnedValue;
        }

        public void ResetInterstitialTime()
        {
            lastInterstitialShowTime = Time.unscaledTime;
        }

        public static void UnlockInterstitialAds()
        {
            interstitialUnlocked = true;
            PlayerPrefs.SetInt("SundaySDK_InterstitialUnlocked", 1);
        }
        
        public static void LockInterstitialAds()
        {
            interstitialUnlocked = false;
            PlayerPrefs.SetInt("SundaySDK_InterstitialUnlocked", 0);
        }

        //Invoked when the initialization process has failed.
        //@param description - string - contains information about the failure.
        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
        }
        //Invoked when the ad fails to show.
        //@param description - string - contains information about the failure.
        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            if (firebaseInitialized)
            {
                float responseTime = Time.unscaledTime - SundaySDK.Monetization.lastAdRequestTime;
                
                Parameter[] firebaseParams =
                {
                    new Parameter("Ad_Type",  "interstitial"),
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  activeInterstitialVideoPlacementName),
                    new Parameter("Reason",  error.getCode().ToString()),
                    new Parameter("Resp_Time",  responseTime.ToString("0.0000", CultureInfo.InvariantCulture)),
                };
                
                FirebaseAnalytics.LogEvent("Ad_Resp", firebaseParams);

                lastActiveImpressionData = null;
            }

            if (error != null && !string.IsNullOrEmpty(error.getDescription()))
            {
                OnInterstitialFailed?.Invoke(new SundayAdError("Interstitial Failed: " + error.getDescription(), "Ironsource"));
            }
        }
        // Invoked when end user clicked on the interstitial ad
        void InterstitialAdClickedEvent()
        {
            SundaySDK.Tracking.TrackInterstitial(SundaySDK.Tracking.AdEvent.Click, "IronSrc", "IronSrc", activeInterstitialVideoPlacementName);
        }
        //Invoked when the interstitial ad closed and the user goes back to the application screen.
        void InterstitialAdClosedEvent()
        {
            OnAdStop?.Invoke();
            isShowingInterstitial = false;
            ResetInterstitialTime();
            IronSource.Agent.loadInterstitial();
            
            if (firebaseInitialized)
            {
                float responseTime = Time.unscaledTime - SundaySDK.Monetization.lastAdRequestTime;
                
                Parameter[] firebaseParams =
                {
                    new Parameter("Ad_Type",  "interstitial"),
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  activeInterstitialVideoPlacementName),
                    new Parameter("Placement",  activeInterstitialVideoPlacementName),
                    new Parameter("Reason",  "Filled"),
                    new Parameter("Ad_Network",  lastActiveImpressionData != null ? lastActiveImpressionData.adNetwork : "Unknown"),
                    new Parameter("Resp_Time",  responseTime.ToString("0.0000", CultureInfo.InvariantCulture)),
                };
                
                FirebaseAnalytics.LogEvent("Ad_Resp", firebaseParams);
            }

            lastActiveImpressionData = null;
            SundaySDK.Tracking.TrackInterstitial(SundaySDK.Tracking.AdEvent.Close, "IronSrc", "IronSrc", activeInterstitialVideoPlacementName);
        }
        //Invoked when the Interstitial is Ready to shown after load function is called
        void InterstitialAdReadyEvent()
        {

        }
        //Invoked when the Interstitial Ad Unit has opened
        void InterstitialAdOpenedEvent()
        {
            OnAdStart?.Invoke();
            isShowingInterstitial = true;
            ResetInterstitialTime();
            SundaySDK.Tracking.TrackInterstitial(SundaySDK.Tracking.AdEvent.Open, "IronSrc", "IronSrc", activeInterstitialVideoPlacementName);
        }
        //Invoked right before the Interstitial screen is about to open. NOTE - This event is available only for some of the networks. 
        //You should treat this event as an interstitial impression, but rather use InterstitialAdOpenedEvent
        void InterstitialAdShowSucceededEvent()
        {
            SundaySDK.Tracking.TrackInterstitial(SundaySDK.Tracking.AdEvent.Show, "IronSrc", "IronSrc", activeInterstitialVideoPlacementName);
        }

        //Invoked when the RewardedVideo ad view has opened.
        //Your Activity will lose focus. Please avoid performing heavy 
        //tasks till the video ad will be closed.
        void RewardedVideoAdOpenedEvent()
        {
            OnAdStart?.Invoke();
            isShowingRewarded = true;
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Open, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
        }
        //Invoked when the RewardedVideo ad view is about to be closed.
        //Your activity will now regain its focus.
        void RewardedVideoAdClosedEvent()
        {
            OnAdStop?.Invoke();
            lastInterstitialShowTime = lastRewardedVideoEndTime = Time.unscaledTime;
            
            if (firebaseInitialized)
            {
                float responseTime = Time.unscaledTime - SundaySDK.Monetization.lastAdRequestTime;
                
                Parameter[] firebaseParams =
                {
                    new Parameter("Ad_Type",  "rewarded"),
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  activeRewardedVideoPlacementName),
                    new Parameter("Reason",  "Filled"),
                    new Parameter("Ad_Network",  lastActiveImpressionData != null ? lastActiveImpressionData.adNetwork : "Unknown"),
                    new Parameter("Resp_Time",  responseTime.ToString("0.0000", CultureInfo.InvariantCulture)),
                };
                
                FirebaseAnalytics.LogEvent("Ad_Resp", firebaseParams);
            }
            
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Close, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
        }
        //Invoked when there is a change in the ad availability status.
        //@param - available - value will change to true when rewarded videos are available. 
        //You can then show the video by calling showRewardedVideo().
        //Value will change to false when no videos are available.
        void RewardedVideoAvailabilityChangedEvent(bool available)
        {
            //Change the in-app 'Traffic Driver' state according to availability.
        }

        //Invoked when the user completed the video and should be rewarded. 
        //If using server-to-server callbacks you may ignore this events and wait for 
        // the callback from the  ironSource server.
        //@param - placement - placement object which contains the reward data
        void RewardedVideoAdRewardedEvent(IronSourcePlacement placement)
        {
            isShowingRewarded = false; //just to be safe
            lastRewardedVideoEndTime = float.MaxValue;
            
            activeRewardedVideoResultDelegate?.Invoke(true);
            activeRewardedVideoResultDelegate = null;
            lastActiveImpressionData = null;
        }
        //Invoked when the Rewarded Video failed to show
        //@param description - string - contains information about the failure.
        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            isShowingRewarded = false; //just to be safe

            activeRewardedVideoResultDelegate?.Invoke(false);
            activeRewardedVideoResultDelegate = null;
            lastActiveImpressionData = null;
            
            if (error != null && !string.IsNullOrEmpty(error.getDescription()))
            {
                OnRewardedFailed?.Invoke(new SundayAdError("Rewarded Failed: " + error.getDescription(), "Ironsource"));
            }
            
            if (firebaseInitialized)
            {
                float responseTime = Time.unscaledTime - SundaySDK.Monetization.lastAdRequestTime;
                
                Parameter[] firebaseParams =
                {
                    new Parameter("Ad_Type",  "rewarded"),
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  activeRewardedVideoPlacementName),
                    new Parameter("Reason",  error.getCode().ToString()),
                    new Parameter("Resp_Time",  responseTime.ToString("0.0000", CultureInfo.InvariantCulture)),
                };
                
                FirebaseAnalytics.LogEvent("Ad_Resp", firebaseParams);
            }
        }

        // ----------------------------------------------------------------------------------------
        // Note: the events below are not available for all supported rewarded video ad networks. 
        // Check which events are available per ad network you choose to include in your build. 
        // We recommend only using events which register to ALL ad networks you include in your build. 
        // ----------------------------------------------------------------------------------------

        //Invoked when the video ad starts playing. 
        void RewardedVideoAdStartedEvent()
        {
            ResetInterstitialTime();
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Open, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
        }
        //Invoked when the video ad finishes playing. 
        void RewardedVideoAdEndedEvent()
        {
            ResetInterstitialTime();
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Close, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
        }
        //Invoked when the video ad is clicked. 
        void RewardedVideoAdClickedEvent(IronSourcePlacement ironSourcePlacement)
        {
            SundaySDK.Tracking.TrackRewarded(SundaySDK.Tracking.AdEvent.Click, "IronSrc", "IronSrc", activeRewardedVideoPlacementName);
        }

        private void BannerAdLoadedEvent()
        {
            BannerWhiteSpaceHandler.SetWhiteSpaceActive(true);
            SundaySDK.Tracking.TrackBanner(SundaySDK.Tracking.AdEvent.Load, "IronSrc", "IronSrc", Settings.Instance.bannerPosition.ToString());
        }

        private void BannerAdClickedEvent()
        {
            SundaySDK.Tracking.TrackBanner(SundaySDK.Tracking.AdEvent.Click, "IronSrc", "IronSrc", Settings.Instance.bannerPosition.ToString());
        }
        
        private void OnApplicationPause(bool pause)
        {
            if (!JustTrackSDKBehaviour.IronSourceInitialized) return;
    
            IronSource.Agent.onApplicationPause(pause);
        }
    }
}