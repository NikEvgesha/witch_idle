using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System;
using System.Collections;
using System.Collections.Generic;

namespace JustTrack {
    public class ValidationPreProcessor : IPreprocessBuildWithReport {
        public int callbackOrder { get { return 0; } }
        public void OnPreprocessBuild(BuildReport report) {
            JustTrackUtils.ValidationResult validateResult;
            validateResult.warnings = new List<string>();
            validateResult.errors = new List<string>();
            var settings = JustTrackUtils.GetSettings();
            if (settings == null) {
                throw new BuildFailedException("justtrack SDK configuration was not found");
            }
            JustTrackUtils.ValidationMode mode = JustTrackUtils.ValidationMode.ValidateAll;
            var buildTarget = report.summary.platform;
            if (buildTarget == BuildTarget.Android) {
                mode = JustTrackUtils.ValidationMode.ValidateAndroid;
            } else if (buildTarget == BuildTarget.iOS) {
                mode = JustTrackUtils.ValidationMode.ValidateIOS;
            }
            var steps = JustTrackUtils.ValidateAsync(settings, mode, (result) => {
                validateResult = result;
            });
            while (steps.MoveNext()) {
                // wait
            }

            foreach (string error in validateResult.errors) {
                Debug.LogError(error);
            }
            foreach (string warning in validateResult.warnings) {
                Debug.LogWarning(warning);
            }

            if (validateResult.errors.Count > 0 && !JustTrackUtils.AreValidationErrorsAllowedOnBuild()) {
                throw new BuildFailedException("justtrack SDK configuration is not valid");
            }
        }
    }
}
