#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class WebGLBuildTool
{
    private const string OutputPath = "Build/WebGL";

    [MenuItem("Tools/Seoul Firework/Build WebGL")]
    public static void BuildWebGL()
    {
        ConfigureWebGLSettings();

        string[] scenes = GetEnabledScenes();
        if (scenes.Length == 0)
        {
            Debug.LogError("No enabled scenes in Build Settings.");
            return;
        }

        if (!Directory.Exists(OutputPath))
        {
            Directory.CreateDirectory(OutputPath);
        }

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = OutputPath,
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("WebGL build succeeded: " + OutputPath + " (" + summary.totalSize + " bytes)");
            AssetDatabase.Refresh();
        }
        else
        {
            Debug.LogError("WebGL build failed: " + summary.result);
        }
    }

    [MenuItem("Tools/Seoul Firework/Apply WebGL Player Defaults")]
    public static void ConfigureWebGLSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);

        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Brotli;
        PlayerSettings.WebGL.decompressionFallback = true;
        PlayerSettings.WebGL.dataCaching = true;

        PlayerSettings.companyName = "Seoul Firework Studio";
        PlayerSettings.productName = "Seoul Fireworks 180";

        Debug.Log("Applied WebGL defaults: Brotli, Decompression Fallback ON, Data Caching ON.");
    }

    private static string[] GetEnabledScenes()
    {
        var enabledScenes = new System.Collections.Generic.List<string>();
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled)
            {
                enabledScenes.Add(scenes[i].path);
            }
        }

        return enabledScenes.ToArray();
    }
}
#endif
