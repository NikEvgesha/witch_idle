#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

namespace JustTrack {
    public static class SwiftPostProcessor {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string buildPath) {
            if (buildTarget == BuildTarget.iOS) {
                // We need to construct our own PBX project path that corrently refers to the Bridging header
                // var projPath = PBXProject.GetPBXProjectPath(buildPath);
                var projPath = buildPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
                var proj = new PBXProject();
                proj.ReadFromFile(projPath);

                disableBitCodeForTarget(proj, proj.GetUnityMainTargetGuid());
                disableBitCodeForTarget(proj, proj.TargetGuidByName(PBXProject.GetUnityTestTargetName()));
                disableBitCodeForTarget(proj, proj.GetUnityFrameworkTargetGuid());

                proj.WriteToFile(projPath);
            }
        }

        private static void disableBitCodeForTarget(PBXProject proj, string targetGuid) {
            proj.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
            foreach (var configName in proj.BuildConfigNames()) {
                var configGuid = proj.BuildConfigByName(targetGuid, configName);
                proj.SetBuildPropertyForConfig(configGuid, "ENABLE_BITCODE", "NO");
            }
        }
    }
}
#endif