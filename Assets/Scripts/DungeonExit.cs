using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExit : MonoBehaviour
{
    private bool mIsTransitioning;

    private void OnTriggerEnter(Collider other)
    {
        if (mIsTransitioning)
            return;

        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsTransitioning = true;
            Game.instance.avatar.transitioning = true;
            Invoke("Transition", 0.5f);
        }
    }

    private void Transition()
    {
        Game.instance.RunEnded(true);

        Game.instance.transitionManager.TransitionToScreen("HUB");
    }
}
