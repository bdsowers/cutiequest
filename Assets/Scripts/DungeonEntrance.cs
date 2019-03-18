using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
    public DungeonData dungeonData;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            EnterDungeon();
        }
    }

    private void EnterDungeon()
    {
        // todo bdsowers - if the player has hearts left, we need to ask them if they really
        // want to dungeon dive before entering and resetting their hearts.
        Game.instance.EnterDungeon(dungeonData);

        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }
}
