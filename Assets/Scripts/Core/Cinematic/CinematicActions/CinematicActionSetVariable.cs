using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionSetVariable : CinematicAction
{
    private string mKey;
    private string mValue;

    public override string actionName
    {
        get
        {
            return "set";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mKey = mParameters["key"]; // Use the raw value to ensure it doesn't get converted...
        mValue = dataProvider.GetStringData(mParameters, "value");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        player.dataProvider.SetData(mKey, mValue);

        yield break;
    }
}
