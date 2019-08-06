using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionSetQuestRTutorial : CinematicAction
{
    private bool mEnabled;

    public override string actionName
    {
        get
        {
            return "questr_tutorial";
        }
    }

    public override string simpleParameterName
    {
        get { return "enabled"; }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mEnabled = dataProvider.GetBoolData(mParameters, "enabled");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject questr = player.objectMap.GetObjectByName("questr");
        questr.GetComponent<QuestR>().SetTutorialMode(mEnabled);

        yield break;
    }
}
