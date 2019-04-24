using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDataList : MonoBehaviour
{
    public CharacterData[] characterData;
    public CharacterModelData[] characterModelData;

    public List<CharacterModelData> CharacterModelsWithGender(int gender)
    {
        List<CharacterModelData> models = new List<CharacterModelData>();
        for (int i = 0; i < characterModelData.Length; ++i)
        {
            if (characterModelData[i].gender == gender)
            {
                models.Add(characterModelData[i]);
            }
        }

        return models;
    }

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
