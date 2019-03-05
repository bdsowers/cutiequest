using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicAnimation : CinematicAction
{
    private bool mDefaultYield;

    public override string actionName
    {
        get
        {
            return "animation";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "id";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mDefaultYield = dataProvider.GetBoolData(mParameters, "default_yield");
    }

    public override void AddChildAction(CinematicAction childAction)
    {
        base.AddChildAction(childAction);

        childAction.shouldYield = (childAction.shouldYield || mDefaultYield);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        yield return PlayChildActions(player);
    }
}
