using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatType
{
    Strength,
    Defense,
    Magic,
    Speed,
    Luck,
}

public class CharacterStatistics : MonoBehaviour
{
    public int strength;
    public int defense;
    public int magic;
    public int speed;
    public int luck;

    /// <summary>
    /// Returns a stat value including all equipment, perks, bonuses, etc that may apply.
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public int ModifiedStatValue(CharacterStatType statType)
    {
        int value = 0;
        if (statType == CharacterStatType.Strength)
            value = strength;
        else if (statType == CharacterStatType.Defense)
            value = defense;
        else if (statType == CharacterStatType.Magic)
            value = magic;
        else if (statType == CharacterStatType.Speed)
            value = speed;
        else
            value = luck;

        return value;
    }
}
