using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using UnityEngine.UI;

public class QuestR : Dialog
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

        DisableButtonNavigation();
    }

    private void DisableButtonNavigation()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        for (int i = 0; i < buttons.Length; ++i)
        {
            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.None;
            buttons[i].navigation = nav;
        }
    }

    public void OnClosePressed()
    {
        Close();
    }

    public override void Update()
    {
        if (Game.instance.actionSet.CloseMenu.WasPressed)
        {
            Close();
        }

        if (matchView.activeSelf && Game.instance.actionSet.Activate.WasPressed)
        {
            Close();
        }
    }

    public void AcceptCharacter(CharacterData characterData)
    {
        Game.instance.playerData.followerUid = characterData.characterUniqueId;

        matchView.SetActive(true);
        standardView.SetActive(false);

        rigModel1.ChangeModel("Chr_Adventure_Warrior_01");
        rigModel2.ChangeModel(characterData);
    }
}
