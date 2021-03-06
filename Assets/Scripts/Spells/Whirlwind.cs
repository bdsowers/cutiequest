﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlwind : Spell
{
    public int level = 1;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        int strength = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, caster);
        strength += level * 2;

        caster.GetComponentInChildren<ProjectileThrower>().ThrowProjectile(strength, Game.instance.avatar.direction);
    }
}
