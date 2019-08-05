using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionIncreaseAttraction : CinematicAction
{
    private string mMessage;
    private int mCost;
    private string mSuccessEvent;
    private string mFailEvent;
    private string mEffect;

    public override string actionName => "increase_attraction";

    public override string[] aliases
    {
        get
        {
            return new string[] { "cost_dialog" };
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mMessage = dataProvider.GetStringData(mParameters, "message");
        mCost = dataProvider.GetIntData(mParameters, "cost");
        mSuccessEvent = dataProvider.GetStringData(mParameters, "success");
        mFailEvent = dataProvider.GetStringData(mParameters, "fail");
        mEffect = dataProvider.GetStringData(mParameters, "effect");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject dialog = player.objectMap.GetObjectByName("increase_attractiveness_dialog");
        dialog.GetComponent<IncreaseAttractivenessDialog>().ShowDialog(mMessage, mCost, mEffect, mSuccessEvent, mFailEvent);

        while (dialog.activeInHierarchy)
            yield return null;

        yield break;
    }
}
