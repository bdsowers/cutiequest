using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestR : MonoBehaviour
{
    public GameObject standardView;
    public GameObject matchView;

    public CharacterModel rigModel1;
    public CharacterModel rigModel2;

    private void OnEnable()
    {
        standardView.SetActive(true);
        matchView.SetActive(false);
    }

    public void OnClosePressed()
    {
        gameObject.SetActive(false);
    }

    public void AcceptCharacter(CharacterData characterData)
    {
        Game.instance.playerData.followerUid = characterData.characterUniqueId;

        matchView.SetActive(true);
        standardView.SetActive(false);

        rigModel1.ChangeModel("Chr_Adventure_Warrior_01");
        rigModel2.ChangeModel(characterData.model);
    }
}
