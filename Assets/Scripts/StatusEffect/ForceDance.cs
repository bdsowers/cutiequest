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

    public void UpdateAI(CharacterComponentBase ccb)
    {
        SimpleMovement.OrientToDirection(ccb.commonComponents.simpleMovement.subMesh, new Vector3(0, 0, -1));
        ccb.commonComponents.animator.Play("Dance1");
    }
}
