using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainFreeze : StatusEffect
{
    protected override void Start()
    {
        base.Start();

        mDuration = 5f;
        mTimeLeft = 5f;
    }
}
