using System;
using UnityEngine;

namespace JustTrack {
    public class JustTrackBuildSettings : ScriptableObject {
        public const string JustTrackBuildSettingsDirectory = JustTrackSettings.JustTrackSettingsDirectory;
        public const string JustTrackBuildSettingsResource = "JustTrackBuildSettings";
        public const string JustTrackBuildSettingsPath = JustTrackBuildSettingsDirectory + "/" + JustTrackBuildSettingsResource + ".asset";

        [SerializeField]
        public bool allowValidationErrorsOnBuild;
    }
}
