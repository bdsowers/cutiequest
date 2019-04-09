﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadAtMathQuirk : Quirk
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

    public static int ApplyQuirkIfPresent(int cost)
    {
        if (mEnabled)
        {
            return Random.Range(1, 999);
        }

        return cost;
    }
}
