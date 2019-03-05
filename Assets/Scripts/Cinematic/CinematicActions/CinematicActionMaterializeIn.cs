using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionMaterializeIn : CinematicAction
{
    private string mTarget;
    private float mSeconds;

    public override string actionName
    {
        get
        {
            return "materialize_in";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        mSeconds = dataProvider.GetFloatData(mParameters, "seconds");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject target = player.objectMap.GetObjectByName(mTarget);

        Renderer renderer = target.GetComponentInChildren<Renderer>();
        CinematicHelpers.MakeMaterialInstancedIfNecessary(renderer);

        float distance = -1f;
        while (distance < 1f)
        {
            renderer.material.SetFloat("_Distance", distance);
            distance += Time.deltaTime * (1 / mSeconds) * 2f * player.playbackTimeScale;
            yield return null;
        }

        renderer.material.SetFloat("_Distance", 1f);

        yield break;
    }
}
