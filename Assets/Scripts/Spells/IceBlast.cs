using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlast : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        caster.GetComponentInChildren<SpellCaster>().CastSpell(5);
    }
}
