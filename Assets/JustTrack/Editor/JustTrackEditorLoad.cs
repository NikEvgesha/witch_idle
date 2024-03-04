using System;
using UnityEditor;
using UnityEngine;

namespace JustTrack {
    class JustTrackEditorLoad {
        const string EDM_DIALOG_KEY = "io.justtrack.unity.edmDialogShown";
        const string FIREBASE_DIALOG_KEY = "io.justtrack.unity.firebaseDialogShown";
        private static bool androidUsesIL2CPP = false;
        private static bool iOSUsesIL2CPP = false;

        [InitializeOnLoadMethod]
        static void OnProjectLoadedInEditor() {
            androidUsesIL2CPP = JustTrackUtils.IsIL2CPP(true);
            iOSUsesIL2CPP = JustTrackUtils.IsIL2CPP(false);

            // ensure everything is loaded before we maybe work on a partially loaded asset database
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            // ensure settings are created and everything is initalized nicely
            var settings = JustTrackUtils.GetOrCreateSettings();
            if (settings != null) {
                ValidateInitialSettings(settings);
            } else {
                EditorApplication.update += ValidateInitialSettingsDelayed;
            }

            EditorApplication.update += Update;

            if (!JustTrackUtils.DetectExternalDependencyManager() && !SessionState.GetBool(EDM_DIALOG_KEY, false)) {
                SessionState.SetBool(EDM_DIALOG_KEY, true);
                var result = EditorUtility.DisplayDialogComplex(
                    "justtrack SDK",
                    "The External Dependency Manager for Unity is missing. This is a required dependency of the justtrack SDK. Please go to " + JustTrackUtils.EDM_DOWNLOAD_PAGE + " and download version 1.2.167 or later. You can also let the justtrack SDK import the newest version directly.",
                    "Import External Dependency Manager for Unity",
                    "Skip for now",
                    "Open download page"
                );
                switch (result) {
                    case 0:
                        CoroutineRuntime.StartCoroutine(JustTrackUtils.ImportPackageFromUrl(JustTrackUtils.EDM_PACKAGE_URL));
                        break;
                    case 1:
                        break;
                    case 2:
                        Application.OpenURL(JustTrackUtils.EDM_DOWNLOAD_PAGE);
                        break;
                }
            }
        }

        private static void ValidateInitialSettingsDelayed() {
            JustTrackSettings settings = JustTrackUtils.GetOrCreateSettings();
            if (settings == null) {
                return;
            }

            EditorApplication.update -= ValidateInitialSettingsDelayed;
            ValidateInitialSettings(settings);
        }

        private static void ValidateInitialSettings(JustTrackSettings settings) {
            // ask about firebase integration
            if (JustTrackUtils.DetectFirebase() && !SessionState.GetBool(FIREBASE_DIALOG_KEY, false) && !settings.ignoreFirebaseIntegration) {
                bool needsAndroid = !String.IsNullOrEmpty(settings.androidApiToken) && !settings.androidFirebaseSettings.enableAutomaticIntegration;
                bool needsIOS = !String.IsNullOrEmpty(settings.iosApiToken) && !settings.iosFirebaseSettings.enableAutomaticIntegration;
                if (needsAndroid || needsIOS) {
                    SessionState.SetBool(FIREBASE_DIALOG_KEY, true);
                    var result = EditorUtility.DisplayDialogComplex(
                        "justtrack SDK",
                        "The Firebase SDK was detected in your app, but the automatic integration with the justtrack SDK is not enabled. Do you want to enable it?",
                        "Enable Firebase Integration",
                        "Skip for now",
                        "Don't ask again"
                    );
                    switch (result) {
                        case 0:
                            JustTrackUtils.EnableFirebaseIntegration(needsAndroid, needsIOS);
                            break;
                        case 1:
                            break;
                        case 2:
                            JustTrackUtils.SetIgnoreFirebaseIntegration(true);
                            break;
                    }
                }
            }

            OnIL2CPPChanged(settings);

            JustTrackUtils.Validate(settings, (validateResult) => {
                foreach (string error in validateResult.errors) {
                    Debug.LogError(error);
                }
                foreach (string warning in validateResult.warnings) {
                    Debug.LogWarning(warning);
                }
            });
        }

        private static void Update() {
            JustTrackSettings settings = JustTrackUtils.GetSettings();
            if (settings == null) {
                return;
            }

            if ((androidUsesIL2CPP != JustTrackUtils.IsIL2CPP(true)) || (iOSUsesIL2CPP != JustTrackUtils.IsIL2CPP(false))) {
                androidUsesIL2CPP = JustTrackUtils.IsIL2CPP(true);
                iOSUsesIL2CPP = JustTrackUtils.IsIL2CPP(false);
                OnIL2CPPChanged(settings);
            }

            JustTrackUtils.ConfigurePreprocessorDefines(
                BuildTargetGroup.Android,
                AttributionProvider.Appsflyer == settings.androidTrackingProvider,
                !String.IsNullOrEmpty(settings.androidIronSourceSettings.appKey)
            );
            JustTrackUtils.ConfigurePreprocessorDefines(
                BuildTargetGroup.iOS,
                AttributionProvider.Appsflyer == settings.iosTrackingProvider,
                !String.IsNullOrEmpty(settings.iosIronSourceSettings.appKey)
            );
        }

        private static void OnIL2CPPChanged(JustTrackSettings settings) {
            JustTrackUtils.OnUnityLoaded(settings);
        }
    }
}
