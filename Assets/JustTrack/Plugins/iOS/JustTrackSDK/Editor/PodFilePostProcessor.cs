#if UNITY_IOS
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;

namespace JustTrack {
    public static class PostProcessIOS {
        // must be between 45 and 50 to ensure that it's not overriden by Podfile generation (40),
        // Ad Colony modifying the Podfile (45) and that it's added before "pod install" (50)
        [PostProcessBuildAttribute(49)]
        private static void PostProcessBuild_iOS(BuildTarget target, string buildPath) {
            if (target == BuildTarget.iOS) {
                JustTrackSettings settings = JustTrackUtils.GetSettings();
                if (settings == null) {
                    throw new BuildFailedException("justtrack SDK configuration was not found");
                }

                var pods = new List<string>();
                if (settings.enableExperimentalPodFilePostProcessor) {
                    ProcessPodFile(buildPath + "/Podfile");
                }

            }
        }

        private static void ProcessPodFile(string podFilePath) {
            var lines = File.ReadAllLines(podFilePath);
            var newLines = new List<string>();
            var skipping = false;

            foreach (var line in lines) {
                if (line.Contains("use_frameworks!")) {
                    newLines.Add("use_frameworks!");
                } else if (line.Contains("target 'Unity-iPhone' do")) {
                    skipping = true;
                } else if (skipping) {
                    skipping = !line.Contains("end");
                } else {
                    if (line.Contains("target 'UnityFramework' do")) {
                        newLines.Add("target 'Unity-iPhone' do");
                        newLines.Add("end");
                    }
                    newLines.Add(line);
                }
            }

            newLines.Add("def remove_duplicated_frameworks(app_pod_name, installer)");
            newLines.Add("  test_targets = get_test_targets(app_pod_name, installer)");
            newLines.Add("  targets = installer.aggregate_targets.select { |x| !test_targets.include?(x.name) }");
            newLines.Add("");
            newLines.Add("  # Checks each pair of targets if they have common pods. Duplicates are removed from the first one's xcconfig.");
            newLines.Add("  for i in 0..targets.size-1 do");
            newLines.Add("      target = targets[i]");
            newLines.Add("      remainingAppPodTargets = targets[i+1..targets.size-1].flat_map(&:pod_targets)");
            newLines.Add("");
            newLines.Add("      target.xcconfigs.each do |config_name, config_file|");
            newLines.Add("          # Removes all frameworks which exist in other pods");
            newLines.Add("          remainingAppPodTargets");
            newLines.Add("              .flat_map { |pod_target| get_framework_names(pod_target) }");
            newLines.Add("              .each { |framework| config_file.frameworks.delete(framework) }");
            newLines.Add("");
            newLines.Add("          # Saves updated xcconfig");
            newLines.Add("          xcconfig_path = target.xcconfig_path(config_name)");
            newLines.Add("          config_file.save_as(xcconfig_path)");
            newLines.Add("      end");
            newLines.Add("  end");
            newLines.Add("end");
            newLines.Add("");
            newLines.Add("def get_test_targets(app_pod_name, installer)");
            newLines.Add("  main_target_name = app_pod_name.gsub(\"Pods-\", \"\")");
            newLines.Add("");
            newLines.Add("  installer.aggregate_targets");
            newLines.Add("      .find { |x| x.name == app_pod_name }");
            newLines.Add("      .user_project");
            newLines.Add("      .targets");
            newLines.Add("      .select { |x| x.test_target_type? }");
            newLines.Add("      .flat_map { |x| [\"Pods-#{x}\", \"Pods-#{main_target_name}-#{x}\"] }");
            newLines.Add("      .select { |x| installer.aggregate_targets.map(&:name).include?(x) }");
            newLines.Add("      .uniq");
            newLines.Add("end");
            newLines.Add("");
            newLines.Add("def get_framework_names(pod_target)");
            newLines.Add("  frameworkNames = pod_target.specs.flat_map do |spec|");
            newLines.Add("      # We should take framework names from 'vendored_frameworks'.");
            newLines.Add("      # If it's not defined, we use 'spec.name' instead.");
            newLines.Add("      #");
            newLines.Add("      # spec.name can be defined like Framework/Something - we take the first part");
            newLines.Add("      # because that's what appears in OTHER_LDFLAGS.");
            newLines.Add("      frameworkPaths = unless spec.attributes_hash['ios'].nil?");
            newLines.Add("          then spec.attributes_hash['ios']['vendored_frameworks']");
            newLines.Add("          else spec.attributes_hash['vendored_frameworks']");
            newLines.Add("          end || [spec.name.split(/\\//, 2).first]");
            newLines.Add("");
            newLines.Add("      map_paths_to_filenames(frameworkPaths)");
            newLines.Add("  end");
            newLines.Add("");
            newLines.Add("  frameworkNames.uniq");
            newLines.Add("end");
            newLines.Add("");
            newLines.Add("def map_paths_to_filenames(paths)");
            newLines.Add("  Array(paths).map(&:to_s).map do |filename|");
            newLines.Add("      extension = File.extname filename");
            newLines.Add("      File.basename filename, extension");
            newLines.Add("  end");
            newLines.Add("end");
            newLines.Add("");
            newLines.Add("post_install do |installer|");
            newLines.Add("  remove_duplicated_frameworks('Pods-Unity-iPhone', installer)");
            newLines.Add("end");

            File.WriteAllLines(podFilePath, newLines);
        }
    }
}
#endif
