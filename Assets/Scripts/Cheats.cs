﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Cheats : MonoBehaviour
{
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
        Game.instance.EnterDungeon(Game.instance.debugDungeonData);
        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }

    private void UnlockAllDungeons()
    {
        Game.instance.playerData.SetFlag("cave_biome_revealed");
    }

    // Update is called once per frame
    void Update()
    {
        // Cheat that works even in builds to clear out save data
        // todo bdsowers - provide a first-class route for this
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.M) && Input.GetKey(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Game.instance.CloseGame();
        }

#if (RELEASE || DISABLE_CHEATS) && !UNITY_EDITOR
        return;
#endif
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
            Game.instance.playerData.numCoins += 100;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Game.instance.playerData.numHearts += 2;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
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
            Game.instance.avatar.GetComponent<Killable>().invulnerable = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            Game.instance.playerData.attractiveness = 1;
        }

        if (Input.GetKeyDown(KeyCode.Slash))
        {
            EnterDebugDungeon();
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            Game.instance.companionBuilder.BuildCheatCompanionSet();
            QuestR.seenMatches = false;
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Game.instance.cinematicDirector.EndAllCinematics();

            Game.instance.ForcePreviewMode();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UnlockAllDungeons();
            Game.instance.saveManager.TriggerSave();
        }
    }
}
