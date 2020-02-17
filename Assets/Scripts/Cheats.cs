using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Cheats : MonoBehaviour
{
    private bool mDialogPopulated;

    // Start is called before the first frame update
    void Start()
    {

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
        Game.instance.avatar.GetComponent<Killable>().TakeDamage(1);
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
        Game.instance.saveManager.TriggerSave();
    }

    public void AddCoins()
    {
        Game.instance.playerData.numCoins += 100;
    }

    public void AddHearts()
    {
        Game.instance.playerData.numHearts += 2;
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

#if (RELEASE || DISABLE_CHEATS) && !UNITY_EDITOR
        return;
#endif
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (!mDialogPopulated)
            {
                mDialogPopulated = true;
                PopulateCheatsDialog();
            }

            Game.instance.hud.cheatsDialog.gameObject.SetActive(true);
        }

        // Cinematic testing cheat
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Game.instance.cinematicDirector.dataProvider.SetData("boss1_defeated", "true");
            Game.instance.cinematicDirector.PostCinematicEvent("Hub_Reload");
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SkipTutorial();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RevealMap();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            TeleportToDungeonExit();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            AddCoins();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            AddHearts();
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            ClearSavedFlags();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            KillAvatar();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TakeScreenshot();
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            SkipToBoss();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            NoDamage();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ResetAttractiveness();
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            EnterDebugDungeon();
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            BuildFullCompanionSet();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            EnterPreview();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UnlockAllDungeons();
        }
    }

    void CloseCheatDialog()
    {
        Game.instance.hud.cheatsDialog.gameObject.SetActive(false);
    }

    void PopulateCheatsDialog()
    {
        CheatsDialog dialog = Game.instance.hud.cheatsDialog;

        dialog.AddButton("Close", CloseCheatDialog, "");
        dialog.AddButton("Unlock Dungeons", UnlockAllDungeons, "T");
        dialog.AddButton("Skip Tutorial", SkipTutorial, "Q");
        dialog.AddButton("Reveal Map", RevealMap, "W");
        dialog.AddButton("Dungeon Exit", TeleportToDungeonExit, "R");
        dialog.AddButton("Add Coins", AddCoins, "Y");
        dialog.AddButton("Add Hearts", AddHearts, "U");
        dialog.AddButton("Clear Flags", ClearSavedFlags, "O");
        dialog.AddButton("Kill Avatar", KillAvatar, "A");
        dialog.AddButton("Screenshot", TakeScreenshot, "S");
        dialog.AddButton("Skip to Boss", SkipToBoss, "H");
        dialog.AddButton("No Damage", NoDamage, "F");
        dialog.AddButton("Reset Attract", ResetAttractiveness, "G");
        dialog.AddButton("Debug Dungeon", EnterDebugDungeon, "/");
        dialog.AddButton("Companions", BuildFullCompanionSet, "\\");
        dialog.AddButton("Preview", EnterPreview, "P");
        dialog.AddButton("Reset Game", ResetGame, "APM");
    }
}
