using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionMoveTo : CinematicAction
{
    private string mTarget;
    private string mDestination;
    private float mSeconds;

    public override string actionName
    {
        get
        {
            return "move_to";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        mDestination = dataProvider.GetStringData(mParameters, "destination");
        mSeconds = dataProvider.GetFloatData(mParameters, "seconds", 1f);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject target = player.objectMap.GetObjectByName(mTarget);
        GameObject destination = player.objectMap.GetObjectByName(mDestination);

        Vector3 startPosition = target.transform.position;
        Vector3 endPosition = destination.transform.position;

        float time = 0f;
        while (time < mSeconds)
        {
            time += Time.deltaTime;
            Vector3 currentPosition = Vector3.Lerp(startPosition, endPosition, time / mSeconds);
            target.transform.position = currentPosition;
            yield return null;
        }

        target.transform.position = endPosition;

        yield break;
    }
}
