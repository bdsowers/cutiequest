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

    public void UpdateAI(CharacterComponentBase ccb)
    {
        SimpleMovement.OrientToDirection(ccb.commonComponents.simpleMovement.subMesh, new Vector3(0, 0, -1));
        ccb.commonComponents.animator.Play("Dizzy");
    }
}
