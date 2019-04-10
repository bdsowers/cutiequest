﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpellCaster : EnemyAI
{
    SpellCaster mSpellCaster;
    SimpleMovement mSimpleMovement;

    private void Start()
    {
        mSpellCaster = GetComponent<SpellCaster>();
        mSimpleMovement = GetComponent<SimpleMovement>();
    }

    public override void UpdateAI()
    {
        Vector3 largeDistanceDirection = OrthogonalDirection(transform, Game.instance.avatar.transform, true);
        Vector3 smallDistanceDirection = OrthogonalDirection(transform, Game.instance.avatar.transform, false);
        float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);

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

            if (run && mSimpleMovement.CanMove(-largeDistanceDirection))
            {
                mSimpleMovement.Move(-largeDistanceDirection);
            }
            else if (run && mSimpleMovement.CanMove(-smallDistanceDirection))
            {
                mSimpleMovement.Move(-smallDistanceDirection);
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
    }

    public override bool CanUpdateAI()
    {
        if (mSpellCaster != null && mSpellCaster.isCasting)
            return false;

        return true;
    }
}