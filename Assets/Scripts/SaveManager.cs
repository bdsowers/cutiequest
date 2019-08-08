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
    private bool mSaveDisabled = false;

    private void Update()
    {
        if (!mSaveTriggered)
            return;

        mSaveTriggered = false;
        SaveGame();
    }

    public void TriggerSave()
    {
        if (mSaveDisabled)
            return;

        mSaveTriggered = true;
    }

    public void SaveGame()
    {
        if (mSaveDisabled)
            return;

        Debug.Log("Saving game...");

        // todo bdsowers - shoving this all in PlayerPrefs is def bad practice
        // but for now it's sufficient. This game doesn't really have a lot of data that needs
        // saving atm.
        PlayerPrefs.SetInt("SaveVersion", 1);
        
        if (Game.instance.playerData.followerUid != null)
        {
            PlayerPrefs.SetString("FollowerID", Game.instance.playerData.followerUid);
        }

        PlayerPrefs.SetString("PlayerModel", Game.instance.playerData.model);
        PlayerPrefs.SetString("PlayerMaterial", Game.instance.playerData.material);
        PlayerPrefs.SetInt("Hearts", Game.instance.playerData.numHearts);
        PlayerPrefs.SetInt("Coins", Game.instance.playerData.numCoins);
        PlayerPrefs.SetInt("Attractiveness", Game.instance.playerData.attractiveness);
        PlayerPrefs.SetInt("FinishedTutorial", Game.instance.finishedTutorial ? 1 : 0);

        string flagsStr = string.Join(" ", Game.instance.playerData.flags);
        PlayerPrefs.SetString("Flags", flagsStr);

        foreach(KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
        {
            PlayerPrefs.SetInt(stat.Key, Game.instance.playerStats.BaseStatValue(stat.Value));
        }
    }

    public void LoadGame()
    {
        mSaveDisabled = true;

        int saveVersion = PlayerPrefs.GetInt("SaveVersion", 0);
        
        if (saveVersion == 1)
        {
            Game.instance.playerData.model = PlayerPrefs.GetString("PlayerModel", null);
            Game.instance.playerData.material = PlayerPrefs.GetString("PlayerMaterial", null);
            Game.instance.playerData.numHearts = PlayerPrefs.GetInt("Hearts");
            Game.instance.playerData.numCoins = PlayerPrefs.GetInt("Coins");
            Game.instance.playerData.attractiveness = PlayerPrefs.GetInt("Attractiveness");
            Game.instance.finishedTutorial = PlayerPrefs.GetInt("FinishedTutorial") == 1;

            string flagsStr = PlayerPrefs.GetString("Flags", "");
            
            string[] flags = flagsStr.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            Game.instance.playerData.flags = new List<string>(flags);
            
            foreach(string flag in flags)
            {
                Game.instance.cinematicDataProvider.SetData(flag, "true");
            }

            foreach (KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
            {
                int value = PlayerPrefs.GetInt(stat.Key);
                Game.instance.playerStats.ChangeBaseStat(stat.Value, value);
            }
        }
        else
        {
            Game.instance.NewGame();
        }

        mSaveDisabled = false;
    }
}
