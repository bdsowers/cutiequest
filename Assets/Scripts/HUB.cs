using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUB : MonoBehaviour
{
    private void Start()
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

        // todo bdsowers - show the phone intro first
        Game.instance.finishedTutorial = true;
    }
}
