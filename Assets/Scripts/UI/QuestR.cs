using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestR : MonoBehaviour
{
    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }

    public void AcceptCharacter(CharacterData characterData)
    {
        Game.instance.playerData.followerUid = characterData.characterUniqueId;

        OnClosePressed();
    }
}
