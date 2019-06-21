using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class HUB : MonoBehaviour
{
    private IEnumerator Start()
    {
        Game.instance.playerData.followerUid = null;

        int numCharacters = Random.Range(3, 6);
        CharacterData[] characters = new CharacterData[numCharacters];
        for (int i = 0; i < numCharacters; ++i)
        {
            CharacterData character = Game.instance.companionBuilder.BuildRandomCharacter();
            characters[i] = character;
        }

        Game.instance.characterDataList.characterData = characters;

        Game.instance.playerData.health = Game.instance.playerStats.ModifiedStatValue(CharacterStatType.MaxHealth, Game.instance.avatar.gameObject);
        Game.instance.playerData.numCoins = 0;
        Game.instance.playerStats.gameObject.RemoveAllChildren();
        yield return null;
        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();

        // todo bdsowers - show the phone intro first
        Game.instance.finishedTutorial = true;
    }
}
