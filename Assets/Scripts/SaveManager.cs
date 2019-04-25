using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private Dictionary<string, CharacterStatType> mStatKeyMap = new Dictionary<string, CharacterStatType>()
    {
        { "MaxHealth", CharacterStatType.MaxHealth },
        { "Strength", CharacterStatType.Strength },
        { "Defense", CharacterStatType.Defense },
        { "Magic", CharacterStatType.Magic },
        { "Speed", CharacterStatType.Speed },
        { "Luck", CharacterStatType.Luck },
    };

    private bool mSaveTriggered = false;

    private void Update()
    {
        if (!mSaveTriggered)
            return;

        mSaveTriggered = false;
        SaveGame();
    }

    public void TriggerSave()
    {
        mSaveTriggered = true;
    }

    public void SaveGame()
    {
        Debug.Log("Saving game...");

        // todo bdsowers - shoving this all in PlayerPrefs is def bad practice
        // but for now it's sufficient. This game doesn't really have a lot of data that needs
        // saving atm.
        PlayerPrefs.SetInt("SaveVersion", 1);
        
        if (Game.instance.playerData.followerUid != null)
        {
            PlayerPrefs.SetString("FollowerID", Game.instance.playerData.followerUid);
        }

        PlayerPrefs.SetInt("Hearts", Game.instance.playerData.numHearts);
        PlayerPrefs.SetInt("Coins", Game.instance.playerData.numCoins);
        PlayerPrefs.SetInt("Attractiveness", Game.instance.playerData.attractiveness);

        foreach(KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
        {
            PlayerPrefs.SetInt(stat.Key, Game.instance.playerStats.BaseStatValue(stat.Value));
        }
    }

    public void LoadGame()
    {
        int saveVersion = PlayerPrefs.GetInt("SaveVersion", 0);
        
        if (saveVersion == 1)
        {
            Game.instance.playerData.followerUid = PlayerPrefs.GetString("FollowerID", null);
            Game.instance.playerData.numHearts = PlayerPrefs.GetInt("Hearts");
            Game.instance.playerData.numCoins = PlayerPrefs.GetInt("Coins");
            Game.instance.playerData.attractiveness = PlayerPrefs.GetInt("Attractiveness");

            foreach (KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
            {
                int value = PlayerPrefs.GetInt(stat.Key);
                Game.instance.playerStats.ChangeBaseStat(stat.Value, value);
            }

            // todo bdsowers - this is causing significant issues
            //Game.instance.playerData.health = Game.instance.playerStats.ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        }
    }
}
