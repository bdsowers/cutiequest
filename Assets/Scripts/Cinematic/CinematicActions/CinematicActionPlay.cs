using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionPlay : CinematicAction
{
    private string mAnimationName;

    public override string actionName
    {
        get
        {
            return "play";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "animation";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mAnimationName = dataProvider.GetStringData(mParameters, "animation");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        yield return player.PlayCinematicAnimation(mAnimationName);

        yield break;
    }
}
