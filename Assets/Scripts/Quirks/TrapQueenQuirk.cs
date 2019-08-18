using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapQueenQuirk : Quirk
{
    private static bool mEnabled;
    public static bool quirkEnabled {  get { return mEnabled; } }

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
}
