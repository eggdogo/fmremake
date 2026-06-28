using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

internal sealed class AppleDoubleCleaner : IPreprocessBuildWithReport
{
    public int callbackOrder => int.MinValue;

    [InitializeOnLoadMethod]
    private static void CleanOnEditorLoad()
    {
        CleanProjectSidecars();
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        CleanProjectSidecars();
    }

    private static void CleanProjectSidecars()
    {
        var removedAny = false;

        foreach (var root in new[]
        {
            "Assets",
            "Packages",
            "ProjectSettings",
            "Library/PackageCache",
            "Library/ScriptAssemblies",
        })
        {
            if (!Directory.Exists(root))
            {
                continue;
            }

            foreach (var path in Directory.EnumerateFiles(root, "._*", SearchOption.AllDirectories))
            {
                try
                {
                    File.Delete(path);
                    removedAny = true;
                }
                catch (Exception exception)
                {
                    UnityEngine.Debug.LogWarning($"Could not remove AppleDouble sidecar '{path}': {exception.Message}");
                }
            }
        }

        if (removedAny && !BuildPipeline.isBuildingPlayer)
        {
            AssetDatabase.Refresh();
        }
    }
}
