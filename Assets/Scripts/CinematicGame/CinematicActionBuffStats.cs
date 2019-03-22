using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicActionBuffStats : CinematicAction
{
    public override string actionName => "buff_stats";

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        GameObject dialog = player.objectMap.GetObjectByName("buff_stats_dialog");
        dialog.GetComponent<BuffStatsDialog>().ShowDialog();

        while (dialog.activeInHierarchy)
            yield return null;

        yield break;
    }
}
