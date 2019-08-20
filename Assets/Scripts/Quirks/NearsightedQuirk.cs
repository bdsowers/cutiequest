using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearsightedQuirk : Quirk
{
    public static float ApplyQuirkIfPresent(float range)
    {
        if (Game.instance.quirkRegistry.IsQuirkActive<NearsightedQuirk>())
        {
            return range / 2;
        }

        return range;
    }
}
