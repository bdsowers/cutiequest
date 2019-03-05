using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionWaitSeconds : CinematicAction
{
    float mSecondsToWait = 0f;
    float mTimer = 0f;

    public override string actionName
    {
        get
        {
            return "wait_seconds";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "seconds";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mSecondsToWait = dataProvider.GetFloatData(mParameters, "seconds");

        shouldYield = true;
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        mTimer = 0f;
        while (mTimer < mSecondsToWait)
        {
            mTimer += Time.deltaTime * player.playbackTimeScale;
            yield return null;
        }

        yield break;
    }
}
