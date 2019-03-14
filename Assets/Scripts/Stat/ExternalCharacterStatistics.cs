using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalCharacterStatistics : CharacterStatistics
{
    private CharacterStatistics mExternalReference;

    public CharacterStatistics externalReference
    {
        get { return mExternalReference; }
        set { mExternalReference = value; }
    }

    public override int ModifiedStatValue(CharacterStatType statType)
    {
        if (externalReference == this)
        {
            return 1;
        }

        int value = externalReference.ModifiedStatValue(statType);

        CharacterStatModifier[] modifiers = GetComponentsInChildren<CharacterStatModifier>();

        // Absolute modifications take preference over relative ones
        for (int i = 0; i < modifiers.Length; ++i)
        {
            if (modifiers[i].statType == statType && modifiers[i].isRelative)
            {
                value += modifiers[i].modification;
            }
        }

        for (int i = 0; i < modifiers.Length; ++i)
        {
            if (modifiers[i].statType == statType && modifiers[i].isAbsolute)
            {
                value = modifiers[i].modification;
            }
        }

        PlayerController pc = GetComponent<PlayerController>();
        if (pc != null)
        {
            // Apply relevant character stat boost
            if (Game.instance.playerData.followerUid != null)
            {
                CharacterData followerData = Game.instance.characterDataList.CharacterWithUID(Game.instance.playerData.followerUid);
                if (followerData.statBoost == statType)
                {
                    value += followerData.statBoostAmount;
                }
            }
        }

        return value;
    }
}
