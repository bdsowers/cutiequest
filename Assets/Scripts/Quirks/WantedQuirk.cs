using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedQuirk : Quirk
{
    public static int ApplyQuirkIfPresent(int numEnemies)
    {
        if (Game.instance.quirkRegistry.IsQuirkActive<WantedQuirk>())
        {
            return numEnemies + Mathf.RoundToInt(numEnemies * 0.25f);
        }

        return numEnemies;
    }
}
