using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class EnemySpellCaster : EnemyAI
{
    SpellCaster mSpellCaster;
    SimpleMovement mSimpleMovement;
    int mCastCounter = 0;

    private GameObject target
    {
        get
        {
            return Decoy.instance != null ? Decoy.instance.gameObject : Game.instance.avatar.gameObject;
        }
    }

    private void Start()
    {
        mSpellCaster = GetComponentInChildren<SpellCaster>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    public override void AIStructureChanged()
    {
        mSpellCaster = GetComponentInChildren<SpellCaster>();
    }

    public override void UpdateAI()
    {
        Vector3 largeDistanceDirection = OrthogonalDirection(transform, target.transform, true);
        Vector3 smallDistanceDirection = OrthogonalDirection(transform, target.transform, false);
        float distance = Vector3.Distance(transform.position, target.transform.position);

        mCastCounter--;
        
        if (mSpellCaster.IsInRange())
        {
            // Increase the likelihood that we'll try to move away from the player the closer they get.
            // However, for casters that target themselves, hold your ground for the most part

            bool run = false;
            if (mSpellCaster.targetCaster)
            {
                if (Random.Range(0f, 5f) > distance * 3)
                {
                    run = true;
                }
            }
            else
            {
                if (Random.Range(0f, 5f) > distance)
                {
                    run = true;
                }
            }

            if (mCastCounter > 0)
                run = true;

            Vector3 randomDirection = VectorHelper.RandomOrthogonalVectorXZ();

            if (run && mSimpleMovement.CanMove(-largeDistanceDirection))
            {
                mSimpleMovement.Move(-largeDistanceDirection);
            }
            else if (run && mSimpleMovement.CanMove(-smallDistanceDirection))
            {
                mSimpleMovement.Move(-smallDistanceDirection);
            }
            else if (run && mSimpleMovement.CanMove(randomDirection))
            {
                mSimpleMovement.Move(randomDirection);
            }
            else
            {
                CastSpell();
            }
        }
        else if (mSimpleMovement.CanMove(largeDistanceDirection))
        {
            mSimpleMovement.Move(largeDistanceDirection);
        }
        else
        {
            if (mSimpleMovement.CanMove(smallDistanceDirection))
            {
                mSimpleMovement.Move(smallDistanceDirection);
            }
        }
    }

    private void CastSpell()
    {
        int magic = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, gameObject);
        mSpellCaster.CastSpell(magic);
        mCastCounter = 3;
    }

    public override bool CanUpdateAI()
    {
        if (mSpellCaster != null && mSpellCaster.isCasting)
            return false;

        return true;
    }
}
