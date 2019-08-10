using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasteSpell : Spell
{
    public int speedModifier;
    public float duration;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        caster.AddComponent<HasteStatusEffect>();

        PlayBoilerplateVFX();
    }
}
