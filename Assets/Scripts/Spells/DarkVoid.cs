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

        caster.GetComponentInChildren<SpellCaster>().CastSpell(strength);

        // todo bdsowers - find other traps and destroy them
    }
}
