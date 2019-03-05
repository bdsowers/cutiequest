using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_CREATIVESPORE
using CreativeSpore.SuperTilemapEditor;
#endif

public class CinematicActionHideMaterializers : CinematicAction
{
    public override string actionName
    {
        get
        {
            return "hide_materializers";
        }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Renderer[] renderers = GameObject.FindObjectsOfType<Renderer>();
        for (int i = 0; i < renderers.Length; ++i)
        {
            if (renderers[i].material.HasProperty("_Distance"))
            {
                CinematicHelpers.MakeMaterialInstancedIfNecessary(renderers[i]);
                renderers[i].material.SetFloat("_Distance", -1f);
            }
        }

#if USING_CREATIVESPORE
        STETilemap[] tileMaps = GameObject.FindObjectsOfType<STETilemap>();
        for (int i = 0; i < tileMaps.Length; ++i)
        {
            if (tileMaps[i].Material.HasProperty("_Distance"))
            {
                CinematicHelpers.MakeMaterialInstancedIfNecessary(tileMaps[i]);
                tileMaps[i].Material.SetFloat("_Distance", -1f);
            }
        }
#endif
        yield break;
    }
}
