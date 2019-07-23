using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class HUB : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return null;
        Game.instance.companionBuilder.BuildCompanionSet();
        QuestR.seenMatches = false;

        Game.instance.playerData.numCoins = 0;
        Game.instance.playerStats.gameObject.RemoveAllChildren();
        yield return null;
        Game.instance.playerData.health = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        yield return null;
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        // todo bdsowers - show the phone intro first
        Game.instance.finishedTutorial = true;

        Game.instance.saveManager.TriggerSave();

        Game.instance.soundManager.PlayRandomMusicInCategory("TownMusic");
    }
}
