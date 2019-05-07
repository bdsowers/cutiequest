using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1 : EnemyAI
{
    // Core actions:
    // * Spawn skeletons
    // * Keep some distance between it & player
    // * Large spell
    // * High-damage projectile when player isn't close
    // * Spell gets bigger & # skeletons become higher as health decreases

    SpellCaster[] mSpellCasters;

    private void Start()
    {
        mSpellCasters = GetComponentsInChildren<SpellCaster>();    
    }

    public override bool CanUpdateAI()
    {
        return !SpellCaster.AnySpellCastersCasting(mSpellCasters);
    }

    public override void UpdateAI()
    {
        mSpellCasters[0].CastSpell(5);
    }

    
}
