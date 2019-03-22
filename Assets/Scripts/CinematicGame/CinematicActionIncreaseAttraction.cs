using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionIncreaseAttraction : CinematicAction
{
    public override string actionName => "increase_attraction";

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject dialog = player.objectMap.GetObjectByName("increase_attractiveness_dialog");
        dialog.GetComponent<IncreaseAttractivenessDialog>().ShowDialog();

        while (dialog.activeInHierarchy)
            yield return null;

        yield break;
    }
}
