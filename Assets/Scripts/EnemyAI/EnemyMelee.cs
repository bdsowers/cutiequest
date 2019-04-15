using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyAI
{
    SimpleAttack mSimpleAttack;
    SimpleMovement mSimpleMovement;

    private void Start()
    {
        mSimpleAttack = GetComponent<SimpleAttack>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    private GameObject target
    {
        get
        {
            return Decoy.instance != null ? Decoy.instance.gameObject : Game.instance.avatar.gameObject;
        }
    }

    public override void UpdateAI()
    {
        Vector3 direction = OrthogonalDirection(transform, target.transform, true);

        if (mSimpleAttack != null && mSimpleAttack.CanAttack(direction))
        {
            mSimpleAttack.Attack(direction);
        }
        else if (mSimpleMovement.CanMove(direction))
        {
            mSimpleMovement.Move(direction);
        }
        else
        {
            direction = OrthogonalDirection(transform, target.transform, false);
            if (mSimpleMovement.CanMove(direction))
            {
                mSimpleMovement.Move(direction);
            }
        }
    }

    public override bool CanUpdateAI()
    {
        if (mSimpleAttack != null && mSimpleAttack.isAttacking)
            return false;

        return true;
    }
}
