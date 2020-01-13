﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BuildSystem : MonoBehaviour
{
    private static string mSteamBuildLocation = "Steam/sdk/tools/ContentBuilder/content/HeroesSwipeRight.exe";
    private static string mConventionBuildLocation = "Build/convention/HeroesSwipeRight.exe";

    private static string BuildPath(string relativeTarget)
    {
        string outputPath = Application.dataPath;
        DirectoryInfo di = new DirectoryInfo(outputPath);
        string fullPath = Path.Combine(di.Parent.FullName, relativeTarget);
        return fullPath;
    }

    public static void SteamBuild()
    {
        // TODO BDSOWERS - update build number
        // TODO BDSOWERS - auto-send to Steam

        string outputPath = BuildPath(mSteamBuildLocation);

        BuildOptions options = BuildOptions.None;
        options |= BuildOptions.Development;
        options |= BuildOptions.AllowDebugging;

        List<string> flags = new List<string>();
        flags.Add("DISABLE_CHEATS");

        Build(outputPath, options, flags);
    }

    public static void ConventionBuild()
    {
        string outputPath = BuildPath(mConventionBuildLocation);

        BuildOptions options = BuildOptions.None;
        options |= BuildOptions.Development;
        options |= BuildOptions.AllowDebugging;

        List<string> flags = new List<string>();
        flags.Add("DEMO");

        Build(outputPath, options, flags);
    }

    public static void Build(string outputPath, BuildOptions options, List<string> flags)
    {
        string cacheSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        string newSymbols = ConstructPreprocessorList(flags);
        Debug.Log(newSymbols);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, newSymbols);

        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, outputPath, BuildTarget.StandaloneWindows, options);
        Debug.Log("Build Complete: " + outputPath);

        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, cacheSymbols);
    }

    private static string ConstructPreprocessorList(List<string> flags)
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone);
        string[] currentFlags = symbols.Split(';');
        List<string> finalFlags = new List<string>(currentFlags);
        finalFlags.AddRange(flags);
        return string.Join(";", finalFlags);
    }
}