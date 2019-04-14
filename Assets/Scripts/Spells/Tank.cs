using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        caster.AddComponent<TankStatusEffect>();
    }
}
