using System.Collections;
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

    public delegate void CharacterStatisticsChanged(CharacterStatistics stats);
    public event CharacterStatisticsChanged onCharacterStatisticsChanged;

    public virtual int BaseStatValue(CharacterStatType statType)
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

    public virtual void ChangeBaseStat(CharacterStatType statType, int newValue)
    {
        if (statType == CharacterStatType.MaxHealth)
            _maxHealth = newValue;
        else if (statType == CharacterStatType.Strength)
            _strength = newValue;
        else if (statType == CharacterStatType.Defense)
            _defense = newValue;
        else if (statType == CharacterStatType.Magic)
            _magic = newValue;
        else if (statType == CharacterStatType.Speed)
            _speed = newValue;
        else
            _luck = newValue;

        if (onCharacterStatisticsChanged != null)
        {
            onCharacterStatisticsChanged(this);
        }
    }

    /// <summary>
    /// Returns a stat value including all equipment, perks, bonuses, etc that may apply.
    /// </summary>
    /// <param name="statType"></param>
    /// <returns></returns>
    public virtual int ModifiedStatValue(CharacterStatType statType, GameObject entity)
    {
        int value = BaseStatValue(statType);

        CharacterStatModifier[] modifiers = entity.GetComponentsInChildren<CharacterStatModifier>();

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

        PlayerController pc = entity.GetComponent<PlayerController>();
        if (pc != null)
        {
            // Apply relevant character stat boost
            if (Game.instance.playerData.followerUid != null)
            {
                CharacterData followerData = Game.instance.characterDataList.CharacterWithUID(Game.instance.playerData.followerUid);
                if (followerData != null && followerData.statBoost == statType)
                {
                    value += followerData.statBoostAmount;
                }
            }
        }

        return value;
    }

    public static CharacterStatType StatTypeFromString(string str)
    {
        if (str == "maxhealth")
            return CharacterStatType.MaxHealth;
        else if (str == "strength")
            return CharacterStatType.Strength;
        else if (str == "defense")
            return CharacterStatType.Defense;
        else if (str == "magic")
            return CharacterStatType.Magic;
        else if (str == "speed")
            return CharacterStatType.Speed;
        else if (str == "luck")
            return CharacterStatType.Luck;
        else
            return CharacterStatType.MaxHealth;
    }

    public bool IsItemEquipped<T>()
    {
        return GetComponentInChildren<T>() != null;
    }
}
