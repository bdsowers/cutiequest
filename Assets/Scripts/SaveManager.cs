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

    // TODO bdsowers : One day we'll want all saving/loading to be file-based in a robust way,
    // but today is not that day.
    private void SaveToFile(string fileName)
    {
        List<string> lines = new List<string>();

        lines.Add("1"); // SaveVersion

        // Follower info (which isn't actually loaded)
        if (Game.instance.playerData.followerUid != null)
            lines.Add(Game.instance.playerData.followerUid);
        else
            lines.Add("null");

        lines.Add(Game.instance.playerData.model);
        lines.Add(Game.instance.playerData.material);
        lines.Add(Game.instance.playerData.numHearts.ToString());
        lines.Add(Game.instance.playerData.numCoins.ToString());
        lines.Add(Game.instance.playerData.attractiveness.ToString());
        lines.Add((Game.instance.finishedTutorial ? "1" : "0"));
        lines.Add(Game.instance.playerData.scoutLevel.ToString());

        string flagsStr = string.Join(" ", Game.instance.playerData.flags);
        lines.Add(flagsStr);

        foreach (KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
        {
            lines.Add(stat.Key + " " + Game.instance.playerStats.BaseStatValue(stat.Value).ToString());
        }

        System.IO.File.WriteAllLines(fileName, lines.ToArray());
    }

    private void LoadFromFile(string fileName)
    {
        mSaveDisabled = true;

        try
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);

            // lines[0] = SaveVersion
            // lines[1] = Follower UID (unused)
            string playerModel = lines[2];
            string playerMaterial = lines[3];
            int hearts = int.Parse(lines[4]);
            int coins = int.Parse(lines[5]);
            int attractiveness = int.Parse(lines[6]);
            bool finishedTutorial = (lines[7] == "1");
            int scoutLevel = int.Parse(lines[8]);
            string allFlags = lines[9];
            string[] splitFlags = allFlags.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

            int currentLine = 10;
            Dictionary<string, int> stats = new Dictionary<string, int>();
            for (int i = 0; i < mStatKeyMap.Count; ++i)
            {
                string statPair = lines[currentLine];
                string[] statTokens = statPair.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                stats.Add(statTokens[0], int.Parse(statTokens[1]));

                ++currentLine;
            }

            Game.instance.playerData.model = playerModel;
            Game.instance.playerData.material = playerMaterial;
            Game.instance.playerData.numHearts = hearts;
            Game.instance.playerData.numCoins = coins;
            Game.instance.playerData.attractiveness = attractiveness;
            Game.instance.finishedTutorial = finishedTutorial;
            Game.instance.playerData.scoutLevel = scoutLevel;

            Game.instance.cinematicDataProvider.Reset();

            foreach(string flag in splitFlags)
            {
                Game.instance.cinematicDataProvider.SetData(flag, "true");
            }

            foreach (KeyValuePair<string, CharacterStatType> stat in mStatKeyMap)
            {
                int value = stats[stat.Key];
                Game.instance.playerStats.ChangeBaseStat(stat.Value, value);
            }
        }
        catch (System.Exception) { }

        mSaveDisabled = false;
    }

    public void SaveTestingBackup()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "testing_backup.txt");
        SaveToFile(path);
    }

    public void LoadTestingBackup()
    {
        string path = System.IO.Path.Combine(Application.persistentDataPath, "testing_backup.txt");
        LoadFromFile(path);

        SaveGame();
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
        PlayerPrefs.SetInt("ScoutLevel", Game.instance.playerData.scoutLevel);

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
            Game.instance.playerData.attractiveness = PlayerPrefs.GetInt("Attractiveness", 1);
            Game.instance.finishedTutorial = PlayerPrefs.GetInt("FinishedTutorial") == 1;
            Game.instance.playerData.scoutLevel = PlayerPrefs.GetInt("ScoutLevel", 0);

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
