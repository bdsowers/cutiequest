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
        Follower follower = GameObject.FindObjectOfType<Follower>();
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(characterData.model);

        Game.instance.playerData.followerUid = characterData.characterUniqueId;

        OnClosePressed();
    }
}
