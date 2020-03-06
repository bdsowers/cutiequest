using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Cheats : MonoBehaviour
{
    public static int forceTestItemGeneration { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        forceTestItemGeneration = -1;
    }

    private void SkipTutorial()
    {
        Game.instance.cinematicDirector.EndAllCinematics();
        Game.instance.finishedTutorial = true;
        KillAvatar();
    }

    private void RevealMap()
    {
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }

    private void TeleportToDungeonExit()
    {
        LevelExit exit = GameObject.FindObjectOfType<LevelExit>();
        Game.instance.avatar.transform.position = exit.transform.position + Vector3.right;
    }

    private void ClearSavedFlags()
    {
        Game.instance.playerData.flags = new List<string>();
    }

    private void KillAvatar()
    {
        Game.instance.playerData.health = 1;
        Game.instance.avatar.GetComponent<Killable>().health = 1;
        Game.instance.avatar.GetComponent<Killable>().TakeDamage(null, 1, DamageReason.Trap);
    }

    private void TakeScreenshot()
    {
        int screenshotNum = PlayerPrefs.GetInt("screenshotnum", 1);
        ScreenCapture.CaptureScreenshot("C:\\Users\\bdsow\\Desktop\\QuestRScreenshots\\screen" + screenshotNum + ".png");

        ++screenshotNum;
        PlayerPrefs.SetInt("screenshotnum", screenshotNum);
    }

    private void SkipToBoss()
    {
        Game.instance.currentDungeonFloor = 4;
        LevelExit exit = GameObject.FindObjectOfType<LevelExit>();
        Game.instance.avatar.transform.position = exit.transform.position + Vector3.right;
    }

    private void EnterDebugDungeon()
    {
        Game.instance.EnterDungeon(Game.instance.debugDungeonData, null);
        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }

    private void UnlockAllDungeons()
    {
        Game.instance.playerData.SetFlag("cave_biome_revealed");
        Game.instance.playerData.SetFlag("scifi_biome_revealed");
        Game.instance.playerData.SetFlag("boss1_defeated");
        Game.instance.saveManager.TriggerSave();
    }

    public void AddCoins()
    {
        Game.instance.playerData.numCoins += 100;
    }

    public void AddHearts()
    {
        Game.instance.playerData.numHearts += 20;
    }

    public void NoDamage()
    {
        Game.instance.avatar.GetComponent<Killable>().invulnerable = true;
    }

    public void ResetAttractiveness()
    {
        Game.instance.playerData.attractiveness = 1;
    }

    public void BuildFullCompanionSet()
    {
        Game.instance.companionBuilder.BuildCheatCompanionSet();
        QuestR.seenMatches = false;
    }

    public void EnterPreview()
    {
        Game.instance.cinematicDirector.EndAllCinematics();

        Game.instance.ForcePreviewMode();
    }

    public void ResetGame()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Game.instance.CloseGame();
    }

    // Update is called once per frame
    void Update()
    {
        // Cheat that works even in builds to clear out save data
        // todo bdsowers - provide a first-class route for this
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.Escape))
        {
            ResetGame();
        }

#if DISABLE_CHEATS
        return;
#endif
        if (Input.GetKeyDown(KeyCode.C))
        {
            CheatsDialog dialog = Game.instance.hud.cheatsDialog;
            if (!dialog.hasBeenPopulated)
            {
                dialog.hasBeenPopulated = true;
                PopulateCheatsDialog();
            }

            Game.instance.hud.cheatsDialog.gameObject.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot();
        }
    }

    void UnlockAllNPCS()
    {
        Game.instance.playerData.SetFlag("hotdogman");
        Game.instance.playerData.SetFlag("punkypeter");
        Game.instance.playerData.SetFlag("trainer");
        Game.instance.playerData.SetFlag("stylist");
        Game.instance.playerData.SetFlag("tourist");
        Game.instance.playerData.SetFlag("beats");
        Game.instance.playerData.SetFlag("bruiser");
    }

    void TestSpecificItem()
    {
        forceTestItemGeneration = (forceTestItemGeneration + 1) % (PrefabManager.instance.itemPrefabs.Length);
    }

    void CloseCheatDialog()
    {
        Game.instance.hud.cheatsDialog.gameObject.SetActive(false);
    }

    void CreateTestingBackup()
    {
        Game.instance.saveManager.SaveTestingBackup();
    }

    void LoadTestingBackup()
    {
        Game.instance.saveManager.LoadTestingBackup();
    }

    void KillEnemies()
    {
        Killable[] allKillables = GameObject.FindObjectsOfType<Killable>();
        for (int i = 0; i < allKillables.Length; ++i)
        {
            if (allKillables[i].gameObject != Game.instance.avatar.gameObject)
            {
                allKillables[i].TakeDamage(null, 99999999, DamageReason.ForceKill);
            }
        }
    }

    void PopulateCheatsDialog()
    {
        CheatsDialog dialog = Game.instance.hud.cheatsDialog;

        dialog.AddButton("Close", CloseCheatDialog, "");
        dialog.AddButton("Unlock Dungeons", UnlockAllDungeons, "");
        dialog.AddButton("Reveal Map", RevealMap, "");
        dialog.AddButton("Dungeon Exit", TeleportToDungeonExit, "");
        dialog.AddButton("Add Coins", AddCoins, "");
        dialog.AddButton("Add Hearts", AddHearts, "");
        dialog.AddButton("Kill Avatar", KillAvatar, "");
        dialog.AddButton("Skip to Boss", SkipToBoss, "");
        dialog.AddButton("No Damage", NoDamage, "");
        dialog.AddButton("Debug Dungeon", EnterDebugDungeon, "");
        dialog.AddButton("Companions", BuildFullCompanionSet, "");
        dialog.AddButton("Reset Game", ResetGame, "");
        dialog.AddButton("Unlock NPCS", UnlockAllNPCS, "");
        dialog.AddButton("Test Item", TestSpecificItem, "");
        dialog.AddButton("Save Backup", CreateTestingBackup, "");
        dialog.AddButton("Load Backup", LoadTestingBackup, "");
        dialog.AddButton("Kill Enemies", KillEnemies, "");
    }
}
