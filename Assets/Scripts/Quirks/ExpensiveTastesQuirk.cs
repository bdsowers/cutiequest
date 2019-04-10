using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpensiveTastesQuirk : Quirk
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

    public static bool isQuirkPresent
    {
        get { return mEnabled; }
    }

    public static int CostModification()
    {
        return Random.Range(10, 100);
    }
}
