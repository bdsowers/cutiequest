using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class QuestR : MonoBehaviour
{
    public GameObject standardView;
    public GameObject matchView;

    public CharacterModel rigModel1;
    public CharacterModel rigModel2;

    public QuestRPanel panel1;
    public QuestRPanel panel2;

    private void OnEnable()
    {
        List<CharacterData> characters = Game.instance.GetComponent<CharacterDataList>().AllCharactersWithinLevelRange(0, Game.instance.playerData.attractiveness);
        characters.Shuffle();

        panel1.availableCharacters = characters;
        panel2.availableCharacters = characters;
        panel1.Setup();
        panel2.Setup();

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
