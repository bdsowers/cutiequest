using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cheats : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Game.instance.transitionManager.TransitionToScreen("SampleScene");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
            for (int i = 0; i  < revealers.Length; ++i)
            {
                revealers[i].Reveal();
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            LevelExit exit = GameObject.FindObjectOfType<LevelExit>();
            Game.instance.avatar.transform.position = exit.transform.position + Vector3.right;
        }
    }
}
