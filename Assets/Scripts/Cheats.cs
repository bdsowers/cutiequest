using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Cheats : MonoBehaviour
{
    private int mCurrentActivationPlate = 0;

    private int mCheatCharacter = 0;
    private int mCheatShrine = 0;

    private int mScreenshotNum = 1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Game.instance.transitionManager.TransitionToScreen("Dungeon");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
            for (int i = 0; i  < revealers.Length; ++i)
            {
                revealers[i].Reveal();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            LevelExit exit = GameObject.FindObjectOfType<LevelExit>();
            Game.instance.avatar.transform.position = exit.transform.position + Vector3.right;
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            ActivationPlate[] activationPlates = GameObject.FindObjectsOfType<ActivationPlate>();
            if (activationPlates.Length > 0)
            {
                Game.instance.avatar.transform.position = activationPlates[mCurrentActivationPlate].transform.position + Vector3.up;
                mCurrentActivationPlate = (mCurrentActivationPlate + 1) % activationPlates.Length;
            }
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
            Game.instance.playerData.flags = new List<string>();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Game.instance.playerData.health = 1;
            Game.instance.avatar.GetComponent<Killable>().health = 1;
            Game.instance.avatar.GetComponent<Killable>().TakeDamage(1);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            mScreenshotNum = PlayerPrefs.GetInt("screenshotnum", 1);
            ScreenCapture.CaptureScreenshot("C:\\Users\\bdsow\\Desktop\\QuestRScreenshots\\screen" + mScreenshotNum + ".png");

            ++mScreenshotNum;
            PlayerPrefs.SetInt("screenshotnum", mScreenshotNum);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            Game.instance.currentDungeonFloor = 4;
            LevelExit exit = GameObject.FindObjectOfType<LevelExit>();
            Game.instance.avatar.transform.position = exit.transform.position + Vector3.right;
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
            Game.instance.EnterDungeon(Game.instance.debugDungeonData);
            Game.instance.transitionManager.TransitionToScreen("Dungeon");
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            Game.instance.companionBuilder.BuildCheatCompanionSet();
            QuestR.seenMatches = false;
        }
    }
}
