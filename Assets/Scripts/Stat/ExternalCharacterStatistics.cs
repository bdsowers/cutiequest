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
        return externalReference.ModifiedStatValue(statType);
    }
}
