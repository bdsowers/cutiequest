using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadAtMathQuirk : Quirk
{
    public static int ApplyQuirkIfPresent(int cost)
    {
        if (Game.instance.quirkRegistry.IsQuirkActive<BadAtMathQuirk>())
        {
            return Random.Range(1, 999);
        }

        return cost;
    }
}
