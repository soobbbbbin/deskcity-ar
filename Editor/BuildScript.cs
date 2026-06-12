// BuildScript.cs
// --------------------------------------------------
// Provides a menu item and a static method to build the iOS player.
// --------------------------------------------------
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;

public class BuildScript {
    private const string IOS_OUTPUT_PATH = "Builds/iOS";

    [MenuItem("Tools/Build/Build iOS Player")]
    public static void BuildIOS() {
        // Ensure the output directory exists.
        if (!System.IO.Directory.Exists(IOS_OUTPUT_PATH)) {
            System.IO.Directory.CreateDirectory(IOS_OUTPUT_PATH);
        }
        // Gather scenes to include in the build.
        string[] scenes = GetEnabledScenes();
        BuildPlayerOptions options = new BuildPlayerOptions {
            scenes = scenes,
            locationPathName = IOS_OUTPUT_PATH,
            target = BuildTarget.iOS,
            options = BuildOptions.None
        };
        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result == BuildResult.Succeeded) {
            Debug.Log("[Build] iOS build completed successfully.");
        } else {
            Debug.LogError("[Build] iOS build failed: " + report.summary.result);
        }
    }

    private static string[] GetEnabledScenes() {
        var scenes = new System.Collections.Generic.List<string>();
        foreach (var scene in EditorBuildSettings.scenes) {
            if (scene.enabled) scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }
}
#endif
