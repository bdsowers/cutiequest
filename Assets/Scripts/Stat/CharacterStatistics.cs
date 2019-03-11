﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterStatType
{
    MaxHealth,
    Strength,
    Defense,
    Magic,
    Speed,
    Luck,
}

public class CharacterStatistics : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth;

    [SerializeField]
    private int _strength;

    [SerializeField]
    private int _defense;

    [SerializeField]
    private int _magic;

    [SerializeField]
    private int _speed;

    [SerializeField]
    private int _luck;

    /// <summary>
    /// Returns a stat value including all equipment, perks, bonuses, etc that may apply.
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public virtual int ModifiedStatValue(CharacterStatType statType)
    {
        int value = 0;
        if (statType == CharacterStatType.MaxHealth)
            value = _maxHealth;
        else if (statType == CharacterStatType.Strength)
            value = _strength;
        else if (statType == CharacterStatType.Defense)
            value = _defense;
        else if (statType == CharacterStatType.Magic)
            value = _magic;
        else if (statType == CharacterStatType.Speed)
            value = _speed;
        else
            value = _luck;

        return value;
    }
}