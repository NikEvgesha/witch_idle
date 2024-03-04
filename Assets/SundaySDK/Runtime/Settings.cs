using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

#if UNITY_EDITOR
using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Linq;
using Facebook.Unity.Settings;
using UnityEditor;
#endif

using JustTrack;

namespace Sunday
{
    [System.Serializable]
    public class SundayJusttrackIronsourceSettings
    {
        public bool ironsourceShouldInitialize = true;
        public bool isBannerEnabled = true;
        public bool isAdaptiveBannerEnabled = true;
        public bool isInterstitialEnabled = true;
        public float interstitialCooldown = 30;
        public bool isRewardedVideoEnabled = true;
        public bool isOfferwallEnabled = true;
    }

    public class Settings : ScriptableObject
    {
        public const string SUNDAY_SETTINGS_PATH = "Assets/Resources/SundaySDK/Settings.asset";
        private const string GAMEANALYTICS_SETTINGS_PATH = "Assets/Resources/GameAnalytics/Settings.asset";
        private const string JUSTTRACK_SETTINGS_PATH = "Assets/JustTrack/Resources/JustTrackSettings.asset";
        private const string JUSTTRACK_BUILD_SETTINGS_PATH = "Assets/JustTrack/Resources/JustTrackBuildSettings.asset";

        private const string IRON_SOURCE_MEDIATION_SETTINGS_PATH = "Assets/IronSource/Resources/IronSourceMediationSettings.asset";
        private const string IRON_SOURCE_NETWORK_SETTINGS_PATH = "Assets/IronSource/Resources/IronSourceMediatedNetworkSettings.asset";

        private const string FACEBOOK_SETTINGS_PATH = "Assets/FacebookSDK/SDK/Resources/FacebookSettings.asset";
        
        static Settings _instance = null;
        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<Settings>("SundaySDK/Settings");
                    if(_instance == null)
                    {
                        _instance = new Settings(); //Use empty settings with default values if settings was not defined.
                    }
                }
                return _instance;
            }
        }

        [Header("Tracking")]
        public string facebookAppID = "";
        public string facebookClientToken = "";
        public string iOSAppId = "";
        public string gameAnalyticsAndroidGameKey;
        public string gameAnalyticsAndroidSecretKey;
        public string gameAnalyticsIOSGameKey;
        public string gameAnalyticsIOSSecretKey;
        public string justTrackAndroidToken;
        public string justTrackIOSToken;

        [Header("Ads")]
        public string ironSourceAndroidAppKey;
        public string ironSourceIOSAppKey;
        public string ironSourceAdQualityKey;

        [Space]
        
        [Min(1)] public int editorTestGroupSettings = 1;
        public SundayJusttrackIronsourceSettings testGroup1Settings;
        public SundayJusttrackIronsourceSettings testGroup2Settings;
        public SundayJusttrackIronsourceSettings testGroup3Settings;

        public IronSourceBannerSize bannerSize = IronSourceBannerSize.BANNER;
        public IronSourceBannerPosition bannerPosition = IronSourceBannerPosition.BOTTOM;

        public string adMobAndroidID;
        public string adMobIosID;

        public bool isFirebaseEnabled = false;
        public bool adImpressionForwarding = false;
        [HideInInspector]
        public bool detailedLogging = false;

