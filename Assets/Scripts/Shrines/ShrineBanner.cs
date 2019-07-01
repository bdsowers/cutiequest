using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineBanner : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            Game.instance.currentDungeonFloor = 5;
            Game.instance.transitionManager.TransitionToScreen("Dungeon");
        }

        yield break;
    }
}
