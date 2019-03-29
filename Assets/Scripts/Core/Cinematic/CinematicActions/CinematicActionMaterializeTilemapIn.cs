using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if USING_CREATIVESPORE
using CreativeSpore.SuperTilemapEditor;
#endif

public class CinematicActionMaterializeTilemapIn : CinematicAction
{
    private string mTarget;
    private float mSeconds;

    private bool mReverse;

    public override string actionName
    {
        get
        {
            return "materialize_tilemap_in";
        }
    }

    public override string[] aliases
    {
        get
        {
            return new string[] { "materialize_tilemap_out" };
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        mSeconds = dataProvider.GetFloatData(mParameters, "seconds", 1f);

        mReverse = (alias == "materialize_tilemap_out");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
#if USING_CREATIVESPORE
        GameObject target = player.objectMap.GetObjectByName(mTarget);

        STETilemap actualTilemap = target.GetComponent<STETilemap>();
        CinematicHelpers.MakeMaterialInstancedIfNecessary(actualTilemap);
        
        float distance = -1f;
        while (distance < 1f)
        {
            SetMaterialDistance(actualTilemap, distance);
            distance += Time.deltaTime * (1 / mSeconds) * 2f * player.playbackTimeScale;
            yield return null;
        }

        SetMaterialDistance(actualTilemap, 1f);
#endif
        yield break;
    }

#if USING_CREATIVESPORE
    private void SetMaterialDistance(STETilemap actualTilemap, float distance)
    {
        if (mReverse)
        {
            distance = -distance;
        }

        actualTilemap.Material.SetFloat("_Distance", distance);
    }
#endif
}
