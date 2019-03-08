using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataList : MonoBehaviour
{
    public CharacterData[] characterData;

    public List<CharacterData> AllCharactersWithinLevelRange(int minLevel, int maxLevel)
    {
        List<CharacterData> characters = new List<CharacterData>();
        for (int i = 0; i < characterData.Length; ++i)
        {
            if (characterData[i].levelRequirement >= minLevel && characterData[i].levelRequirement <= maxLevel)
            {
                characters.Add(characterData[i]);
            }
        }
        return characters;
    }

    public CharacterData CharacterWithName(string name)
    {
        for (int i = 0; i < characterData.Length; ++i)
        {
            if (characterData[i].characterName == name)
            {
                return characterData[i];
            }
        }
        return null;
    }

    public CharacterData CharacterWithUID(string uid)
    {
        for (int i = 0; i < characterData.Length; ++i)
        {
            if (characterData[i].characterUniqueId == uid)
            {
                return characterData[i];
            }
        }
        return null;
    }
}
