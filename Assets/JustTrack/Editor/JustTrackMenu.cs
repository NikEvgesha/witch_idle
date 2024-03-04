using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JustTrack {

    public class JustTrackMenu {
        [MenuItem("justtrack/Show Settings", false, 100)]
        public static void ShowSettings() {
            SettingsService.OpenProjectSettings(JustTrackSettingsIMGUIRegister.settingsPath);
        }

        [MenuItem("justtrack/Validate Configuration", false, 101)]
        public static void ValidateConfiguration() {
            Debug.ClearDeveloperConsole();
            ClearConsole();

            var settings = JustTrackUtils.GetOrCreateSettings();
            if (settings == null) {
                Debug.LogError("justtrack SDK configuration was not correctly loaded");
                return;
            }

            JustTrackUtils.Validate(settings, (validateResult) => {
                foreach (string error in validateResult.errors) {
                    Debug.LogError(error);
                }
                foreach (string warning in validateResult.warnings) {
                    Debug.LogWarning(warning);
                }

                if (validateResult.errors.Count == 0) {
                    Debug.Log("justtrack configuration is valid");
                }
            });
        }


        [MenuItem("justtrack/Create SDK Instance", false, 102)]
        public static void CreateInstance() {
            var path = "Packages/io.justtrack.justtrack-unity-sdk/Prefabs/JustTrackSDK.prefab";
            var fallback = "Assets/JustTrack/Prefabs/JustTrackSDK.prefab";
            UnityEngine.Object prefab;

            if (File.Exists(path)) {
                prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            } else {
                prefab = AssetDatabase.LoadAssetAtPath(fallback, typeof(GameObject));
            }
            PrefabUtility.InstantiatePrefab(prefab);
        }

        [MenuItem("justtrack/Print Diagnostics", false, 103)]
        public static void PrintDiagnostics() {
            JustTrackSettings settings = JustTrackUtils.GetSettings();

            var commonInfoLines = new List<string>();
            var androidInfoLines = new List<string>();
            var iOSInfoLines = new List<string>();

            commonInfoLines.Add("Version: " + JustTrackSDK.getVersion());

            var generatedFiles = new List<string>();
            if (JustTrackCodeGenerator.HasIronSourceAdptImpl()) {
                generatedFiles.Add("IronSourceAdapterImpl.cs");
            }
            if (JustTrackCodeGenerator.HasFBAudienceNetworkAdpt()) {
                generatedFiles.Add("FacebookAudienceNetworkAdapter.cs");
            }

            commonInfoLines.Add("Generated Files: " + JoinElements(generatedFiles));

            var containSDKInFirstSceneLine = "";
            Scene firstScene = SceneManager.GetSceneByBuildIndex(0);
            if (firstScene != null) {
                containSDKInFirstSceneLine = "Main Scene: " + firstScene.name + " (Does not contain JustTrack SDK)";

                foreach (GameObject go in firstScene.GetRootGameObjects()) {
                    if (HasJustTrackSDK(go)) {
                        containSDKInFirstSceneLine = "Main Scene: " + firstScene.name + " (contains JustTrack SDK)";
                        break;
                    }
                }
            } else {
                containSDKInFirstSceneLine = "Main Scene: No scene found or loaded";
            }
            commonInfoLines.Add(containSDKInFirstSceneLine);

            var externalDepMangerVersions = new List<string>();
            foreach (string filePath in Directory.GetFiles("Assets/ExternalDependencyManager/Editor")) {
                var fileName = Path.GetFileNameWithoutExtension(filePath);
                if (Regex.IsMatch(fileName, @"^\d+\.\d+\.\d+$")) {
                    externalDepMangerVersions.Add(fileName);
                }

            }
            commonInfoLines.Add("External Dependency Manager: " + JoinElements(externalDepMangerVersions));

            androidInfoLines.Add("API Token: " + settings.androidApiToken);
            androidInfoLines.Add("Tracking Provider: " + settings.androidTrackingProvider);
            androidInfoLines.Add("Package ID: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android));
            androidInfoLines.Add("Scripting Backend: " + (JustTrackUtils.IsIL2CPP(true) ? "IL2CPP" : "Mono"));
            androidInfoLines.Add("Automatic Purchase Tracking: " + !settings.androidDisableAutomaticInAppPurchaseTracking);

            var androidIntegrations = new List<string>();
            if (settings.androidFirebaseSettings.enableAutomaticIntegration) {
                androidIntegrations.Add("Firebase");
            }
            if (settings.androidAdColonyIntegration) {
                androidIntegrations.Add("AdColony");
            }
            if (settings.androidAppLovinIntegration) {
                androidIntegrations.Add("AppLovin");
            }
            if (settings.androidChartboostIntegration) {
                androidIntegrations.Add("Chartboost");
            }
            if (settings.androidUnityAdsIntegration) {
                androidIntegrations.Add("Unity Ads");
            }
            androidInfoLines.Add("Integrations: " + JoinElements(androidIntegrations));
            AddIronSourceLines(androidInfoLines, settings.androidIronSourceSettings);

            iOSInfoLines.Add("API Token: " + settings.iosApiToken);
            iOSInfoLines.Add("Tracking Provider: " + settings.iosTrackingProvider);
            iOSInfoLines.Add("Package ID: " + PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS));
            iOSInfoLines.Add("Scripting Backend: " + (JustTrackUtils.IsIL2CPP(false) ? "IL2CPP" : "Mono"));
            iOSInfoLines.Add("Automatic Purchase Tracking: " + !settings.iosDisableAutomaticInAppPurchaseTracking);

            var iosIntegrations = new List<string>();
            if (settings.iosFirebaseSettings.enableAutomaticIntegration) {
                iosIntegrations.Add("Firebase");
            }
            if (settings.iosAdColonyIntegration) {
                iosIntegrations.Add("AdColony");
            }
            if (settings.iosAppLovinIntegration) {
                iosIntegrations.Add("AppLovin");
            }
            if (settings.iosChartboostIntegration) {
                iosIntegrations.Add("Chartboost");
            }
            if (settings.iosUnityAdsIntegration) {
                iosIntegrations.Add("Unity Ads");
            }
            iOSInfoLines.Add("Integrations: " + JoinElements(iosIntegrations));
            AddIronSourceLines(iOSInfoLines, settings.iosIronSourceSettings);

            var justtrackSdks = new List<string>();
            var nativeSdks = new List<string>();

            // Check external libs in Assets folder
            string[] sdkExtensions = new string[] { ".dll", ".jar", ".a", ".framework", ".aar" };
            foreach (string assetPath in AssetDatabase.GetAllAssetPaths()) {
                if (!ArrayUtility.Contains(sdkExtensions, Path.GetExtension(assetPath))) {
                    continue;
                }

                string fileName = Path.GetFileName(assetPath);
                Match match = Regex.Match(fileName, @"^(.*)-(\d+(\.\d+)*.*)$");
                string sdkName = fileName;
                string sdkVersion = "";

                if (match.Success) {
                    sdkName = match.Groups[1].Value;
                    sdkVersion = match.Groups[2].Value;
                }

                if (fileName.Contains("com.ironsource.sdk") ||
                    fileName.Contains("com.appsflyer.af-android-sdk") ||
                    fileName.Contains("firebase-installations") ||
                    fileName.Contains("com.unity3d.ads") ||
                    fileName.Contains("com.chartboost:chartboost-sdk") ||
                    fileName.Contains("com.chartboost.mediation") ||
                    fileName.Contains("com.applovin.applovin-sdk") ||
                    fileName.Contains("com.adcolony.sdk")) {
                    nativeSdks.Add(sdkName + ": " + sdkVersion);
                } else if (fileName.Contains("justtrack")) {
                    justtrackSdks.Add(sdkName +": " + sdkVersion + " (native)");
                }
            }

            nativeSdks.Sort();

            //check external libs by manifest file
            string manifestPath = Path.Combine(Application.dataPath, "../Packages/manifest.json");
            string manifestJson = File.ReadAllText(manifestPath);
            Dictionary<string, object> manifestData = MiniJson.Deserialize(manifestJson) as Dictionary<string, object>;
            Dictionary<string, object> dependencies = manifestData["dependencies"] as Dictionary<string, object>;
            var manifestSdks = new List<string>();
            foreach (KeyValuePair<string, object> dependency in dependencies) {
                var dependencyName = dependency.Key;
                if (dependencyName.Contains("com.ironsource.sdk") ||
                       dependencyName.Contains("com.appsflyer.af-android-sdk") ||
                       dependencyName.Contains("firebase-installations") ||
                       dependencyName.Contains("com.unity3d.ads") ||
                       dependencyName.Contains("com.chartboost:chartboost-sdk") ||
                       dependencyName.Contains("com.chartboost.mediation") ||
                       dependencyName.Contains("com.applovin.applovin-sdk") ||
                       dependencyName.Contains("com.adcolony.sdk")) {
                    manifestSdks.Add(dependencyName + ": " + dependency.Value);
                } else if (dependencyName.Contains("justtrack")) {
                    justtrackSdks.Add(dependencyName + ": " + dependency.Value + " (from manifest)");
                }
            }

            JustTrackUtils.Validate(settings, (validateResult) => {
                var verificationBlock = new List<string>();
                foreach (string error in validateResult.errors) {
                    verificationBlock.Add("Error: " + error);
                }
                foreach (string warning in validateResult.warnings) {
                    verificationBlock.Add("Warning: " + warning);
                }

                if (validateResult.errors.Count == 0) {
                    verificationBlock.Add("Verification Status: valid");
                } else {
                    verificationBlock.Add("Verification Status: invalid");
                }

                var report = CombineBlock("Common", commonInfoLines) +
                    CombineBlock("Native version", justtrackSdks) +
                    CombineBlock("Android", androidInfoLines) +
                    CombineBlock("iOS", iOSInfoLines) +
                    CombineBlock("Dependencies", manifestSdks) +
                    CombineBlock("Native Dependencies", nativeSdks) +
                    CombineBlock("Verification", verificationBlock);
                DisplayDiagnoseDialog(report);
            });
        }

        private static string JoinElements(List<string> elements) {
            if (elements.Count == 0) {
                return "-";
            }

            return String.Join(", ", elements.ToArray());
        }

        private static bool HasJustTrackSDK(GameObject gameObject) {
            return gameObject.GetComponentInChildren<JustTrackSDKBehaviour>() != null;
        }

        private static void AddIronSourceLines(List<string> lines, IronSourceSettings settings) {
            lines.Add("IronSource App Key: " + (String.IsNullOrEmpty(settings.appKey) ? "Missing" : "Set"));

            var adUnits = new List<string>();
            if (settings.enableBanner) {
                adUnits.Add("Banner");
            }
            if (settings.enableInterstitial) {
                adUnits.Add("Interstitial");
            }
            if (settings.enableRewardedVideo) {
                adUnits.Add("Rewarded-Video");
            }
            if (settings.enableOfferwall) {
                adUnits.Add("Offerwall");
            }
            lines.Add("IronSource Ad Units: " + JoinElements(adUnits));
        }

        private static string CombineBlock(string name, List<string> block) {
            if (block.Count == 0) {
                block.Add("- empty -");
            }

            return "\n### " + name + " ###\n\n" + String.Join("\n", block.ToArray()) + "\n";
        }

        private static void DisplayDiagnoseDialog(string dialogText) {
            int option = EditorUtility.DisplayDialogComplex("Integration Scan", dialogText, "OK", "Copy", null);
            if (option == 1) {
                GUIUtility.systemCopyBuffer = dialogText;
            }
        }

        [MenuItem("justtrack/Android Docs", false, 200)]
        public static void AndroidDocs() {
            Application.OpenURL("https://docs.justtrack.io/sdk/android-sdk-readme");
        }

        [MenuItem("justtrack/iOS Docs", false, 201)]
        public static void iOSDocs() {
            Application.OpenURL("https://docs.justtrack.io/sdk/ios-sdk-readme");
        }

        [MenuItem("justtrack/Unity Docs", false, 202)]
        public static void UnityDocs() {
            Application.OpenURL("https://docs.justtrack.io/sdk/unity-sdk-readme");
        }

        private static void ClearConsole() {
            try {
                var assembly = Assembly.GetAssembly(typeof(SceneView));
                var type = assembly.GetType("UnityEditor.LogEntries");
                var method = type.GetMethod("Clear");
                method.Invoke(new object(), null);
            } catch (Exception) {
                // well... then it seems like we don't clear the console today
            }
        }
    }
}
