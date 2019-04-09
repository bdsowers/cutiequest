using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadAtMathQuirk : Quirk
{
    public static int ApplyQuirkIfPresent(int cost)
    {
        if (GameObject.FindObjectsOfType<BadAtMathQuirk>() != null)
        {
            return Random.Range(1, 999);
        }

        return cost;
    }
}
