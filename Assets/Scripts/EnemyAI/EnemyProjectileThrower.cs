using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileThrower : EnemyAI
{
    ProjectileThrower mProjectileThrower;
    SimpleMovement mSimpleMovement;

    private void Start()
    {
        mProjectileThrower = GetComponent<ProjectileThrower>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    public override void UpdateAI()
    {
        Vector3 direction = OrthogonalDirection(transform, Game.instance.avatar.transform, true);

        if (mProjectileThrower != null && mProjectileThrower.ShouldThrow())
        {
            int magic = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, gameObject);
            mProjectileThrower.ThrowProjectile(magic);
        }
        else if (mSimpleMovement.CanMove(direction))
        {
            mSimpleMovement.Move(direction);
        }
        else
        {
            direction = OrthogonalDirection(transform, GameObject.Find("Avatar").transform, false);
            if (mSimpleMovement.CanMove(direction))
            {
                mSimpleMovement.Move(direction);
            }
        }
    }

    public override bool CanUpdateAI()
    {
        if (mProjectileThrower != null && mProjectileThrower.isThrowing)
            return false;

        return true;
    }
}