#if UNITY_EDITOR

        public void ValidateAll()
        {
            InitializeSettings();
            ReconfigureGameAnalytics();
            ReconfigureJustTrack();
            ReconfigureJustTrackBuildSettings();
            ReconfigureFacebookSDK();

            SetAdmobIDInAndroidManifest(adMobAndroidID);
            SetFacebookCredentialsInAndroidManifest(facebookAppID, facebookClientToken);
        }

        public static void InitializeSettings()
        {
            if (!Directory.Exists(Application.dataPath + "/Resources"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Resources");
            }
            if (!Directory.Exists(Application.dataPath + "/Resources/SundaySDK"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Resources/SundaySDK");
            }

            if (!File.Exists(SUNDAY_SETTINGS_PATH))
            {
                Settings settings = ScriptableObject.CreateInstance<Settings>();
                AssetDatabase.CreateAsset(settings, SUNDAY_SETTINGS_PATH);

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void ReconfigureGameAnalytics()
        {
            if (!Directory.Exists(Application.dataPath + "/Resources"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Resources");
            }
            if (!Directory.Exists(Application.dataPath + "/Resources/GameAnalytics"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Resources/GameAnalytics");
            }

            GameAnalyticsSDK.Setup.Settings settings;

            if (File.Exists(GAMEANALYTICS_SETTINGS_PATH))
            {
                settings = AssetDatabase.LoadAssetAtPath<GameAnalyticsSDK.Setup.Settings>(GAMEANALYTICS_SETTINGS_PATH);
            }
            else
            {
                settings = ScriptableObject.CreateInstance<GameAnalyticsSDK.Setup.Settings>();
                AssetDatabase.CreateAsset(settings, GAMEANALYTICS_SETTINGS_PATH);
            }

            if (settings != null)
            {
                for (int i = settings.Platforms.Count - 1; i >= 0; --i)
                {
                    settings.RemovePlatformAtIndex(i);
                }

                //Add android
                if (!string.IsNullOrWhiteSpace(gameAnalyticsAndroidGameKey) && !string.IsNullOrWhiteSpace(gameAnalyticsAndroidSecretKey))
                {
                    settings.AddPlatform(RuntimePlatform.Android);
                    int androidIndex = settings.Platforms.IndexOf(RuntimePlatform.Android);
                    settings.UpdateGameKey(androidIndex, gameAnalyticsAndroidGameKey);
                    settings.UpdateSecretKey(androidIndex, gameAnalyticsAndroidSecretKey);
                }

                //Add ios
                if (!string.IsNullOrWhiteSpace(gameAnalyticsIOSGameKey) && !string.IsNullOrWhiteSpace(gameAnalyticsIOSSecretKey))
                {
                    settings.AddPlatform(RuntimePlatform.IPhonePlayer);
                    int androidIndex = settings.Platforms.IndexOf(RuntimePlatform.IPhonePlayer);
                    settings.UpdateGameKey(androidIndex, gameAnalyticsIOSGameKey);
                    settings.UpdateSecretKey(androidIndex, gameAnalyticsIOSSecretKey);
                }

                settings.UsePlayerSettingsBuildNumber = true;

                settings.CustomDimensions01 = new List<string>();
                settings.CustomDimensions01.Add("0");
                settings.CustomDimensions01.Add("1");
                settings.CustomDimensions01.Add("2");
                settings.CustomDimensions01.Add("3");

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void ReconfigureFacebookSDK()
        {
            if (!Directory.Exists(Application.dataPath + "/FacebookSDK"))
            {
                Directory.CreateDirectory(Application.dataPath + "/FacebookSDK");
            }
            if (!Directory.Exists(Application.dataPath + "/FacebookSDK/SDK"))
            {
                Directory.CreateDirectory(Application.dataPath + "/FacebookSDK/SDK");
            }
            if (!Directory.Exists(Application.dataPath + "/FacebookSDK/SDK/Resources"))
            {
                Directory.CreateDirectory(Application.dataPath + "/FacebookSDK/SDK/Resources");
            }
            
            // Most of the public members of FacebookSettings class are static, so we need to set them directly instead
            // of creating new instance of settings object
            if (File.Exists(FACEBOOK_SETTINGS_PATH))
            {
                FacebookSettings.AppIds[0] = facebookAppID;
                FacebookSettings.ClientTokens[0] = facebookClientToken;
                // Because this flag only works on Web Player
                FacebookSettings.Logging = false;
                
                // Attempts to initialize the Facebook object with valid session data
                FacebookSettings.Status = true;
                
                EditorUtility.SetDirty(FacebookSettings.Instance);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        private void ReconfigureJustTrack()
        {
            if (!Directory.Exists(Application.dataPath + "/JustTrack"))
            {
                Directory.CreateDirectory(Application.dataPath + "/JustTrack");
            }
            if (!Directory.Exists(Application.dataPath + "/JustTrack/Resources"))
            {
                Directory.CreateDirectory(Application.dataPath + "/JustTrack/Resources");
            }

            JustTrackSettings settings;

            if (File.Exists(JUSTTRACK_SETTINGS_PATH))
            {
                settings = AssetDatabase.LoadAssetAtPath<JustTrackSettings>(JUSTTRACK_SETTINGS_PATH);
            }
            else
            {
                settings = ScriptableObject.CreateInstance<JustTrackSettings>();
                AssetDatabase.CreateAsset(settings, JUSTTRACK_SETTINGS_PATH);
            }

            if (settings != null)
            {
                settings.androidApiToken = justTrackAndroidToken;
                settings.iosApiToken = justTrackIOSToken;
                settings.androidTrackingProvider = AttributionProvider.justtrack;
                settings.iosTrackingProvider = AttributionProvider.justtrack;

                settings.iosTrackingSettings.requestTrackingPermission = true;
                settings.iosTrackingSettings.trackingPermissionDescription = 
                    "Your data will only be used to deliver personalized ads to you";
                settings.iosTrackingSettings.facebookAudienceNetworkIntegration =
                    FacebookAudienceNetworkIntegration.NativeIntegration;

                settings.androidIronSourceSettings.appKey = ironSourceAndroidAppKey;
                if (ironSourceAndroidAppKey != "")
                {
                    settings.androidIronSourceSettings.enableBanner = true;
                    settings.androidIronSourceSettings.enableInterstitial = true;
                    settings.androidIronSourceSettings.enableOfferwall = true;
                    settings.androidIronSourceSettings.enableRewardedVideo = true;
                }
                else
                {
                    settings.androidIronSourceSettings.enableBanner = false;
                    settings.androidIronSourceSettings.enableInterstitial = false;
                    settings.androidIronSourceSettings.enableOfferwall = false;
                    settings.androidIronSourceSettings.enableRewardedVideo = false;
                }

                settings.iosIronSourceSettings.appKey = ironSourceIOSAppKey;
                if (ironSourceIOSAppKey != "")
                {
                    settings.iosIronSourceSettings.enableBanner = true;
                    settings.iosIronSourceSettings.enableInterstitial = true;
                    settings.iosIronSourceSettings.enableOfferwall = true;
                    settings.iosIronSourceSettings.enableRewardedVideo = true;
                }
                else
                {
                    settings.iosIronSourceSettings.enableBanner = false;
                    settings.iosIronSourceSettings.enableInterstitial = false;
                    settings.iosIronSourceSettings.enableOfferwall = false;
                    settings.iosIronSourceSettings.enableRewardedVideo = false;
                }

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        private void ReconfigureJustTrackBuildSettings()
        {
            if (!Directory.Exists(Application.dataPath + "/JustTrack"))
            {
                Directory.CreateDirectory(Application.dataPath + "/JustTrack");
            }
            if (!Directory.Exists(Application.dataPath + "/JustTrack/Resources"))
            {
                Directory.CreateDirectory(Application.dataPath + "/JustTrack/Resources");
            }

            JustTrackBuildSettings settings;

            if (File.Exists(JUSTTRACK_BUILD_SETTINGS_PATH))
            {
                settings = AssetDatabase.LoadAssetAtPath<JustTrackBuildSettings>(JUSTTRACK_BUILD_SETTINGS_PATH);
            }
            else
            {
                settings = ScriptableObject.CreateInstance<JustTrackBuildSettings>();
                AssetDatabase.CreateAsset(settings, JUSTTRACK_BUILD_SETTINGS_PATH);
            }

            if (settings != null)
            {
                settings.allowValidationErrorsOnBuild = true;
                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void OnValidate()
        {
            ValidateAll();
        }

        public static void SetAdmobIDInAndroidManifest(string admobId)
        {
            const string manifestPath = "/Plugins/Android/AndroidManifest.xml";
            XNamespace AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
            XNamespace AndroidXmlToolsNamespace = "http://schemas.android.com/tools";
            XNamespace androidNs = AndroidXmlNamespace;
            XNamespace androidToolsNs = AndroidXmlToolsNamespace;


            if (File.Exists(Application.dataPath + manifestPath))
            {
                XmlDocument document = new XmlDocument();

                document.Load(Application.dataPath + manifestPath);

                var nsResolver = new XmlNamespaceManager(new NameTable());
                nsResolver.AddNamespace("android", androidNs.NamespaceName);

                XmlNode node = document.SelectSingleNode("/manifest/application/meta-data[@android:name='com.google.android.gms.ads.APPLICATION_ID']", nsResolver);
                if (node != null)
                {
                    var value = node.Attributes["android:value"];

                    if (admobId != "")
                    {
                        value.Value = admobId;
                    }
                    else
                    {
                        value.Value = "ca-app-pub-3940256099942544~3347511713"; //admob example
                    }
                }

                document.Save(Application.dataPath + manifestPath);
            }
        }
        
        // Updates facebook related essential keys (ApplicationId, ClientToken & ContentProvieder) in Android Manifest
        // based on entered data on SundaySDK
        public static void SetFacebookCredentialsInAndroidManifest(string appID, string clientToken)
        {
            const string manifestPath = "/Plugins/Android/AndroidManifest.xml";
            XNamespace AndroidXmlNamespace = "http://schemas.android.com/apk/res/android";
            XNamespace androidNs = AndroidXmlNamespace;
            
            if (File.Exists(Application.dataPath + manifestPath))
            {
                XmlDocument document = new XmlDocument();

                document.Load(Application.dataPath + manifestPath);

                var nsResolver = new XmlNamespaceManager(new NameTable());
                nsResolver.AddNamespace("android", androidNs.NamespaceName);

                // Facebook App ID
                XmlNode appIDNode = document.SelectSingleNode("/manifest/application/meta-data[@android:name='com.facebook.sdk.ApplicationId']", nsResolver);
                if (appIDNode != null)
                {
                    var value = appIDNode.Attributes["android:value"];

                    if (appID != "")
                    {
                        value.Value = "fb" + appID;
                    }
                }
                
                //Facebook Client Token
                XmlNode clientTokenNode = document.SelectSingleNode("/manifest/application/meta-data[@android:name='com.facebook.sdk.ClientToken']", nsResolver);
                if (clientTokenNode != null)
                {
                    var value = clientTokenNode.Attributes["android:value"];

                    if (clientToken != "")
                    {
                        value.Value = clientToken;
                    }
                }
                
                //Facebook Content Provider
                XmlNode contentProviderNode = document.SelectSingleNode("/manifest/application/provider[@android:name='com.facebook.FacebookContentProvider']", nsResolver);
                if (contentProviderNode != null)
                {
                    var value = contentProviderNode.Attributes["android:authorities"];
                    if (appID != "")
                    {
                        value.Value = "com.facebook.app.FacebookContentProvider" + appID;
                    }
                }
                document.Save(Application.dataPath + manifestPath);
            }
        }
        /*
        private void ChangeIronSourceSettings()
        {
            if (!Directory.Exists("Assets/IronSource/Resources"))
            {
                Directory.CreateDirectory("Assets/IronSource/Resources");
            }

            IronSourceMediationSettings mediationSettings;

            if (File.Exists(IRON_SOURCE_MEDIATION_SETTINGS_PATH))
            {
                mediationSettings = AssetDatabase.LoadAssetAtPath<IronSourceMediationSettings>(IRON_SOURCE_MEDIATION_SETTINGS_PATH);
            }
            else
            {
                mediationSettings = ScriptableObject.CreateInstance<IronSourceMediationSettings>();
                AssetDatabase.CreateAsset(mediationSettings, IRON_SOURCE_MEDIATION_SETTINGS_PATH);
            }

            if (mediationSettings != null)
            {
                //Add android
                if (!string.IsNullOrWhiteSpace(ironSourceAndroidAppKey))
                {
                    mediationSettings.AndroidAppKey = ironSourceAndroidAppKey;
                }

                //Add ios
                if (!string.IsNullOrWhiteSpace(ironSourceIOSAppKey))
                {
                    mediationSettings.IOSAppKey = ironSourceIOSAppKey;
                }

                mediationSettings.EnableIronsourceSDKInitAPI = false;
                mediationSettings.AddIronsourceSkadnetworkID = true;

                EditorUtility.SetDirty(mediationSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        */

#endif

    }
}
