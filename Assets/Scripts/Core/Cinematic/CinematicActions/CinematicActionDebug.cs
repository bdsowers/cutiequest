using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionDebug : CinematicAction
{
    public override string actionName
    {
        get { return "debug"; }
    }

    public override string simpleParameterName
    {
        get { return "message"; }
    }

    private string mMessage;

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mMessage = dataProvider.GetStringData(mParameters, "message");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Debug.Log(mMessage);

        yield break;
    }
}
