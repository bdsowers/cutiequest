﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionEnable : CinematicAction
{
    public override string actionName
    {
        get { return "enable"; }
    }

    public override string simpleParameterName
    {
        get { return "target"; }
    }
    private string mTarget;

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mTarget = dataProvider.GetStringData(mParameters, "target");
        Debug.Assert(mTarget != null);
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        player.objectMap.GetObjectByName(mTarget).SetActive(true);

        yield break;
    }
}
