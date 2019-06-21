using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
    public DungeonData dungeonData;
    public GameObject visualDungeonEntrance;

    private bool mActive = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            if (Game.instance.followerData != null)
            {
                EnterDungeon();
            }
        }
    }

    private void EnterDungeon()
    {
        // todo bdsowers - if the player has hearts left, we need to ask them if they really
        // want to dungeon dive before entering and resetting their hearts.
        Game.instance.EnterDungeon(dungeonData);

        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }

    private void Update()
    {
        if (mActive && Game.instance.followerData == null)
        {
            mActive = false;
            visualDungeonEntrance.SetActive(true);
        }

        if (!mActive && Game.instance.followerData != null)
        {
            mActive = true;
            visualDungeonEntrance.SetActive(false);
        }
    }
}
