using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionQuestRToggleMoreInfo : CinematicAction
{
    private bool mEnabled;

    public override string actionName
    {
        get
        {
            return "questr_moreinfo";
        }
    }

    public override string simpleParameterName
    {
        get
        {
            return "enabled";
        }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mEnabled = dataProvider.GetBoolData(mParameters, "enabled");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject panel = player.objectMap.GetObjectByName("questr_panel");

        if (mEnabled)
        {
            panel.GetComponent<QuestRPanel>().MoreInfo();
        }
        else
        {
            panel.GetComponent<QuestRPanel>().LessInfo();
        }

        yield break;
    }
}
