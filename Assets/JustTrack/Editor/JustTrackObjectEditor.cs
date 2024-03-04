using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JustTrack {
    [CustomEditor(typeof(JustTrackSDKBehaviour))]
    public class JustTrackObjectEditor : Editor {
        bool justTrackFoldout = true;
        int selectedApiTokenPlatform = 0;
        bool attFoldout = true;

        bool integrationsFoldout = true;

        bool ironsourceFoldout = true;
        int selectedIronsourcePlatform = 0;

        bool firebaseFoldout = true;
        int selectedFirebasePlatform = 0;

        public override void OnInspectorGUI() {
            try {
                var settings = JustTrackUtils.GetSerializedSettings();
                if (settings == null) {
                    EditorGUILayout.HelpBox("No justtrack settings could be loaded.", MessageType.Error);
                    return;
                }
                RenderGUI(settings, ref justTrackFoldout, ref selectedApiTokenPlatform, ref attFoldout, ref integrationsFoldout, ref ironsourceFoldout, ref selectedIronsourcePlatform, ref firebaseFoldout, ref selectedFirebasePlatform, () => {
                    Repaint();
                });
            } catch (Exception e) {
                EditorGUILayout.HelpBox("Failed to render settings: " + e.Message + "\n" + e.StackTrace, MessageType.Error);
            }
        }

        internal static void RenderGUI(SerializedObject serializedObject, ref bool justTrackFoldout, ref int selectedApiTokenPlatform, ref bool attFoldout, ref bool integrationsFoldout, ref bool ironsourceFoldout, ref int selectedIronsourcePlatform, ref bool firebaseFoldout, ref int selectedFirebasePlatform, Action repaint) {
            SerializedProperty androidApiToken = serializedObject.FindProperty("androidApiToken");
            SerializedProperty iosApiToken = serializedObject.FindProperty("iosApiToken");
            SerializedProperty androidAttributionProvider = serializedObject.FindProperty("androidTrackingProvider"); // yes, they were called tracking provider in the past...
            SerializedProperty iosAttributionProvider = serializedObject.FindProperty("iosTrackingProvider");
            SerializedProperty androidIronSourceSettings = serializedObject.FindProperty("androidIronSourceSettings");
            SerializedProperty iosIronSourceSettings = serializedObject.FindProperty("iosIronSourceSettings");
            SerializedProperty iosTrackingSettings = serializedObject.FindProperty("iosTrackingSettings");
            SerializedProperty androidFirebaseSettings = serializedObject.FindProperty("androidFirebaseSettings");
            SerializedProperty iosFirebaseSettings = serializedObject.FindProperty("iosFirebaseSettings");
            SerializedProperty androidDisableAutomaticInAppPurchaseTracking = serializedObject.FindProperty("androidDisableAutomaticInAppPurchaseTracking");
            SerializedProperty iosDisableAutomaticInAppPurchaseTracking = serializedObject.FindProperty("iosDisableAutomaticInAppPurchaseTracking");

            SerializedProperty androidAdColonyIntegration = serializedObject.FindProperty("androidAdColonyIntegration");
            SerializedProperty iosAdColonyIntegration = serializedObject.FindProperty("iosAdColonyIntegration");
            SerializedProperty androidAppLovinIntegration = serializedObject.FindProperty("androidAppLovinIntegration");
            SerializedProperty iosAppLovinIntegration = serializedObject.FindProperty("iosAppLovinIntegration");
            SerializedProperty androidChartboostIntegration = serializedObject.FindProperty("androidChartboostIntegration");
            SerializedProperty iosChartboostIntegration = serializedObject.FindProperty("iosChartboostIntegration");
            SerializedProperty androidUnityAdsIntegration = serializedObject.FindProperty("androidUnityAdsIntegration");
            SerializedProperty iosUnityAdsIntegration = serializedObject.FindProperty("iosUnityAdsIntegration");

            SerializedProperty androidIronsourceAppKey = androidIronSourceSettings.FindPropertyRelative("appKey");
            SerializedProperty androidIronsourceEnableBanner = androidIronSourceSettings.FindPropertyRelative("enableBanner");
            SerializedProperty androidIronsourceEnableInterstitial = androidIronSourceSettings.FindPropertyRelative("enableInterstitial");
            SerializedProperty androidIronsourceEnableRewardedVideo = androidIronSourceSettings.FindPropertyRelative("enableRewardedVideo");
            SerializedProperty androidIronsourceEnableOfferwall = androidIronSourceSettings.FindPropertyRelative("enableOfferwall");
            SerializedProperty iosIronsourceAppKey = iosIronSourceSettings.FindPropertyRelative("appKey");
            SerializedProperty iosIronsourceEnableBanner = iosIronSourceSettings.FindPropertyRelative("enableBanner");
            SerializedProperty iosIronsourceEnableInterstitial = iosIronSourceSettings.FindPropertyRelative("enableInterstitial");
            SerializedProperty iosIronsourceEnableRewardedVideo = iosIronSourceSettings.FindPropertyRelative("enableRewardedVideo");
            SerializedProperty iosIronsourceEnableOfferwall = iosIronSourceSettings.FindPropertyRelative("enableOfferwall");

            SerializedProperty iosTrackingSettingsRequestTrackingPermission = iosTrackingSettings.FindPropertyRelative("requestTrackingPermission");
            SerializedProperty iosTrackingSettingsTrackingPermissionDescription = iosTrackingSettings.FindPropertyRelative("trackingPermissionDescription");
            SerializedProperty iosTrackingSettingsTrackingPermissionDescriptionManaged = iosTrackingSettings.FindPropertyRelative("trackingPermissionDescriptionManaged");
            SerializedProperty iosTrackingSettingsFacebookAudienceNetworkIntegration = iosTrackingSettings.FindPropertyRelative("facebookAudienceNetworkIntegration");

            SerializedProperty androidFirebaseEnableAutomaticIntegration = androidFirebaseSettings.FindPropertyRelative("enableAutomaticIntegration");
            SerializedProperty iosFirebaseEnableAutomaticIntegration = iosFirebaseSettings.FindPropertyRelative("enableAutomaticIntegration");

            serializedObject.Update();

            using (var check = new EditorGUI.ChangeCheckScope()) {
                string[] platformNames = {"Android", "iOS"};
                var attributionProviderValues = Enum.GetValues(AttributionProvider.justtrack.GetType());
                var facebookAudienceNetworkIntegrationValues = Enum.GetValues(FacebookAudienceNetworkIntegration.NoIntegration.GetType());

                ////////////
                // HEADER //
                ////////////

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Box("logo.png", 512, 128);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                ///////////////////////////////////////
                // EXTERNAL DEPENDENCY MANAGER CHECK //
                ///////////////////////////////////////

                if (!JustTrackUtils.DetectExternalDependencyManager()) {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + JustTrackUtils.EDM_DOWNLOAD_PAGE + " and download version 1.2.167 or later.", MessageType.Error);
                    // Manual download button
                    GUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Open download page", new GUILayoutOption[] { GUILayout.Width(150) })) {
                        Application.OpenURL(JustTrackUtils.EDM_DOWNLOAD_PAGE);
                    }
                    // Automatic download button
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Import External Dependency Manager for Unity", new GUILayoutOption[] { GUILayout.Width(300) })) {
                        CoroutineRuntime.StartCoroutine(JustTrackUtils.ImportPackageFromUrl(JustTrackUtils.EDM_PACKAGE_URL));
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                ///////////////
                // API TOKEN //
                ///////////////

                bool hasAndroid = !String.IsNullOrEmpty(androidApiToken.stringValue);
                bool hasIOS = !String.IsNullOrEmpty(iosApiToken.stringValue);

                justTrackFoldout = EditorGUILayout.Foldout(justTrackFoldout, new GUIContent("justtrack SDK Settings"), true);

                if (justTrackFoldout) {
                    EditorGUILayout.Space();
                    selectedApiTokenPlatform = GUILayout.Toolbar(selectedApiTokenPlatform, platformNames);

                    string packageId = "";
                    switch (selectedApiTokenPlatform) {
                        case 0:
                            packageId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
                            break;
                        case 1:
                            packageId = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.iOS);
                            break;
                    }
                    EditorGUILayout.LabelField(platformNames[selectedApiTokenPlatform] + " Package Id:", packageId);

                    EditorGUILayout.HelpBox("You have to configure the correct API token for each platform you want to build for. An API token should be of the form 'prod-...64 characters...'.", MessageType.Info);
                    string apiToken = "";
                    switch (selectedApiTokenPlatform) {
                        case 0:
                            EditorGUILayout.PropertyField(androidApiToken, new GUIContent("Android Api Token"));
                            apiToken = androidApiToken.stringValue;
                            break;
                        case 1:
                            EditorGUILayout.PropertyField(iosApiToken, new GUIContent("iOS Api Token"));
                            apiToken = iosApiToken.stringValue;
                            break;
                    }

                    switch (JustTrackUtils.VerifyApiToken(packageId, apiToken, repaint)) {
                        case VerificationResult.VALID:
                            EditorGUILayout.HelpBox("Your Api token is valid.", MessageType.None);
                            break;
                        case VerificationResult.INVALID:
                            EditorGUILayout.HelpBox("Your Api token is invalid. Please follow the instructions at https://docs.justtrack.io/sdk/unity-sdk-readme#getting-an-api-token to obtain a valid Api token.", MessageType.Error);
                            GUILayout.BeginHorizontal();
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("Open documentation", new GUILayoutOption[] { GUILayout.Width(150) })) {
                                Application.OpenURL("https://docs.justtrack.io/sdk/unity-sdk-readme#getting-an-api-token");
                            }
                            GUILayout.FlexibleSpace();
                            GUILayout.EndHorizontal();
                            break;
                        case VerificationResult.PENDING:
                            EditorGUILayout.HelpBox("Verification of your Api token is still pending. This should only take a few seconds...", MessageType.None);
                            break;
                        case VerificationResult.UNKNOWN:
                            EditorGUILayout.HelpBox("Verification of your Api token could not be performed. Are you offline?", MessageType.Warning);
                            break;
                        case VerificationResult.MISSING:
                            EditorGUILayout.HelpBox("You did not configure an Api token and can't use the justtrack SDK on " + platformNames[selectedApiTokenPlatform] + ".", MessageType.Warning);
                            break;
                    }
                    EditorGUILayout.Space();

                    ///////////////////////
                    // TRACKING PROVIDER //
                    ///////////////////////

                    switch (selectedApiTokenPlatform) {
                        case 0: {
                            using (new EditorGUI.DisabledScope(!hasAndroid)) {
                                SerializedProperty attributionProviderProperty = androidAttributionProvider;
                                AttributionProvider attributionProvider = (AttributionProvider) EditorGUILayout.EnumPopup("Attribution provider", (AttributionProvider) attributionProviderValues.GetValue(attributionProviderProperty.enumValueIndex));
                                int attributionProviderIndex = Array.IndexOf(attributionProviderValues, attributionProvider);
                                attributionProviderProperty.enumValueIndex = attributionProviderIndex;
                                if (attributionProvider == AttributionProvider.Appsflyer && !JustTrackUtils.DetectAppsflyer()) {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.HelpBox("No Appsflyer SDK was detected, but it is set to be used as attribution provider.", MessageType.Warning);
                                }
                            }
                            break;
                        }
                        case 1: {
                            using (new EditorGUI.DisabledScope(!hasIOS)) {
                                SerializedProperty attributionProviderProperty = iosAttributionProvider;
                                AttributionProvider attributionProvider = (AttributionProvider) EditorGUILayout.EnumPopup("Attribution provider", (AttributionProvider) attributionProviderValues.GetValue(attributionProviderProperty.enumValueIndex));
                                int attributionProviderIndex = Array.IndexOf(attributionProviderValues, attributionProvider);
                                attributionProviderProperty.enumValueIndex = attributionProviderIndex;
                                if (attributionProvider == AttributionProvider.Appsflyer && !JustTrackUtils.DetectAppsflyer()) {
                                    EditorGUILayout.Space();
                                    EditorGUILayout.HelpBox("No Appsflyer SDK was detected, but it is set to be used as attribution provider.", MessageType.Warning);
                                }
                                EditorGUILayout.Space();

                                using (new EditorGUI.IndentLevelScope()) {
                                    attFoldout = EditorGUILayout.Foldout(attFoldout, new GUIContent("App Tracking Transparency"), true);
                                    if (attFoldout) {
                                        EditorGUILayout.HelpBox("Here you can configure the justtrack SDK to automatically ask the user to opt in to Apple's App Tracking Transparency framework.", MessageType.Info);
                                        EditorGUILayout.PropertyField(iosTrackingSettingsRequestTrackingPermission, new GUIContent("Request Tracking Permission"));

                                        using (new EditorGUI.DisabledScope(String.IsNullOrEmpty(iosTrackingSettingsTrackingPermissionDescription.stringValue) && iosTrackingSettingsTrackingPermissionDescriptionManaged.boolValue)) {
                                            EditorGUILayout.HelpBox("If you request permission to track the user, you have to provide the NSUserTrackingUsageDescription string. You can configure a string here to automatically configure that setting for you.", MessageType.Info);
                                            EditorGUILayout.PropertyField(iosTrackingSettingsTrackingPermissionDescription, new GUIContent("Tracking Permission Description"));
                                        }

                                        using (new EditorGUI.DisabledScope(!String.IsNullOrEmpty(iosTrackingSettingsTrackingPermissionDescription.stringValue) || !iosTrackingSettingsRequestTrackingPermission.boolValue)) {
                                            EditorGUILayout.HelpBox("If you are setting NSUserTrackingUsageDescription yourself, set the below checkbox to silence the warning about it being empty.", MessageType.Info);
                                            EditorGUILayout.PropertyField(iosTrackingSettingsTrackingPermissionDescriptionManaged, new GUIContent("Ignore Empty Tracking Description"));
                                        }

                                        using (new EditorGUI.DisabledScope(!iosTrackingSettingsRequestTrackingPermission.boolValue)) {
                                            EditorGUILayout.HelpBox("If you are using the Facebook Audience Network, the justtrack SDK can provide the permission of the advertiser id automatically to it after requesting it. This will also ensure the permission was provided (if given) before the IronSource SDK is initialized by the justtrack SDK (if configured).", MessageType.Info);
                                            FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration = (FacebookAudienceNetworkIntegration) EditorGUILayout.EnumPopup("FAN Integration", (FacebookAudienceNetworkIntegration) facebookAudienceNetworkIntegrationValues.GetValue(iosTrackingSettingsFacebookAudienceNetworkIntegration.enumValueIndex));
                                            int facebookAudienceNetworkIntegrationIndex = Array.IndexOf(facebookAudienceNetworkIntegrationValues, facebookAudienceNetworkIntegration);
                                            iosTrackingSettingsFacebookAudienceNetworkIntegration.enumValueIndex = facebookAudienceNetworkIntegrationIndex;

                                            switch (facebookAudienceNetworkIntegration) {
                                            case FacebookAudienceNetworkIntegration.NoIntegration:
                                                if (iosTrackingSettingsRequestTrackingPermission.boolValue && Reflection.HasFacebookAudienceNetworkUnity()) {
                                                    EditorGUILayout.HelpBox("You have not enabled the automatic integration with the Unity version of the Facebook Audience Network, but it was deteted in your game.", MessageType.Warning);
                                                }
                                                break;
                                            case FacebookAudienceNetworkIntegration.UnityIntegration:
                                                if (!iosTrackingSettingsRequestTrackingPermission.boolValue) {
                                                    EditorGUILayout.HelpBox("The automatic integration with the Unity version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.", MessageType.Warning);
                                                } else if (!Reflection.HasFacebookAudienceNetworkUnity()) {
                                                    EditorGUILayout.HelpBox("The automatic integration with the Unity version of the Facebook Audience Network might not work. No assembly was found containing the code of the FAN.", MessageType.Warning);
                                                }
                                                break;
                                            case FacebookAudienceNetworkIntegration.NativeIntegration:
                                                if (!iosTrackingSettingsRequestTrackingPermission.boolValue) {
                                                    EditorGUILayout.HelpBox("The automatic integration with the native version of the Facebook Audience Network will not be performed. Enable requesting the tracking permission for it to be performed.", MessageType.Warning);
                                                } else if (Reflection.HasFacebookAudienceNetworkUnity()) {
                                                    EditorGUILayout.HelpBox("You have chosen to integrate with the native version of the Facebook Audience Network, but the Unity version of it was detected.", MessageType.Warning);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }

                            break;
                        }
                    }

                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("The justtrack SDK automatically tracks the revenue your users generate with in-app product and subscription purchases. Here you can configure whether the tracking should be active by default:", MessageType.Info);

                    bool enableAutomaticInAppPurchaseTracking = false;
                    SerializedProperty? disableAutomaticInAppPurchaseTracking = null;
                    EditorGUI.BeginChangeCheck();
                    switch (selectedApiTokenPlatform) {
                        case 0:
                            enableAutomaticInAppPurchaseTracking = EditorGUILayout.Toggle("Android Automatic IAP tracking", !androidDisableAutomaticInAppPurchaseTracking.boolValue);
                            disableAutomaticInAppPurchaseTracking = androidDisableAutomaticInAppPurchaseTracking;
                            break;
                        case 1:
                            enableAutomaticInAppPurchaseTracking = EditorGUILayout.Toggle("iOS Automatic IAP tracking", !iosDisableAutomaticInAppPurchaseTracking.boolValue);
                            disableAutomaticInAppPurchaseTracking = iosDisableAutomaticInAppPurchaseTracking;
                            break;
                    }
                    if (EditorGUI.EndChangeCheck() && disableAutomaticInAppPurchaseTracking != null) {
                        disableAutomaticInAppPurchaseTracking.boolValue = !enableAutomaticInAppPurchaseTracking;
                    }
                }

                ///////////////////
                // DOCUMENTATION //
                ///////////////////

                EditorGUILayout.Space();
                EditorGUILayout.HelpBox("For more information on setting up the justtrack SDK check out the relevant docs.", MessageType.None);
                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack Android Docs", new GUILayoutOption[] { GUILayout.Width(150) })) {
                    Application.OpenURL("https://docs.justtrack.io/sdk/android-sdk-readme");
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack iOS Docs", new GUILayoutOption[] { GUILayout.Width(150) })) {
                    Application.OpenURL("https://docs.justtrack.io/sdk/ios-sdk-readme");
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("justtrack Unity Docs", new GUILayoutOption[] { GUILayout.Width(150) })) {
                    Application.OpenURL("https://docs.justtrack.io/sdk/unity-sdk-readme");
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                ////////////////
                // IRONSOURCE //
                ////////////////

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Box("ironsrc_logo.png", 512, 128);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (JustTrackUtils.DetectIronSource()) {
                    if (String.IsNullOrEmpty(androidIronsourceAppKey.stringValue) && hasAndroid) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("IronSource SDK was detected, but no app key was set for Android.", MessageType.Warning);
                    }
                    if (String.IsNullOrEmpty(iosIronsourceAppKey.stringValue) && hasIOS) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("IronSource SDK was detected, but no app key was set for iOS.", MessageType.Warning);
                    }

                    var settings = (JustTrackSettings) serializedObject.targetObject;
                    if (JustTrackUtils.NeedsIronSourceIntegrationCode(settings) && Reflection.GetIronSourceAdapter() == null) {
                        EditorGUILayout.HelpBox("No justtrack IronSource Adapter was found in your assembly. You can generate one to ensure you can successfully integrate the justtrack SDK with the IronSource SDK at runtime.", MessageType.Error);

                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Generate IronSource Adapter", new GUILayoutOption[] { GUILayout.Width(200) })) {
                            JustTrackUtils.RunCodeGenerator(settings, true, JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings), true);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                } else {
                    if (!String.IsNullOrEmpty(androidIronsourceAppKey.stringValue)) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No IronSource SDK was detected, but an app key was set for Android.", MessageType.Warning);
                    }
                    if (!String.IsNullOrEmpty(iosIronsourceAppKey.stringValue)) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No IronSource SDK was detected, but an app key was set for iOS.", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();
                ironsourceFoldout = EditorGUILayout.Foldout(ironsourceFoldout, new GUIContent("justtrack IronSource Integration"), true);
                if (ironsourceFoldout) {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("This component will automatically integrate the justtrack SDK with the IronSource SDK and initialize the latter as soon as the integration was successful. Fill in the app key and all the ad units you want to enable per platform here. If you don't want to use IronSource, leave the fields blank.", MessageType.Info);
                    EditorGUILayout.Space();
                    selectedIronsourcePlatform = GUILayout.Toolbar(selectedIronsourcePlatform, platformNames);

                    EditorGUILayout.HelpBox("Fill in your IronSource app key here.", MessageType.Info);
                    switch (selectedIronsourcePlatform) {
                        case 0:
                            using (new EditorGUI.DisabledScope(!hasAndroid)) {
                                EditorGUILayout.PropertyField(androidIronsourceAppKey, new GUIContent("Android App Key"));
                                using (new EditorGUI.DisabledScope(String.IsNullOrEmpty(androidIronsourceAppKey.stringValue))) {
                                    EditorGUILayout.HelpBox("Enable IronSource Ad Units", MessageType.None);
                                    EditorGUILayout.PropertyField(androidIronsourceEnableBanner, new GUIContent("Banner"));
                                    EditorGUILayout.PropertyField(androidIronsourceEnableInterstitial, new GUIContent("Interstitial"));
                                    EditorGUILayout.PropertyField(androidIronsourceEnableRewardedVideo, new GUIContent("Rewarded Video"));
                                    EditorGUILayout.PropertyField(androidIronsourceEnableOfferwall, new GUIContent("Offerwall"));
                                    if (!String.IsNullOrEmpty(androidIronsourceAppKey.stringValue)) {
                                        var anyEnabled = androidIronsourceEnableBanner.boolValue || androidIronsourceEnableInterstitial.boolValue || androidIronsourceEnableRewardedVideo.boolValue || androidIronsourceEnableOfferwall.boolValue;
                                        if (!anyEnabled) {
                                            EditorGUILayout.Space();
                                            EditorGUILayout.HelpBox("No IronSource ad units are enabled, but an app key was set.", MessageType.Warning);
                                        }
                                    }
                                }
                            }
                            break;
                        case 1:
                            using (new EditorGUI.DisabledScope(!hasIOS)) {
                                EditorGUILayout.PropertyField(iosIronsourceAppKey, new GUIContent("iOS App Key"));
                                using (new EditorGUI.DisabledScope(String.IsNullOrEmpty(iosIronsourceAppKey.stringValue))) {
                                    EditorGUILayout.HelpBox("Enable IronSource Ad Units", MessageType.None);
                                    EditorGUILayout.PropertyField(iosIronsourceEnableBanner, new GUIContent("Banner"));
                                    EditorGUILayout.PropertyField(iosIronsourceEnableInterstitial, new GUIContent("Interstitial"));
                                    EditorGUILayout.PropertyField(iosIronsourceEnableRewardedVideo, new GUIContent("Rewarded Video"));
                                    EditorGUILayout.PropertyField(iosIronsourceEnableOfferwall, new GUIContent("Offerwall"));
                                    if (!String.IsNullOrEmpty(iosIronsourceAppKey.stringValue)) {
                                        var anyEnabled = iosIronsourceEnableBanner.boolValue || iosIronsourceEnableInterstitial.boolValue || iosIronsourceEnableRewardedVideo.boolValue || iosIronsourceEnableOfferwall.boolValue;
                                        if (!anyEnabled) {
                                            EditorGUILayout.Space();
                                            EditorGUILayout.HelpBox("No IronSource ad units are enabled, but an app key was set.", MessageType.Warning);
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }

                //////////////
                // FIREBASE //
                //////////////

                EditorGUILayout.Space();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                Box("firebase_logo.png", 512, 128);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if (JustTrackUtils.DetectFirebase()) {
                    bool needsAndroid = false;
                    bool needsIOS = false;
                    if (!androidFirebaseEnableAutomaticIntegration.boolValue && hasAndroid) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Firebase SDK was detected, but the automatic integration for Android is disabled.", MessageType.Warning);
                        needsAndroid = true;
                    }
                    if (!iosFirebaseEnableAutomaticIntegration.boolValue && hasIOS) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("Firebase SDK was detected, but the automatic integration for iOS is disabled.", MessageType.Warning);
                        needsIOS = true;
                    }
                    if (needsAndroid || needsIOS) {
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        if (GUILayout.Button("Enable Firebase Integration", new GUILayoutOption[] { GUILayout.Width(200) })) {
                            JustTrackUtils.EnableFirebaseIntegration(needsAndroid, needsIOS);
                        }
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                } else {
                    if (androidFirebaseEnableAutomaticIntegration.boolValue) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No Firebase SDK was detected, but the automatic integration for Android is enabled.", MessageType.Warning);
                    }
                    if (iosFirebaseEnableAutomaticIntegration.boolValue) {
                        EditorGUILayout.Space();
                        EditorGUILayout.HelpBox("No Firebase SDK was detected, but the automatic integration for iOS is enabled.", MessageType.Warning);
                    }
                }

                EditorGUILayout.Space();
                firebaseFoldout = EditorGUILayout.Foldout(firebaseFoldout, new GUIContent("justtrack Firebase Integration"), true);
                if (firebaseFoldout) {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("This component will automatically integrate the justtrack SDK with the Firebase SDK and automatically forward the Firebase app instance id of the user to the justtrack backend.", MessageType.Info);
                    EditorGUILayout.Space();
                    selectedFirebasePlatform = GUILayout.Toolbar(selectedFirebasePlatform, platformNames);

                    switch (selectedFirebasePlatform) {
                        case 0:
                            using (new EditorGUI.DisabledScope(!hasAndroid)) {
                                EditorGUILayout.PropertyField(androidFirebaseEnableAutomaticIntegration, new GUIContent("Enable Firebase Integration"));
                            }
                            break;
                        case 1:
                            using (new EditorGUI.DisabledScope(!hasIOS)) {
                                EditorGUILayout.PropertyField(iosFirebaseEnableAutomaticIntegration, new GUIContent("Enable Firebase Integration"));
                            }
                            break;
                    }
                }

                //////////////////
                // INTEGRATIONS //
                //////////////////

                EditorGUILayout.Space();
                integrationsFoldout = EditorGUILayout.Foldout(integrationsFoldout, new GUIContent("justtrack Ad Network Integrations"), true);
                if (integrationsFoldout) {
                    EditorGUILayout.Space();
                    EditorGUILayout.HelpBox("You can configure the justtrack SDK to automatically integrate with the SDKs of different ad networks. Doing so will automatically forward ad impressions to the justtrack backend for these networks.\n\nYou don't have to enable these integrations if you are only using these networks via a mediation provider like IronSource.", MessageType.Info);
                    EditorGUILayout.Space();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(androidAdColonyIntegration, new GUIContent("Enable AdColony Android Integration"));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(iosAdColonyIntegration, new GUIContent("Enable AdColony iOS Integration"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(androidAppLovinIntegration, new GUIContent("Enable AppLovin Android Integration"));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(iosAppLovinIntegration, new GUIContent("Enable AppLovin iOS Integration"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(androidChartboostIntegration, new GUIContent("Enable Chartboost Android Integration"));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(iosChartboostIntegration, new GUIContent("Enable Chartboost iOS Integration"));
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(androidUnityAdsIntegration, new GUIContent("Enable Unity Ads Android Integration"));
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.PropertyField(iosUnityAdsIntegration, new GUIContent("Enable Unity Ads iOS Integration"));
                    GUILayout.EndHorizontal();
                }

                serializedObject.ApplyModifiedProperties();

                if (check.changed) {
                    var settings = (JustTrackSettings) serializedObject.targetObject;
                    JustTrackUtils.RunCodeGenerator(settings, JustTrackUtils.NeedsIronSourceIntegrationCode(settings), JustTrackUtils.GetFacebookAudienceNetworkIntegration(settings), false);
                    AssetDatabase.SaveAssets();
                }
            }
        }

        internal static void Box(string image, int width, int height) {
            var path = "Packages/io.justtrack.justtrack-unity-sdk/Editor/" + image;
            var fallback = "Assets/JustTrack/Editor/" + image;

            if (File.Exists(path)) {
                GUILayout.Box((Texture) AssetDatabase.LoadAssetAtPath(path, typeof(Texture)), new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) });
            } else {
                GUILayout.Box((Texture) AssetDatabase.LoadAssetAtPath(fallback, typeof(Texture)), new GUILayoutOption[] { GUILayout.Width(width), GUILayout.Height(height) });
            }
        }
    }
}