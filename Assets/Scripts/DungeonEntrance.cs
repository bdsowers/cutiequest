using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonEntrance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
        Game.instance.playerData.numHearts = 0;

        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }
}
