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

    [TextArea(5, 10)]
    public string bio;

    public int levelRequirement;

    public Spell spell;
    public Quirk quirk;
    public CharacterStatType statBoost;
    public int statBoostAmount;

    public Material material;
}
