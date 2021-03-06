﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrineSupplyCart : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            NumberPopupGenerator.instance.GeneratePopup(gameObject, Game.instance.playerData.numCoins, NumberPopupReason.RemoveCoins);
            Game.instance.playerData.numCoins = 0;

            int maxHealth = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
            Game.instance.playerData.health = maxHealth;
            Game.instance.avatar.GetComponent<Killable>().health = maxHealth;

            NumberPopupGenerator.instance.GeneratePopup(gameObject, maxHealth, NumberPopupReason.Good, 0.4f);
        }

        yield break;
    }
}
