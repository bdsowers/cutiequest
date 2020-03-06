using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionShowPopup : CinematicAction
{
    public override string actionName
    {
        get { return "show_popup"; }
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
        NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, mMessage, NumberPopupReason.Bad, 0f);

        yield break;
    }
}
