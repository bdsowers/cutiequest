using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterQuirk
{
    Boring,
    IceQueen,
    ShortSighted,
}

[CreateAssetMenu(fileName = "Character", menuName = "CutieQuest/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public string characterUniqueId;
    public string model;
    public int age;
    public string tagline;

    public int levelRequirement;

    public Spell spell;
    public CharacterQuirk quirk;
    public CharacterStatType statBoost;
    public int statBoostAmount;
}
