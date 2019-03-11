using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HasteStatusEffect : StatusEffect
{
    CharacterStatModifier mSpeedModifier = null;

    public override void OnAdded()
    {
        base.OnAdded();

        duration = 5f;

        mSpeedModifier = gameObject.AddComponent<CharacterStatModifier>();
        mSpeedModifier.SetRelativeModification(CharacterStatType.Speed, 5);
    }

    public override void OnRemoved()
    {
        Destroy(mSpeedModifier);
        mSpeedModifier = null;

        base.OnRemoved();
    }
}
