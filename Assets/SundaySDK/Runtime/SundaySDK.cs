using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sunday;
using GameAnalyticsSDK;
using System;
using Firebase.Analytics;
using JustTrack;

public static class SundaySDK
{
    public const float REWARDED_AD_TIMEOUT_DURATION = 5;
    
    public static int ActiveTestGroupIndex
    {
        get
        {
            if(SundaySDKBehaviour.Instance != null)
            {
                return SundaySDKBehaviour.Instance.ActiveTestGroupIndex;
            }
            else
            {
                return 0;
            }
        }
    }

    public static SundayJusttrackIronsourceSettings ActiveTestGroupSettings
    {
        get
        {
            if (SundaySDKBehaviour.Instance != null)
            {
                return SundaySDKBehaviour.Instance.activeTestGroupSettings;
            }
            else
            {
                return null;
            }
        }
    }
    
    public static int LastLevelStartIndexReceived
    {
        get
        {
            return PlayerPrefs.GetInt("LastLevelStartIndexReceived", 0);
        }
        set
        {
            PlayerPrefs.SetInt("LastLevelStartIndexReceived", value);
            PlayerPrefs.Save();
        }
    }

    public static class Tracking
    {
        public enum ProgressEvent
        {
            Start,
            Fail,
            Complete
        }

        internal enum AdEvent
        {
            Load,
            Open,
            Close,
            Click,
            Show
        }

        public static void TrackLevelStart(int level, string param1 = "", string param2 = "")
        {
            LastLevelStartIndexReceived = level;
            TrackProgress(ProgressEvent.Start, level, param1, param2);
        }

        public static void TrackLevelComplete(int level, string param1 = "", string param2 = "")
        {
            TrackProgress(ProgressEvent.Complete, level, param1, param2);
        }

        public static void TrackLevelFail(int level, string param1 = "", string param2 = "")
        {
            TrackProgress(ProgressEvent.Fail, level, param1, param2);
        }


        /// <summary>
        /// The most simple event
        /// </summary>
        /// <param name="eventName">Name of the event</param>
        public static void TrackEvent(string eventName)
        {
            GameAnalytics.NewDesignEvent(eventName);
            JustTrack.JustTrackSDK.PublishEvent(eventName);
            
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                };
                
                FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            }
        }

        /// <summary>
        /// Send event with more details
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="category"></param>
        /// <param name="element"></param>
        /// <param name="action"></param>
        public static void TrackEvent(string eventName, string category, string element, string action)
        {
            string eventString = string.Format("{0}:{1}:{2}:{3}", eventName, category, element, action);
            GameAnalytics.NewDesignEvent(eventString);

            var events = new JustTrack.EventDetails(eventName, category, element, action);
            JustTrack.JustTrackSDK.PublishEvent(events);
            
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Category", category),
                    new Parameter("Element", element),
                    new Parameter("Action", action),
                };
                
                FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            }
        }

        /// <summary>
        /// Set User Property Dimension
        /// </summary>
        public static void TrackSetUserProperty(string pname, string value)
        {
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                FirebaseAnalytics.SetUserProperty(pname, value);
            }
        }

        /// <summary>
        /// Send and event with a numeric relevant value
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="category"></param>
        /// <param name="element"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        public static void TrackEventCount(string eventName, string category, string element, string action, float value)
        {
            string eventString = string.Format("{0}:{1}:{2}:{3}", eventName, category, element, action);
            GameAnalytics.NewDesignEvent(eventString, value);

            var events = new JustTrack.CustomUserEvent(new JustTrack.EventDetails(eventName, category, element, action)).SetValueAndUnit(value, JustTrack.Unit.Count);
            JustTrack.JustTrackSDK.PublishEvent(events);
        }

        /// <summary>
        /// Send a custom event with relevant duration information
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="category"></param>
        /// <param name="element"></param>
        /// <param name="action"></param>
        /// <param name="value"></param>
        public static void TrackEventDuration(string eventName, string category, string element, string action, double value)
        {
            string eventString = string.Format("{0}:{1}:{2}:{3}", eventName, category, element, action);
            GameAnalytics.NewDesignEvent(eventString, (float)value);

            var events = new JustTrack.CustomUserEvent(new JustTrack.EventDetails(eventName, category, element, action)).SetValueAndUnit(value, JustTrack.Unit.Seconds);
            JustTrack.JustTrackSDK.PublishEvent(events);
        }

        private static GAProgressionStatus GetGameAnalyticsStatus(ProgressEvent progressEvent)
        {
            GAProgressionStatus status;
            switch (progressEvent)
            {
                case ProgressEvent.Start:
                    status = GAProgressionStatus.Start;
                    break;
                case ProgressEvent.Fail:
                    status = GAProgressionStatus.Fail;
                    break;
                case ProgressEvent.Complete:
                    status = GAProgressionStatus.Complete;
                    break;
                default:
                    status = GAProgressionStatus.Undefined;
                    break;
            }
            return status;
        }

        /// <summary>
        /// Track the progress of the game
        /// </summary>
        /// <param name="progressEvent"> Select weather it's a starting failing or completion event.</param>
        /// <param name="level"> The number of the current level.</param>
        /// <param name="eventName"></param>
        /// <param name="details"> Used only by GameAnalytics as a second parameter </param>
        public static void TrackProgress(ProgressEvent progressEvent, int level, string eventName = null, string details = "")
        {
            string levelId = string.Format("Level_{0}", level);
            string progress = "";

            JustTrack.UserEvent justrackEvent = null;
            switch (progressEvent)
            {
                case ProgressEvent.Start:
                    progress = "Start";
                    justrackEvent = new JustTrack.ProgressionLevelStartEvent((eventName != null) ? eventName : levelId, levelId);
                    break;
                case ProgressEvent.Fail:
                    progress = "Fail";
                    justrackEvent = new JustTrack.ProgressionLevelFailEvent((eventName != null) ? eventName : levelId, levelId);
                    break;
                case ProgressEvent.Complete:
                    progress = "Complete";
                    justrackEvent = new JustTrack.ProgressionLevelFinishEvent((eventName != null) ? eventName : levelId, levelId);
                    break;
            }
            Debug.Log("Track Progress:" + progress);
            
            
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("eventName", (eventName != null) ? eventName : levelId),
                    new Parameter("progressType", progress),
                    new Parameter("levelId", levelId),
                    new Parameter("details", details)
                };
                
                FirebaseAnalytics.LogEvent(levelId + "_" + progress, firebaseParams);
            }
            
            //Todo: clean up
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("Level_Index", level.ToString()),
                };
                
                FirebaseAnalytics.LogEvent("Level_" + progress, firebaseParams);
            }

            var status = GetGameAnalyticsStatus(progressEvent);

            if (string.IsNullOrEmpty(eventName))
            {
                GameAnalytics.NewProgressionEvent(status, levelId);
            }
            else
            {
                GameAnalytics.NewProgressionEvent(status, levelId, eventName, details);
            }

            JustTrack.JustTrackSDK.PublishEvent(justrackEvent);
        }

        internal static void TrackInterstitial(AdEvent adEvent, string sdkName, string network, string placement)
        {
            switch (adEvent)
            {
                case AdEvent.Click:
                    GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Interstitial, network, placement);
                    break;
                case AdEvent.Load:
                    GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Interstitial, network, placement);
                    break;
                case AdEvent.Show:
                    GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, network, placement);
                    break;
            }

            switch (adEvent)
            {
                case AdEvent.Load:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdInterstitialLoadEvent(sdkName, network, placement));
                    break;
                case AdEvent.Open:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdInterstitialOpenEvent(sdkName, network, placement));
                    break;
                case AdEvent.Close:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdInterstitialCloseEvent(sdkName, network, placement));
                    break;
                case AdEvent.Show:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdInterstitialShowEvent(sdkName, network, placement));
                    break;
                case AdEvent.Click:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdInterstitialClickEvent(sdkName, network, placement));
                    break;
            }
        }

        internal static void TrackRewarded(AdEvent adEvent, string sdkName, string network, string placement)
        {
            switch (adEvent)
            {
                case AdEvent.Click:
                    GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.RewardedVideo, network, placement);
                    break;
                case AdEvent.Load:
                    GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.RewardedVideo, network, placement);
                    break;
                case AdEvent.Show:
                    GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.RewardedVideo, network, placement);
                    break;
            }

            switch (adEvent)
            {
                case AdEvent.Load:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdRewardedLoadEvent(sdkName, network, placement));
                    break;
                case AdEvent.Open:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdRewardedOpenEvent(sdkName, network, placement));
                    break;
                case AdEvent.Close:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdRewardedCloseEvent(sdkName, network, placement));
                    break;
                case AdEvent.Show:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdRewardedShowEvent(sdkName, network, placement));
                    break;
                case AdEvent.Click:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdRewardedClickEvent(sdkName, network, placement));
                    break;
            }
        }

        internal static void TrackBanner(AdEvent adEvent, string sdkName, string network, string placement)
        {
            switch (adEvent)
            {
                case AdEvent.Click:
                    GameAnalytics.NewAdEvent(GAAdAction.Clicked, GAAdType.Banner, network, placement);
                    break;
                case AdEvent.Load:
                    GameAnalytics.NewAdEvent(GAAdAction.Loaded, GAAdType.Banner, network, placement);
                    break;
            }

            switch (adEvent)
            {
                case AdEvent.Load:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdBannerLoadEvent(sdkName, network, placement));
                    break;
                case AdEvent.Open:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdBannerOpenEvent(sdkName, network, placement));
                    break;
                case AdEvent.Close:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdBannerCloseEvent(sdkName, network, placement));
                    break;
                case AdEvent.Show:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdBannerShowEvent(sdkName, network, placement));
                    break;
                case AdEvent.Click:
                    JustTrack.JustTrackSDK.PublishEvent(new JustTrack.AdBannerClickEvent(sdkName, network, placement));
                    break;
            }
        }

        public static void TrackAppOpenAds(double revenue)
        {
            JustTrackSDK.ForwardAdImpression(JustTrack.AdFormat.AppOpen, "Admob", "Admob",
                "AppOpenAd", null, null, null, null,
                new Money(revenue, "USD"));
        }
    }

    public static class Monetization
    {
        public static Action OnAdStart { get { return SundaySDKBehaviour.OnAdStart; } set { SundaySDKBehaviour.OnAdStart = value; } }
        public static Action OnAdStop { get { return SundaySDKBehaviour.OnAdStop; } set { SundaySDKBehaviour.OnAdStop = value; } }
        public static Action<int> OnTestGroupFound { get { return SundaySDKBehaviour.OnTestGroupFound; } set { SundaySDKBehaviour.OnTestGroupFound = value; } }

        public static Action<SundayAdError> OnInterstitialFailed { get { return SundaySDKBehaviour.OnInterstitialFailed; } set { SundaySDKBehaviour.OnInterstitialFailed = value; }}
        public static Action<SundayAdError> OnRewardedFailed { get { return SundaySDKBehaviour.OnRewardedFailed; } set { SundaySDKBehaviour.OnRewardedFailed = value; }}

        public static bool IsInterstitialAvailable =>
            SundaySDKBehaviour.Instance != null
            && SundaySDKBehaviour.Instance.IsInterstitialAvailable;
        
        public static bool IsRewardedVideoAvailable =>
            SundaySDKBehaviour.Instance != null
            && SundaySDKBehaviour.Instance.IsRewardedVideoAvailable;
        
        public static Action<bool> OnAdAvailibityStatusChanged
        {
            get
            {
                if (SundaySDKBehaviour.Instance != null)
                    return SundaySDKBehaviour.Instance.onAdAvailibityStatusChanged;
                
                return null;
            }
        }
        
        public static float lastAdRequestTime;

        public static void UnlockInterstitialAds()
        {
            SundaySDKBehaviour.UnlockInterstitialAds();
        }

        private static bool IsInterstitialCooldownReady()
        {
            return Time.unscaledTime > SundaySDKBehaviour.lastInterstitialShowTime +
                SundaySDKBehaviour.Instance.activeTestGroupSettings.interstitialCooldown;
        }

        public static void ShowInterstitial(string placementName, bool ignoreInterval = false)
        {
            if (SundaySDKBehaviour.Instance == null)
            {
                OnInterstitialFailed?.Invoke(new SundayAdError("Interstitial Failed: SundaySDK is not initialized yet", "Ironsource"));
                return;
            }

            if (SundaySDKBehaviour.Instance.IsShowingAd)
            {
                OnInterstitialFailed?.Invoke(new SundayAdError("Interstitial Failed: There is an ongoing ad", "Ironsource"));
                return;
            }
            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  placementName)
                };
                
                FirebaseAnalytics.LogEvent("Ad_Int_Trigger", firebaseParams);
            }
            
            SundaySDKBehaviour.activeInterstitialVideoPlacementName = placementName;

            if (ignoreInterval || IsInterstitialCooldownReady())
            {
                if (SundaySDKBehaviour.firebaseInitialized)
                {
                    Parameter[] firebaseParams =
                    {
                        new Parameter("Ad_Type",  "interstitial"),
                        new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                        new Parameter("Placement",  placementName)
                    };
                
                    FirebaseAnalytics.LogEvent("Ad_Opp", firebaseParams);
                }

                if (IsInterstitialAvailable)
                {
                    if (SundaySDKBehaviour.firebaseInitialized)
                    {
                        Parameter[] firebaseParams =
                        {
                            new Parameter("Ad_Type",  "interstitial"),
                            new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                            new Parameter("Placement",  placementName)
                        };
                
                        FirebaseAnalytics.LogEvent("Ad_Req", firebaseParams);
                        lastAdRequestTime = Time.unscaledTime;
                    }
                
                    IronSource.Agent.showInterstitial(SundaySDKBehaviour.activeInterstitialVideoPlacementName);
                }
                else
                {
                    OnInterstitialFailed?.Invoke(new SundayAdError("Interstitial Failed: ad is not available", "Ironsource"));
                }
            }
            else
            {
                OnInterstitialFailed?.Invoke(new SundayAdError("Interstitial Failed: Cooldown is not ready yet", "Ironsource"));
            }
        }

        public static void ShowRewardedVideo(string placementName = "DefaultRewardedVideo", SundaySDKBehaviour.RewardedVideoResultDelegate rewardedVideoResultDelegate = null)
        {
            if (SundaySDKBehaviour.Instance == null)
            {
                OnRewardedFailed?.Invoke(new SundayAdError("Rewarded Failed: SundaySDK is not initialized yet", "Ironsource"));
                return;
            }

            if (SundaySDKBehaviour.firebaseInitialized)
            {
                Parameter[] firebaseParams =
                {
                    new Parameter("Ad_Type",  "rewarded"),
                    new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                    new Parameter("Placement",  placementName)
                };
                
                FirebaseAnalytics.LogEvent("Ad_Opp", firebaseParams);
            }
            
            if (IsRewardedVideoAvailable || Application.isEditor)
            {
                
                if (SundaySDKBehaviour.firebaseInitialized)
                {
                    Parameter[] firebaseParams =
                    {
                        new Parameter("Ad_Type",  "rewarded"),
                        new Parameter("Level_Index", SundaySDK.LastLevelStartIndexReceived.ToString()),
                        new Parameter("Placement",  placementName)
                    };
                
                    FirebaseAnalytics.LogEvent("Ad_Req", firebaseParams);
                    lastAdRequestTime = Time.unscaledTime;
                }
            
                SundaySDKBehaviour.Instance.ShowRewardedVideo(placementName, rewardedVideoResultDelegate);
            }
            else
            {
                OnRewardedFailed?.Invoke(new SundayAdError("Rewarded Failed: ad is not available", "Ironsource"));

            }

        }

        public static void ResetInterstitialTime()
        {
            if (SundaySDKBehaviour.Instance == null) return;
            
            SundaySDKBehaviour.Instance.ResetInterstitialTime();
        }
    }
}
