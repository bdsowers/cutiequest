using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineRiches : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            NumberPopupGenerator.instance.GeneratePopup(gameObject, Game.instance.playerData.numCoins, NumberPopupReason.RemoveCoins);
            Game.instance.playerData.numCoins = 0;

            CharacterData followerData = Game.instance.characterDataList.CharacterWithUID(Game.instance.playerData.followerUid);
            followerData.statBoostAmount *= 2;
        }

        yield break;
    }
}
