using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WantedQuirk : Quirk
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

    public static int ApplyQuirkIfPresent(int numEnemies)
    {
        if (mEnabled)
        {
            return numEnemies + Mathf.RoundToInt(numEnemies * 0.25f);
        }

        return numEnemies;
    }
}
