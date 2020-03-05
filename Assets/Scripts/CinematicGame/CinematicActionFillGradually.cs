using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionFillGradually : CinematicAction
{
    public override string actionName
    {
        get { return "fill_gradually"; }
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject obj = player.objectMap.GetObjectByName("road3");
        if (obj != null)
        {
            obj.GetComponent<FillRegion>().fillGradually = true;
        }

        yield break;
    }
}
