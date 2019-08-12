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

        ResetCharacter();

        yield return null;
        Game.instance.playerData.health = Game.instance.avatar.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        yield return null;
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();
        
        if (!Game.instance.finishedTutorial)
        {
            Game.instance.cinematicDirector.PostCinematicEvent("hub_tutorial");
            Game.instance.finishedTutorial = true;
        }

        Game.instance.saveManager.TriggerSave();

        Game.instance.cinematicDirector.PostCinematicEvent("Hub_Reload");

        Game.instance.soundManager.PlayRandomMusicInCategory("TownMusic");
    }

    private void ResetCharacter()
    {
        Game.instance.playerData.numCoins = 0;
        Game.instance.playerStats.gameObject.RemoveAllChildren();

        CharacterStatModifier[] modifiers = Game.instance.playerStats.GetComponentsInChildren<CharacterStatModifier>();
        for (int i = 0; i < modifiers.Length; ++i)
        {
            Destroy(modifiers[i]);
        }
    }
}
