﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private bool mIsTransitioning;

    private void OnTriggerEnter(Collider other)
    {
        if (mIsTransitioning)
            return;

        if (other.GetComponentInParent<PlayerController>() != null)
        {
            if (CheckCompletionistQuirk())
                return;

            mIsTransitioning = true;
            Game.instance.avatar.transitioning = true;
            Invoke("Transition", 0.5f);
        }
    }

    private bool CheckCompletionistQuirk()
    {
        if (Game.instance.quirkRegistry.IsQuirkActive<CompletionistQuirk>())
        {
            RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
            for (int i = 0; i < revealers.Length; ++i)
            {
                if (!revealers[i].fullyRevealed)
                {
                    NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, "Not until we see everything!", NumberPopupReason.Bad);
                    return true;
                }
            }
        }

        return false;
    }

    private void Transition()
    {
        Game.instance.soundManager.PlaySound("warp");

        Game.instance.currentDungeonFloor++;

        Game.instance.transitionManager.TransitionToScreen("Dungeon");
    }
}
