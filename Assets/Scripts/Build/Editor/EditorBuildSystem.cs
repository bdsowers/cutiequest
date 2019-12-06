using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorBuildSystem : MonoBehaviour
{
    [MenuItem("HSR/Steam Build")]
    public static void SteamBuild()
    {
        BuildSystem.SteamBuild();
    }

    [MenuItem("HSR/Convention Build")]
    public static void ConventionBuild()
    {
        BuildSystem.ConventionBuild();
    }
}
