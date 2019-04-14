using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        caster.AddComponent<TankStatusEffect>();

        GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX2_PickupDiamond2");
        vfx.transform.position = transform.position;
        vfx.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
    }
}
