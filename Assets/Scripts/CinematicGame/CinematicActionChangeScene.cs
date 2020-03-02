using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionChangeScene : CinematicAction
{
    public override string actionName
    {
        get { return "change_scene"; }
    }

    public override string simpleParameterName
    {
        get { return "scene"; }
    }

    private string mScene;

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mScene = dataProvider.GetStringData(mParameters, "scene");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Game.instance.transitionManager.TransitionToScreen(mScene);

        yield break;
    }
}
