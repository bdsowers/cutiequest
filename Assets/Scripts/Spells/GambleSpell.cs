﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GambleSpell : Spell
{
    public int level = 1;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        int strength = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, caster);
        strength += Random.Range(-5, 5);

        int luck = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Luck, caster);
        strength += luck / 4;

        caster.GetComponentInChildren<SpellCaster>().CastSpell(strength);
    }
}
