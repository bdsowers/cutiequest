using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Cheats : MonoBehaviour
{
    private int mCurrentActivationPlate = 0;

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
    }
}
