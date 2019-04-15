using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkSpell : Spell
{
    public int level = 1;

    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        caster.AddComponent<BerserkStatusEffect>().level = level;
    }
}
