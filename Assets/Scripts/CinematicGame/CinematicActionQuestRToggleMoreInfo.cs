using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionQuestRToggleMoreInfo : CinematicAction
{
    public override string actionName
    {
        get
        {
            return "questr_moreinfo";
        }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject panel = player.objectMap.GetObjectByName("questr_panel");
        panel.GetComponent<QuestRPanel>().OnMoreInfoPressed();

        yield break;
    }
}
