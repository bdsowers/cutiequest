using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionCallFunction : CinematicAction
{
    private string mTarget;
    private string mFunction;

    public override string actionName
    {
        get
        {
            return "call";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        mFunction = dataProvider.GetStringData(mParameters, "function");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject target = player.objectMap.GetObjectByName(mTarget);
        target.SendMessage(mFunction);

        yield break;
    }
}
