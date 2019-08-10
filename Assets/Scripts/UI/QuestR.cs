using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using UnityEngine.UI;

public class QuestR : Dialog
{
    public GameObject contentsContainer;
    public Image backgroundShadow;

    public GameObject standardView;
    public GameObject matchView;

    public CharacterModel rigModel1;
    public CharacterModel rigModel2;

    public QuestRPanel panel1;
    public QuestRPanel panel2;

    public bool moreInfoMode { get; set; }

    public static bool seenMatches { get; set; }

    private bool mTutorialMode;

    public bool tutorialMode {  get { return mTutorialMode; } }

    public bool joystickNeedsReset { get; set; }

    public void SetTutorialMode(bool tutorialMode)
    {
        if (tutorialMode)
        {
            contentsContainer.transform.localPosition = Vector3.up * 120f;
            backgroundShadow.enabled = false;
            mTutorialMode = true;
        }
        else
        {
            contentsContainer.transform.localPosition = Vector3.zero;
            backgroundShadow.enabled = true;
            mTutorialMode = false;
        }
    }

    private void OnEnable()
    {
        List<CharacterData> characters = Game.instance.GetComponent<CharacterDataList>().AllCharactersWithinLevelRange(0, Game.instance.playerData.attractiveness);
        characters.Shuffle();

        panel1.characterOffset = (panel1.isFrontPanel ? 0 : 1);
        panel2.characterOffset = (panel2.isFrontPanel ? 0 : 1);

        panel1.availableCharacters = characters;
        panel2.availableCharacters = characters;
        panel1.Setup();
        panel2.Setup();

        standardView.SetActive(true);
        matchView.SetActive(false);

        DisableButtonNavigation();

        seenMatches = true;
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
        if (matchView.gameObject.activeSelf)
        {
            if (Game.instance.followerData != null)
            {
                string spellName = LocalizedText.Get(Game.instance.followerData.spell.friendlyName);
                string quirkName = LocalizedText.Get(Game.instance.followerData.quirk.friendlyName);

                NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, "Spell: " + spellName, NumberPopupReason.Heal, 0f);
                NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.transform.position + Vector3.up * 0.7f, "Quirk: " + quirkName, NumberPopupReason.Heal, 0.9f);
            }
        }

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

        rigModel1.ChangeModel(Game.instance.playerData.model);
        rigModel2.ChangeModel(characterData);

        Game.instance.soundManager.PlaySound("confirm_special");
    }
}
