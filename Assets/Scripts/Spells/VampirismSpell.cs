using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VampirismSpell : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        int strength = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, caster);

        caster.AddComponent<VampirismStatusEffect>().strength = strength;
    }
}
