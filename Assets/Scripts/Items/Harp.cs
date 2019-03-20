using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Harp : SingleUseItem
{
    protected override void OnUse()
    {
        CharacterDataList list = Game.instance.GetComponent<CharacterDataList>();
        List<CharacterData> characters = list.AllCharactersWithinLevelRange(0, 100);
        CharacterData chosen = characters.Sample();

        Game.instance.playerData.followerUid = chosen.characterUniqueId;
    }
}
