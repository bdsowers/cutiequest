using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceDance : StatusEffect
{
    protected override void Start()
    {
        base.Start();

        mDuration = 5f;
        mTimeLeft = mDuration;
    }
}
