using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class HUB : MonoBehaviour
{
    private IEnumerator Start()
    {
        Game.instance.companionBuilder.BuildCompanionSet();

        Game.instance.playerData.health = Game.instance.playerStats.ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        Game.instance.playerData.numCoins = 0;
        Game.instance.playerStats.gameObject.RemoveAllChildren();
        yield return null;
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        // todo bdsowers - show the phone intro first
        Game.instance.finishedTutorial = true;

        Game.instance.saveManager.TriggerSave();
    }
}
