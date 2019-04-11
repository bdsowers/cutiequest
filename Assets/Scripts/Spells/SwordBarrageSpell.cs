using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBarrageSpell : Spell
{
    public int level = 1;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        int strength = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, caster);
        strength += level * 4;

        caster.GetComponentInChildren<ProjectileThrower>().ThrowProjectile(strength, Game.instance.avatar.direction);

        Vector3 offset = (Mathf.Abs(Game.instance.avatar.direction.x) > 0.1f) ? new Vector3(0, 0, 1) : new Vector3(1, 0, 0);
        caster.GetComponentInChildren<ProjectileThrower>().ThrowProjectile(strength, Game.instance.avatar.direction, offset);
        caster.GetComponentInChildren<ProjectileThrower>().ThrowProjectile(strength, Game.instance.avatar.direction, offset * -1);
    }
}
