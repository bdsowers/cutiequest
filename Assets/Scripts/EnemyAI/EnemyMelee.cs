using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : EnemyAI
{
    SimpleAttack mSimpleAttack;
    SimpleMovement mSimpleMovement;
    EnemyTeleport mTeleport;

    private bool mUpdateMeleeAI = true;
    public bool UpdateMeleeAI
    {
        get { return mUpdateMeleeAI; }
        set { mUpdateMeleeAI = value; }
    }

    private void Start()
    {
        mSimpleAttack = GetComponent<SimpleAttack>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mTeleport = GetComponent<EnemyTeleport>();
    }

    public override void AIStructureChanged()
    {
        throw new System.NotImplementedException();
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

        if (mTeleport != null && mTeleport.ShouldTeleport())
        {
            mTeleport.Teleport();
        }
        else if (mSimpleAttack != null && mSimpleAttack.CanAttack(direction))
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
        if (!mUpdateMeleeAI)
            return false;

        if (mSimpleAttack != null && mSimpleAttack.isAttacking)
            return false;

        return true;
    }
}
