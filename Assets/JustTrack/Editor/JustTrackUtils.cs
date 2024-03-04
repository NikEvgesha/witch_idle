using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace JustTrack {
    internal class JustTrackUtils {
        internal const string EDM_DOWNLOAD_PAGE = "https://developers.google.com/unity/archive#external_dependency_manager_for_unity";
        internal const string EDM_PACKAGE_URL = "https://github.com/googlesamples/unity-jar-resolver/raw/v1.2.176/external-dependency-manager-1.2.176.unitypackage";

        internal static JustTrackSettings GetSettings() {
            return AssetDatabase.LoadAssetAtPath<JustTrackSettings>(JustTrackSettings.JustTrackSettingsPath);
        }

        internal static bool AreValidationErrorsAllowedOnBuild() {
            var settings = AssetDatabase.LoadAssetAtPath<JustTrackBuildSettings>(JustTrackBuildSettings.JustTrackBuildSettingsPath);

            return settings != null && settings.allowValidationErrorsOnBuild;
        }

        internal static JustTrackSettings GetOrCreateSettings() {
            var settings = GetSettings();
            if (settings == null) {
                if (File.Exists(JustTrackSettings.JustTrackSettingsPath)) {
                    return null;
                }
                settings = ScriptableObject.CreateInstance<JustTrackSettings>();
                settings.androidApiToken = "";
                settings.iosApiToken = "";
                settings.androidTrackingProvider = AttributionProvider.justtrack;
                settings.iosTrackingProvider = AttributionProvider.justtrack;
                PersistSettings(settings);

                return settings;
            }

            if (settings.androidTrackingProvider != AttributionProvider.Appsflyer && settings.androidTrackingProvider != AttributionProvider.justtrack) {
                SerializedObject serializedObject = new SerializedObject(settings);
                SerializedProperty androidTrackingProvider = serializedObject.FindProperty("androidTrackingProvider");
                var attributionProviderValues = Enum.GetValues(AttributionProvider.justtrack.GetType());
                androidTrackingProvider.enumValueIndex = Array.IndexOf(attributionProviderValues, AttributionProvider.justtrack);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

            if (settings.iosTrackingProvider != AttributionProvider.Appsflyer && settings.iosTrackingProvider != AttributionProvider.justtrack) {
                SerializedObject serializedObject = new SerializedObject(settings);
                SerializedProperty iosTrackingProvider = serializedObject.FindProperty("iosTrackingProvider");
                var attributionProviderValues = Enum.GetValues(AttributionProvider.justtrack.GetType());
                iosTrackingProvider.enumValueIndex = Array.IndexOf(attributionProviderValues, AttributionProvider.justtrack);
                serializedObject.ApplyModifiedProperties();
                AssetDatabase.SaveAssets();
            }

            return settings;
        }

        private static void PersistSettings(JustTrackSettings settings) {
            Directory.CreateDirectory(JustTrackSettings.JustTrackSettingsDirectory);

            AssetDatabase.CreateAsset(settings, JustTrackSettings.JustTrackSettingsPath);
            AssetDatabase.SaveAssets();
        }

        internal static SerializedObject GetSerializedSettings() {
            var settings = GetOrCreateSettings();
            if (settings == null) {
                return null;
            }

            return new SerializedObject(settings);
        }

        internal static void SetAlwaysUpdateInjectedCode(bool value) {
            var serializedObject = GetSerializedSettings();
            if (serializedObject == null) {
                Debug.LogError("no justtrack settings could be loaded");
                return;
            }
            SerializedProperty alwaysUpdateInjectedCode = serializedObject.FindProperty("alwaysUpdateInjectedCode");
            alwaysUpdateInjectedCode.boolValue = value;
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        internal static void SetIgnoreFirebaseIntegration(bool value) {
            var serializedObject = GetSerializedSettings();
            if (serializedObject == null) {
                Debug.LogError("no justtrack settings could be loaded");
                return;
            }
            SerializedProperty ignoreFirebaseIntegration = serializedObject.FindProperty("ignoreFirebaseIntegration");
            ignoreFirebaseIntegration.boolValue = value;
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        internal static void EnableFirebaseIntegration(bool enableAndroidIntegration, bool enableIOSIntegration) {
            var serializedObject = GetSerializedSettings();
            if (serializedObject == null) {
                Debug.LogError("no justtrack settings could be loaded");
                return;
            }
            SerializedProperty ignoreFirebaseIntegration = serializedObject.FindProperty("ignoreFirebaseIntegration");
            SerializedProperty androidFirebaseSettings = serializedObject.FindProperty("androidFirebaseSettings");
            SerializedProperty iosFirebaseSettings = serializedObject.FindProperty("iosFirebaseSettings");
            SerializedProperty androidFirebaseEnableAutomaticIntegration = androidFirebaseSettings.FindPropertyRelative("enableAutomaticIntegration");
            SerializedProperty iosFirebaseEnableAutomaticIntegration = iosFirebaseSettings.FindPropertyRelative("enableAutomaticIntegration");
            ignoreFirebaseIntegration.boolValue = false;
            if (enableAndroidIntegration) {
                androidFirebaseEnableAutomaticIntegration.boolValue = true;
            }
            if (enableIOSIntegration) {
                iosFirebaseEnableAutomaticIntegration.boolValue = true;
            }
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        internal struct ValidationResult {
            internal List<string> warnings;
            internal List<string> errors;
        }

        internal enum ValidationMode {
            ValidateAndroid,
            ValidateIOS,
            ValidateAll,
        }

        internal static void Validate(JustTrackSettings settings, Action<ValidationResult> callback) {
            CoroutineRuntime.StartCoroutine(ValidateAsync(settings, ValidationMode.ValidateAll, callback));
        }

        internal static IEnumerator ValidateAsync(JustTrackSettings settings, ValidationMode mode, Action<ValidationResult> callback) {
            ValidationResult result;
            result.warnings = new List<string>();
            result.errors = new List<string>();

            var androidErrors = mode == ValidationMode.ValidateIOS ? result.warnings : result.errors;
            var iosErrors = mode == ValidationMode.ValidateAndroid ? result.warnings : result.errors;

            foreach (var step in CoroutineRuntime.ToEnumerable(ValidateToken(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android), settings.androidApiToken, "Android", result.warnings, androidErrors))) {
                yield return step;
            }
            foreach (var step in CoroutineRuntime.ToEnumerable(ValidateToken(PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS), settings.iosApiToken, "iOS", result.warnings, iosErrors))) {
                yield return step;
            }

            if (String.IsNullOrEmpty(settings.androidApiToken) && String.IsNullOrEmpty(settings.iosApiToken)) {
                result.errors.Add("Neither Android nor iOS API tokens are configured. You need to configure an API token for each platform to use the justtrack SDK");
            }

            if (!String.IsNullOrEmpty(settings.androidApiToken)) {
                switch (settings.androidTrackingProvider) {
                    case AttributionProvider.Appsflyer:
                        ValidateAppsflyerDetected("Android", androidErrors);
                        break;
                    case AttributionProvider.justtrack:
                        break;
                    default:
                        androidErrors.Add("Invalid attribution provider configured on Android");
                        break;
                }
            }

            if (!String.IsNullOrEmpty(settings.iosApiToken)) {
                switch (settings.iosTrackingProvider) {
                    case AttributionProvider.Appsflyer:
                        ValidateAppsflyerDetected("iOS", iosErrors);
                        break;
                    case AttributionProvider.justtrack:
                        break;
                    default:
                        iosErrors.Add("Invalid attribution provider configured on iOS");
                        break;
                }
            }

            ValidateTrackingSettings(!String.IsNullOrEmpty(settings.iosApiToken), settings.iosTrackingSettings, result.warnings, iosErrors);

            ValidateIronSourceSettings(!String.IsNullOrEmpty(settings.androidApiToken), "Android", settings.androidIronSourceSettings, androidErrors);
            ValidateIronSourceSettings(!String.IsNullOrEmpty(settings.iosApiToken), "iOS", settings.iosIronSourceSettings, iosErrors);

            ValidateFirebaseSettings(!String.IsNullOrEmpty(settings.androidApiToken), "Android", settings.ignoreFirebaseIntegration, settings.androidFirebaseSettings, androidErrors);
            ValidateFirebaseSettings(!String.IsNullOrEmpty(settings.iosApiToken), "iOS", settings.ignoreFirebaseIntegration, settings.iosFirebaseSettings, iosErrors);

            if (!DetectExternalDependencyManager()) {
                result.errors.Add("The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + EDM_DOWNLOAD_PAGE + " and download version 1.2.167 or later.");
            }

            callback(result);
            yield break;
        }

        private static IEnumerator ValidateToken(string packageId, string token, string platform, List<string> warnings, List<string> errors) {
            if (String.IsNullOrEmpty(token)) {
                yield break;
            }

            var parts = token.Split('-');
            if (parts.Length != 2) {
                errors.Add(platform + " API token does not consist of two parts, expected something of the form 'prod-tokendata' or 'sandbox-tokendata'");
            } else if (parts[0] != "prod" && parts[0] != "sandbox") {
                errors.Add(platform + " API token starts with unknown environment '" + parts[0] + "', expected 'prod' or 'sandbox'");
            } else if (parts[1].Length != 64) {
                errors.Add(platform + " API token has " + parts[1].Length + " instead of 64 characters of token data");
            }

            foreach (var step in CoroutineRuntime.ToEnumerable(DoVerifyApiToken(packageId, token, (success, valid) => {
                if (!success) {
                    warnings.Add(platform + " API token could not be verified. Are you offline?");
                } else if (!valid) {
                    errors.Add(platform + " API token is not valid. Please follow the instructions at https://docs.justtrack.io/sdk/unity-sdk-readme#getting-an-api-token to obtain a valid Api token");
                }
            }))) {
                yield return step;
            }
        }

        private static void ValidateAppsflyerDetected(string platform, List<string> errors) {
            if (JustTrackUtils.DetectAppsflyer()) {
                return;
            }

            errors.Add("The justtrack SDK is configured to use appsflyer as tracking id provider on " + platform + ", but it was not found in your game");
        }

        private static void ValidateTrackingSettings(bool activeOnIOS, iOSTrackingSettings settings, List<string> warnings, List<string> errors) {
            if (!activeOnIOS && settings.requestTrackingPermission) {
                errors.Add("You configured automatically requesting App Tracking Transparency opt-in, but there is no API token configured for iOS");
            } else if (settings.requestTrackingPermission && String.IsNullOrEmpty(settings.trackingPermissionDescription) && !settings.trackingPermissionDescriptionManaged) {
                warnings.Add("You configured automatically requesting App Tracking Transparency opt-in, but there isn't a description configured. You have to set NSUserTrackingUsageDescription yourself.");
            } else if (!settings.requestTrackingPermission && !String.IsNullOrEmpty(settings.trackingPermissionDescription)) {
                warnings.Add("You have not configured automatically requesting App Tracking Transparency opt-in, but there is a description configured. This will overwrite NSUserTrackingUsageDescription.");
            }

            switch (settings.facebookAudienceNetworkIntegration) {
                case FacebookAudienceNetworkIntegration.NoIntegration:
                    if (settings.requestTrackingPermission && Reflection.HasFacebookAudienceNetworkUnity()) {
                        errors.Add("You have not enabled the automatic integration with the Unity version of the Facebook Audience Network, but it was deteted in your game.");
                    }
                    break;
                case FacebookAudienceNetworkIntegration.UnityIntegration:
                    if (!settings.requestTrackingPermission) {
                        errors.Add("The automatic integration with the Unity version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.");
                    } else if (!Reflection.HasFacebookAudienceNetworkUnity()) {
                        errors.Add("The automatic integration with the Unity version of the Facebook Audience Network might not work. No assembly was found containing the code of the FAN.");
                    }
                    break;
                case FacebookAudienceNetworkIntegration.NativeIntegration:
                    if (!settings.requestTrackingPermission) {
                        errors.Add("The automatic integration with the native version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.");
                    } else if (Reflection.HasFacebookAudienceNetworkUnity()) {
                        errors.Add("You have chosen to integrate with the native version of the Facebook Audience Network, but the Unity version of it was detected.");
                    }
                    break;
            }
        }

        private static void ValidateIronSourceSettings(bool activeOnPlatform, string platform, IronSourceSettings settings, List<string> errors) {
            if (String.IsNullOrEmpty(settings.appKey)) {
                if (settings.enableBanner) {
                    errors.Add("IronSource will not be managed by the justtrack SDK on platform " + platform + ", but is configured to load banners");
                }
                if (settings.enableInterstitial) {
                    errors.Add("IronSource will not be managed by the justtrack SDK on platform " + platform + ", but is configured to load interstitials");
                }
                if (settings.enableRewardedVideo) {
                    errors.Add("IronSource will not be managed by the justtrack SDK on platform " + platform + ", but is configured to load rewarded videos");
                }
                if (settings.enableOfferwall) {
                    errors.Add("IronSource will not be managed by the justtrack SDK on platform " + platform + ", but is configured to load the offerwall");
                }
            } else if (!activeOnPlatform) {
                errors.Add("There is no API token configured for the justtrack SDK on platform " + platform + ", but IronSource is configured to be loaded by the SDK");
            } else {
                ValidateIronSourceDetected(platform, errors);
                if (!settings.enableBanner && !settings.enableInterstitial && !settings.enableRewardedVideo && !settings.enableOfferwall) {
                    errors.Add("IronSource will be loaded without any banners, interstitials, rewarded videos, or the offerwall on platform " + platform + " as no ad unit was configured to be loaded");
                }
            }
        }

        private static void ValidateIronSourceDetected(string platform, List<string> errors) {
            if (JustTrackUtils.DetectIronSource()) {
                return;
            }

            errors.Add("The justtrack SDK is configured to integrate with ironSource on " + platform + ", but it was not found in your game");
        }

        private static void ValidateFirebaseSettings(bool activeOnPlatform, string platform, bool ignoreFirebaseIntegration, FirebaseSettings firebaseSettings, List<string> errors) {
            if (!activeOnPlatform) {
                if (firebaseSettings.enableAutomaticIntegration) {
                    errors.Add("There is no API token configured for the justtrack SDK on platform " + platform + ", but the Firebase SDK integration is enabled.");
                }
            } else if (DetectFirebase()) {
                if (!firebaseSettings.enableAutomaticIntegration && !ignoreFirebaseIntegration) {
                    errors.Add("The Firebase SDK was detected on platform " + platform + ", but the automatic Firebase SDK configuration is disabled.");
                }
            } else {
                if (firebaseSettings.enableAutomaticIntegration) {
                    errors.Add("No Firebase SDK was detected, but the Firebase SDK integration is enabled on " + platform + ".");
                }
            }
        }

        internal static void OnUnityLoaded(JustTrackSettings settings) {
            RunCodeGenerator(settings, JustTrackUtils.NeedsIronSourceIntegrationCode(settings), JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings), false);
        }

        internal static void RunCodeGenerator(JustTrackSettings settings, bool integrateWithIronSource, FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration, bool force) {
            new JustTrackCodeGenerator(integrateWithIronSource, facebookAudienceNetworkIntegration).Run(settings, force);
        }

        internal static bool DetectAppsflyer() {
            try {
                return AssetDatabase.FindAssets("t:Prefab AppsFlyerObject").Length > 0 && Reflection.GetAppsflyerAssembly() != null;
            } catch (Exception) {
                return false;
            }
        }

        internal static bool DetectIronSource() {
            return Reflection.GetIronSourceAssembly() != null;
        }

        internal static bool DetectFirebase() {
            return DetectFirebaseLibraryAsset("com.google.firebase.firebase-analytics", ".aar")
                || DetectFirebaseLibraryAsset("libFirebaseCppAnalytics", ".a");
        }

        private static bool DetectFirebaseLibraryAsset(string assetName, string extension) {
            string[] guids = AssetDatabase.FindAssets(assetName);

            foreach (string guid in guids) {
                if (AssetDatabase.GUIDToAssetPath(guid).EndsWith(extension)) {
                    return true;
                }
            }

            return false;
        }

        internal static void ConfigurePreprocessorDefines(BuildTargetGroup targetGroup, bool trackingProviderIsAppsflyer, bool enableIronsource) {
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            string newDefines = GeneratePreprocessorDefines(defines, trackingProviderIsAppsflyer, enableIronsource, Enum.GetName(typeof(BuildTargetGroup), targetGroup));
            if (newDefines != defines) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newDefines);
                try {
                    // Sometimes the defines are not updated directly... we have to press ctrl+s first. Avoid that need by doing it automatically.
                    EditorApplication.ExecuteMenuItem("File/Save Project");
                } catch (Exception) {
                    Debug.LogError("Could not save project automatically. Please save project by hand to ensure preprocessor defines are up to date.");
                }
            }
        }

        private static string GeneratePreprocessorDefines(string currentDefines, bool trackingProviderIsAppsflyer, bool enableIronsource, string platform) {
            return BuildPreprocessorDefines(currentDefines, platform, new Dictionary<string, bool> {
                ["JUSTTRACK_APPSFLYER_INTEGRATION"] = trackingProviderIsAppsflyer,
                ["JUSTTRACK_IRONSOURCE_INTEGRATION"] = enableIronsource,
                // keep them for now to ensure we drop those defines again, remove with 4.5.0 or so
                ["JUSTTRACK_DEVELOPMENT_USE_PROD_ENDPOINT"] = false,
                ["JUSTTRACK_DEVELOPMENT_USE_SANDBOX_ENDPOINT"] = false,
                ["JUSTTRACK_RELEASE_USE_PROD_ENDPOINT"] = false,
                ["JUSTTRACK_RELEASE_USE_SANDBOX_ENDPOINT"] = false,
            });
        }

        private static string BuildPreprocessorDefines(string currentDefines, string platform, Dictionary<string, bool> wantedDefines) {
            List<string> defines = new List<string>();
            foreach (var define in currentDefines.Split(';')) {
                if (wantedDefines.ContainsKey(define)) {
                    // define managed by us
                    if (wantedDefines[define]) {
                        // we want to keep it
                        defines.Add(define);
                    } else {
                        Debug.Log("Dropping " + define + " preprocessor symbol for platform " + platform);
                    }
                    // remove it in any case, afterwards we don't need to iterate over the defines again
                    wantedDefines.Remove(define);
                } else {
                    // unknown define, keep it as it is
                    defines.Add(define);
                }
            }
            foreach (var wantedDefine in wantedDefines) {
                if (wantedDefine.Value) {
                    // we wanted to have this, but didn't find it yet, so add it
                    defines.Add(wantedDefine.Key);
                    Debug.Log("Adding " + wantedDefine.Key + " preprocessor symbol for platform " + platform);
                }
            }
            return String.Join(";", defines.ToArray());
        }

        internal static bool NeedsIronSourceIntegrationCode(JustTrackSettings settings) {
            bool hasAndroid = !String.IsNullOrEmpty(settings.androidApiToken) && !String.IsNullOrEmpty(settings.androidIronSourceSettings.appKey);
            bool hasIOS = !String.IsNullOrEmpty(settings.iosApiToken) && !String.IsNullOrEmpty(settings.iosIronSourceSettings.appKey);

            return (hasAndroid && IsIL2CPP(true)) || (hasIOS && IsIL2CPP(false));
        }

        internal static FacebookAudienceNetworkIntegration GetFacebookAudienceNetworkIntegration(JustTrackSettings settings) {
            if (String.IsNullOrEmpty(settings.iosApiToken) || !settings.iosTrackingSettings.requestTrackingPermission) {
                return FacebookAudienceNetworkIntegration.NoIntegration;
            }

            return settings.iosTrackingSettings.facebookAudienceNetworkIntegration;
        }

        internal static bool IsIL2CPP(bool isAndroid) {
            var backend = PlayerSettings.GetScriptingBackend(isAndroid ? BuildTargetGroup.Android : BuildTargetGroup.iOS);
            return backend == ScriptingImplementation.IL2CPP;
        }

        internal static bool DetectExternalDependencyManager() {
            var edmAssemblies = new List<string>(new string[] {
                "Google.PackageManagerResolver",
                "Google.VersionHandler",
                "Google.JarResolver",
                "Google.IOSResolver",
                "Google.VersionHandlerImpl",
            });
            try {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies) {
                    if (edmAssemblies.Contains(assembly.GetName().Name)) {
                        return true;
                    }
                }
                return false;
            } catch (System.Exception) {
                return false;
            }
        }

        internal static IEnumerator ImportPackageFromUrl(string url) {
            UnityWebRequest request = UnityWebRequest.Get(url);
            yield return request.SendWebRequest();

            while (!request.isDone || request.responseCode == -1) {
                yield return null; // what we yield is ignored anyway
            }

            if (request.responseCode < 200 || request.responseCode > 299) {
                Debug.LogError("Failed to download url: " + request.error + " with status " + request.responseCode);
                yield break;
            }
            string tempFile = Path.GetTempFileName();
            File.WriteAllBytes(tempFile, request.downloadHandler.data);
            AssetDatabase.ImportPackage(tempFile, true);
            // sadly we can't clean up the temp file as importing the EDM will cause a recompile of the scripts, causing this
            // routine to be unloaded before we can do anything else...
        }

        internal static Dictionary<string, VerificationResult> verificationResults = new Dictionary<string, VerificationResult>();

        internal static VerificationResult VerifyApiToken(string packageId, string apiToken, Action repaint) {
            if (String.IsNullOrEmpty(packageId) || String.IsNullOrEmpty(apiToken)) {
                return VerificationResult.MISSING;
            }

            var key = packageId + "\u2063" + apiToken;
            lock (verificationResults) {
                if (verificationResults.ContainsKey(key)) {
                    return verificationResults[key];
                }

                verificationResults[key] = VerificationResult.PENDING;
            }

            CoroutineRuntime.StartCoroutine(DoVerifyApiToken(packageId, apiToken, (success, valid) => {
                lock (verificationResults) {
                    verificationResults[key] = success ? valid ? VerificationResult.VALID : VerificationResult.INVALID : VerificationResult.UNKNOWN;
                }
                repaint();
            }));

            return VerificationResult.PENDING;
        }

        private static IEnumerator DoVerifyApiToken(string packageId, string apiToken, Action<bool, bool> callback) {
            string url, token;
            if (apiToken.StartsWith("prod-")) {
                url = "https://ipv4.justtrack.io/v0/sign";
                token = apiToken.Substring("prod-".Length);
            } else if (apiToken.StartsWith("sandbox-")) {
                url = "https://ipv4.marketing-sandbox.info/v0/sign";
                token = apiToken.Substring("sandbox-".Length);
            } else {
                callback(true, false);
                yield break;
            }

            if (apiToken.Contains(" ")) {
                callback(true, false);
                yield break;
            }

            var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("X-CLIENT-ID", packageId);
            request.SetRequestHeader("X-CLIENT-TOKEN", token);
            yield return request.SendWebRequest();

            while (!request.isDone || request.responseCode == -1) {
                yield return null; // what we yield is ignored anyway
            }

            var success = request.result == UnityWebRequest.Result.Success || request.result == UnityWebRequest.Result.ProtocolError;
            var valid = request.responseCode >= 200 && request.responseCode <= 299 && request.result == UnityWebRequest.Result.Success;
            callback(success, valid);
        }
    }
}
