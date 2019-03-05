using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_CREATIVESPORE
using CreativeSpore.SuperTilemapEditor;
#endif

/// <summary>
/// Static methods that get used a lot in the cinematic system
/// </summary>
public class CinematicHelpers : MonoBehaviour
{
    public static void MakeMaterialInstancedIfNecessary(Renderer renderer)
    {
        if (!renderer.material.name.Contains("Instance"))
        {
            renderer.material = new Material(renderer.material);
        }
    }

#if USING_CREATIVESPORE
    public static void MakeMaterialInstancedIfNecessary(STETilemap tileMap)
    {
        // TODO: this feels kidn of kludgey
        if (!tileMap.Material.name.Contains("Instance"))
        {
            tileMap.Material = new Material(tileMap.Material);
        }
    }
#endif
}
