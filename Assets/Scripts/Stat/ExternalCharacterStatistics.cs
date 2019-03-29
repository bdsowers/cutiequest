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

    public override int BaseStatValue(CharacterStatType statType)
    {
        return externalReference.BaseStatValue(statType);
    }

    public override void ChangeBaseStat(CharacterStatType statType, int newValue)
    {
        externalReference.ChangeBaseStat(statType, newValue);
    }

    public override int ModifiedStatValue(CharacterStatType statType, GameObject entity)
    {
        int valueOnThisEntity = externalReference.ModifiedStatValue(statType, entity);
        int valueOnExternalRef = externalReference.ModifiedStatValue(statType, externalReference.gameObject);
        int doubleBaseCorrection = externalReference.BaseStatValue(statType);
        int total = valueOnThisEntity + valueOnExternalRef - doubleBaseCorrection;
        return total;
    }
}
