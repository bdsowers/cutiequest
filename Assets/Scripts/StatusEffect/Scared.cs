using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scared : StatusEffect
{
    protected override void Start()
    {
        base.Start();

        mDuration = 4f;
        mTimeLeft = 4f;
    }

    public void TryMoveAwayFromPlayer(CharacterComponentBase ccb)
    {
        Vector3 awayFromPlayer = transform.position - Game.instance.avatar.transform.position;
        awayFromPlayer.y = 0f;

        // Try one axis, than the other
        Vector3 firstDir, secondDir;

        if (Mathf.Abs(awayFromPlayer.x) > Mathf.Abs(awayFromPlayer.z))
        {
            firstDir = awayFromPlayer;
            firstDir.z = 0f;

            secondDir = awayFromPlayer;
            secondDir.x = 0f;
        }
        else
        {
            firstDir = awayFromPlayer;
            firstDir.x = 0f;

            secondDir = awayFromPlayer;
            secondDir.z = 0f;
        }

        firstDir.Normalize();
        secondDir.Normalize();

        if (firstDir.magnitude > 0.1f && ccb.commonComponents.simpleMovement.CanMove(firstDir))
        {
            ccb.commonComponents.simpleMovement.Move(firstDir);
        }
        else if (secondDir.magnitude > 0.1f && ccb.commonComponents.simpleMovement.CanMove(secondDir))
        {
            ccb.commonComponents.simpleMovement.Move(secondDir);
        }
    }
}
