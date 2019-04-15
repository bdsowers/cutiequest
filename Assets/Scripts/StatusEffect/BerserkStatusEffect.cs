using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkStatusEffect : StatusEffect
{
    CharacterStatModifier mSpeedModifier = null;
    CharacterStatModifier mAttackModifier = null;
    CharacterStatModifier mDefenseModifier = null;

    public int level = 1;

    public override void OnAdded()
    {
        base.OnAdded();

        duration = 5f;

        mSpeedModifier = gameObject.AddComponent<CharacterStatModifier>();
        mSpeedModifier.SetRelativeModification(CharacterStatType.Speed, 3 * level);

        mAttackModifier = gameObject.AddComponent<CharacterStatModifier>();
        mAttackModifier.SetRelativeModification(CharacterStatType.Strength, 3 * level);

        mDefenseModifier = gameObject.AddComponent<CharacterStatModifier>();
        mDefenseModifier.SetRelativeModification(CharacterStatType.Defense, -3 * level);
    }

    public override void OnRemoved()
    {
        Destroy(mSpeedModifier);
        mSpeedModifier = null;

        Destroy(mAttackModifier);
        mAttackModifier = null;

        Destroy(mDefenseModifier);
        mDefenseModifier = null;

        base.OnRemoved();
    }
}
