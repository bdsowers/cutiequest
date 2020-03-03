using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadAtMathQuirk : Quirk
{
    public static int ApplyQuirkIfPresent(int cost, int maxDigits = 3)
    {
        if (Game.instance.quirkRegistry.IsQuirkActive<BadAtMathQuirk>())
        {
            if (maxDigits == 2)
                return Random.Range(1, 99);
            else
                return Random.Range(1, 999);
        }

        return cost;
    }
}
