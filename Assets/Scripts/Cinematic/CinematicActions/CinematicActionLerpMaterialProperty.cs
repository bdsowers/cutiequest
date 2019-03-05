using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionLerpMaterialProperty : CinematicAction
{
    private string mTarget;
    private string mProperty;
    private float mSeconds;
    private float mStartValue = 0f;
    private float mEndValue = 1f;

    public override string actionName
    {
        get
        {
            return "lerp_material_property";
        }
    }

    public override string[] aliases
    {
        get
        {
            return new string[] { "fade_in" };
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "target";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        mProperty = dataProvider.GetStringData(mParameters, "property");

        if (mProperty == null && alias == "fade_in")
        {
            mProperty = "_Alpha";
        }

        mSeconds = dataProvider.GetFloatData(mParameters, "seconds", 1f);
        mStartValue = dataProvider.GetFloatData(mParameters, "start", 0f);
        mEndValue = dataProvider.GetFloatData(mParameters, "end", 1f);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject target = player.objectMap.GetObjectByName(mTarget);
        Renderer renderer = target.GetComponent<Renderer>();

        float alpha = mStartValue;
        while (alpha < 1f)
        {
            renderer.material.SetFloat(mProperty, alpha);
            alpha += (mEndValue - mStartValue) * Time.deltaTime * (1 / mSeconds) * player.playbackTimeScale;
            yield return null;
        }

        renderer.material.SetFloat(mProperty, mEndValue);

        yield break;
    }
}
