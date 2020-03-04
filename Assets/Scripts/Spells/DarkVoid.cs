using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkVoid : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        int strength = caster.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Magic, caster);
        strength = Mathf.Max(strength, 1);
        strength *= 2;

        caster.GetComponentInChildren<SpellCaster>().CastSpell(strength);
    }
}
