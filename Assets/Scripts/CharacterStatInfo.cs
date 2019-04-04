using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CharacterStatData
{
    public CharacterStatType stat;
    public Sprite icon;
    public string name;
    public string description;
}

public class CharacterStatInfo : MonoBehaviour
{
    public List<CharacterStatData> data;

    public CharacterStatData DataForStat(CharacterStatType stat)
    {
        for (int i = 0; i < data.Count; ++i)
        {
            if (stat == data[i].stat)
            {
                return data[i];
            }
        }

        return new CharacterStatData();
    }
}
