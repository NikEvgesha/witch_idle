using System;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace JustTrack {
    internal class JustTrackCodeGenerator {
        private const string INJECTED_CODE_DIRECTORY = "Assets/JustTrack/Injected";
        private bool integrateWithIronSource;
        private FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration;

        internal JustTrackCodeGenerator(bool integrateWithIronSource_, FacebookAudienceNetworkIntegration facebookAudienceNetworkIntegration_) {
            integrateWithIronSource = integrateWithIronSource_;
            facebookAudienceNetworkIntegration = facebookAudienceNetworkIntegration_;
        }

        internal void Run(JustTrackSettings settings, bool force) {
            if (!NeedsInjectedCode()) {
                if (force || ShouldRemoveInjectedCode(settings)) {
                    RemoveInjectedCode();
                }
            } else {
                if (force || Directory.Exists(INJECTED_CODE_DIRECTORY) || ShouldInjectCode(settings)) {
                    GenerateInjectedCode();
                }
            }
        }

        const string IRONSOURCE_ADPT_IMPL_FILE = "/IronSourceAdapterImpl.cs";
        const string FB_AUDIENCE_NETWORK_ADPT_FILE = "/FacebookAudienceNetworkAdapter.cs";

        const string REMOVE_KEY_ASKED = "io.justtrack.unity.shouldRemoveInjectedCode.asked";
        const string REMOVE_KEY_RESULT = "io.justtrack.unity.shouldRemoveInjectedCode.result";
        const string INJECT_KEY_ASKED = "io.justtrack.unity.shouldInjectCode.asked";
        const string INJECT_KEY_RESULT = "io.justtrack.unity.shouldInjectCode.result";

        internal static bool HasIronSourceAdptImpl() {
            var path = INJECTED_CODE_DIRECTORY + IRONSOURCE_ADPT_IMPL_FILE;
            return File.Exists(path);
        }

        internal static bool HasFBAudienceNetworkAdpt() {
            var path = INJECTED_CODE_DIRECTORY + FB_AUDIENCE_NETWORK_ADPT_FILE;
            return File.Exists(path);
        }

        private bool ShouldRemoveInjectedCode(JustTrackSettings settings) {
            if (!Directory.Exists(INJECTED_CODE_DIRECTORY)) {
                // nothing to remove, no need to remove anything
                return false;
            }
            if (settings.alwaysUpdateInjectedCode) {
                return true;
            }
            if (SessionState.GetBool(REMOVE_KEY_ASKED, false)) {
                return SessionState.GetBool(REMOVE_KEY_RESULT, false);
            }
            SessionState.SetBool(REMOVE_KEY_ASKED, true);
            var result = EditorUtility.DisplayDialogComplex(
                "justtrack SDK",
                "The code injected by the justtrack SDK into your game (located in " + INJECTED_CODE_DIRECTORY + ") is no longer needed. Do you want to remove it?",
                "Remove injected code",
                "Keep injected code",
                "Always update injected code automatically"
            );
            switch (result) {
                case 2:
                    // always ok
                    JustTrackUtils.SetAlwaysUpdateInjectedCode(true);
                    // fall through to handling the ok case
                    goto case 0;
                case 0:
                    // ok
                    // reset the state of the other message to be able to ask again
                    SessionState.SetBool(REMOVE_KEY_RESULT, true);
                    SessionState.SetBool(INJECT_KEY_ASKED, false);
                    return true;
                case 1:
                default:
                    // cancel
                    SessionState.SetBool(REMOVE_KEY_RESULT, false);
                    return false;

            }
        }

        private bool ShouldInjectCode(JustTrackSettings settings) {
            if (settings.alwaysUpdateInjectedCode) {
                return true;
            }
            if (SessionState.GetBool(INJECT_KEY_ASKED, false)) {
                return SessionState.GetBool(INJECT_KEY_RESULT, false);
            }
            SessionState.SetBool(INJECT_KEY_ASKED, true);
            var result = EditorUtility.DisplayDialogComplex(
                "justtrack SDK",
                "The justtrack SDK needs to add some code to your game to help with the integration of some of the SDKs (like the IronSource SDK). It will be placed in " + INJECTED_CODE_DIRECTORY + " and needs to be compiled in the default assembly (Assembly-CSharp.dll).",
                "Create or update the injected code",
                "Don't add code to my project",
                "Always update injected code automatically"
            );
            switch (result) {
                case 2:
                    // always ok
                    JustTrackUtils.SetAlwaysUpdateInjectedCode(true);
                    // fall through to handling the ok case
                    goto case 0;
                case 0:
                    // ok
                    // reset the state of the other message to be able to ask again
                    SessionState.SetBool(INJECT_KEY_RESULT, true);
                    SessionState.SetBool(REMOVE_KEY_ASKED, false);
                    return true;
                case 1:
                default:
                    // cancel
                    SessionState.SetBool(INJECT_KEY_RESULT, false);
                    return false;
            }
        }

        private bool NeedsInjectedCode() {
            return integrateWithIronSource || facebookAudienceNetworkIntegration != FacebookAudienceNetworkIntegration.NoIntegration;
        }

        private static void RemoveInjectedCode() {
            if (!Directory.Exists(INJECTED_CODE_DIRECTORY)) {
                return;
            }
            Directory.Delete(INJECTED_CODE_DIRECTORY, true);
            File.Delete(INJECTED_CODE_DIRECTORY + ".meta");
            if (Directory.GetFiles("Assets/JustTrack").Length == 0) {
                Directory.Delete("Assets/JustTrack", true);
                File.Delete("Assets/JustTrack.meta");
            }
            AssetDatabase.Refresh();
        }

        private void GenerateInjectedCode() {
            Directory.CreateDirectory(INJECTED_CODE_DIRECTORY);
            bool needsRefresh = false;
            var readmeFile = INJECTED_CODE_DIRECTORY + "/ReadMe.md";
            var ironSourceAdapterFile = INJECTED_CODE_DIRECTORY + IRONSOURCE_ADPT_IMPL_FILE;
            var facebookAudienceNetworkAdapterFile = INJECTED_CODE_DIRECTORY + FB_AUDIENCE_NETWORK_ADPT_FILE;

            if (!File.Exists(readmeFile) || File.ReadAllText(readmeFile) != GetReadmeString()) {
                needsRefresh = true;
                File.WriteAllText(readmeFile, GetReadmeString());
            }

            if (integrateWithIronSource) {
                if (!File.Exists(ironSourceAdapterFile) || File.ReadAllText(ironSourceAdapterFile) != GetIronSourceAdapterString()) {
                    needsRefresh = true;
                    File.WriteAllText(ironSourceAdapterFile, GetIronSourceAdapterString());
                }
            } else if (File.Exists(ironSourceAdapterFile)) {
                needsRefresh = true;
                File.Delete(ironSourceAdapterFile);
                File.Delete(ironSourceAdapterFile + ".meta");
            }

            if (facebookAudienceNetworkIntegration != FacebookAudienceNetworkIntegration.NoIntegration) {
                bool unityIntegration = facebookAudienceNetworkIntegration == FacebookAudienceNetworkIntegration.UnityIntegration;
                if (!File.Exists(facebookAudienceNetworkAdapterFile) || File.ReadAllText(facebookAudienceNetworkAdapterFile) != GetFacebookAudienceNetworkAdapterString(unityIntegration)) {
                    needsRefresh = true;
                    File.WriteAllText(facebookAudienceNetworkAdapterFile, GetFacebookAudienceNetworkAdapterString(unityIntegration));
                }
            } else if (File.Exists(facebookAudienceNetworkAdapterFile)) {
                needsRefresh = true;
                File.Delete(facebookAudienceNetworkAdapterFile);
                File.Delete(facebookAudienceNetworkAdapterFile + ".meta");
            }

            if (needsRefresh) {
                AssetDatabase.Refresh();
            }
        }

        private static string GetReadmeString() {
            return @"# justtrack SDK - Injected Code

This folder contains code managed by the justtrack SDK.

## Why is this needed?

To make it easy to integrate and update the justtrack SDK it is provieded as a Unity package.
However, this means that the C# code of the justtrack SDK can't access code from the default
assembly (Assembly-CSharp.dll). This is normally a good thing and can speed up builds by some
bit, but it also comes with some drawbacks. Mainly, some SDKs like the IronSource SDK are
provided as a .unitypackage by default and therefore end up in the default assembly if no futher
changes are made to them by a developer. This means, we can only access them via reflection at
runtime.

For the most part, using reflection is sufficient to perform the needed tasks. However, sometimes
we need to generate a small amount of code at runtime and this might not be supported if you are
using the IL2CPP backend instead of the Mono backend for greater speed. Thus, we need to add some
small bridging code to the default assembly to avoid having to generate that code at runtime.

## Can I edit these files?

There should be no need to change any of these files and your changes might be overwritten at any
time during futher development by the code which generated them in the first place. Instead, if
you need to make some changes, please tell us why you need to change them, what needs to be
changed and we will find a solution fitting you and improving the justtrack SDK for everyone
else, too. You can contact us at <mailto:contact@justtrack.io> or <https://justtrack.io/contact/>.

## Should I add these files to version control?

You should add the files in this folder to version control. This will make it simpler along your
team to work with the justtrack SDK as not every developer has to make the decision whether to
actually create the generated code in the first place. It is also needed if you are building your
game in some CI pipeline to ensure a consistent version of your game is produced every time.
";
        }

        private static string GetIronSourceAdapterString() {
            return MakeInjectedFile("JUSTTRACK_IRONSOURCE_INTEGRATION", "", @"public class IronSourceAdapterImpl: IronSourceAdapter {
        [Preserve]
        public void SetIronSourceOnImpressionHandler(Action<object> proxy) {
            IronSourceEvents.onImpressionSuccessEvent += (eventObj) => proxy(eventObj);
        }
    }");
        }

        private static string GetFacebookAudienceNetworkAdapterString(bool unityIntegration) {
            if (unityIntegration) {
                return MakeInjectedFile(null, "", @"public class FacebookAudienceNetworkAdapter {
        #if UNITY_IOS
        [Preserve]
        public static void SetAdvertiserTrackingEnabled(bool enabled) {
            AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(enabled);
        }
        #endif
    }");
            }

            return MakeInjectedFile(null, @"
using System.Runtime.InteropServices;", @"public class FacebookAudienceNetworkAdapter {
        #if UNITY_IOS
        [DllImport(""__Internal"")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);

        [Preserve]
        public static void SetAdvertiserTrackingEnabled(bool enabled) {
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(enabled);
        }
        #endif
    }");
        }

        private static string MakeInjectedFile(string define, string imports, string contents) {
            return (define != null ? "#if " + define + "\n" : "") + @"using System;
using JustTrack;" + imports + @"
using UnityEngine.Scripting;

// DO NOT EDIT - AUTOMATICALLY GENERATED BY THE JUSTTRACK SDK

namespace JustTrackInjected {
    [Preserve]
    " + contents + @"
}
" + (define != null ? "\n#endif" : "");
        }
    }
}
