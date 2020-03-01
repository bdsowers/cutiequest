using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearsightedQuirk : Quirk
{
    public static float ApplyQuirkIfPresent(float range)
    {
        // If Monocle is equipped, this is negated
        if (Game.instance.playerStats.IsItemEquipped<Monocle>())
        {
            return range;
        }

        if (Game.instance.quirkRegistry.IsQuirkActive<NearsightedQuirk>())
        {
            return range / 2;
        }

        return range;
    }
}
