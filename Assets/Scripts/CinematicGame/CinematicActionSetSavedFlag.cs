using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionSetSavedFlag : CinematicAction
{
    private string mFlag;

    public override string actionName
    {
        get
        {
            return "set_saved_flag";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "flag";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mFlag = dataProvider.GetStringData(mParameters, "flag");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Game.instance.playerData.SetFlag(mFlag);
        player.dataProvider.SetData(mFlag, "true");

        yield break;
    }
}
