using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearsightedQuirk : Quirk
{
    private static bool mEnabled;

    private void Start()
    {
        mEnabled = true;
    }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDestroy()
    {
        mEnabled = false;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    public static float ApplyQuirkIfPresent(float range)
    {
        if (mEnabled)
        {
            return range / 2;
        }

        return range;
    }
}
