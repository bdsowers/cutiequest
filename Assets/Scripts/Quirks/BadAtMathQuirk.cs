using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadAtMathQuirk : Quirk
{
    public static int ApplyQuirkIfPresent(int cost)
    {
        BadAtMathQuirk quirk = GameObject.FindObjectOfType<BadAtMathQuirk>();
        if (quirk != null)
        {
            return Random.Range(1, 999);
        }

        return cost;
    }
}
