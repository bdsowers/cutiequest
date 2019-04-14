using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHellSpell : Spell
{
    public int level = 1;
    private GameObject mCaster;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);
        mCaster = caster;

        GetComponent<ProjectileThrower>().suppressStateUpdates = true;
        FireProjectile();
        Invoke("FireProjectile", 0.35f);
        Invoke("FireProjectile", 0.7f);
        Invoke("Finished", 1f);
    }

    void FireProjectile()
    {
        int strength = mCaster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, mCaster);
        strength += level * 1;

        mCaster.GetComponentInChildren<ProjectileThrower>().ThrowProjectile(strength, Game.instance.avatar.direction);
    }

    void Finished()
    {
        GetComponent<ProjectileThrower>().suppressStateUpdates = false;
        GetComponent<ProjectileThrower>().isThrowing = false;
    }
}
