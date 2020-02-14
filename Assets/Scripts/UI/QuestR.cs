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

    public GameObject matchMakerContainer;
    public GameObject personalContainer;
    public GameObject mapContainer;
    public GameObject inventoryContainer;

    public Image[] tabs;
    public Image[] tabIcons;
    private int mSelectedTab = 0;

    public GameObject frontCameraRig;

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
        rigModel1.transform.localPosition = Vector3.zero;
        rigModel2.transform.localPosition = Vector3.zero;

        List<CharacterData> characters = Game.instance.GetComponent<CharacterDataList>().AllCharactersWithinLevelRange(0, Game.instance.playerData.attractiveness);
        characters.Shuffle();

        panel1.isFrontPanel = panel1.transform.GetSiblingIndex() > panel2.transform.GetSiblingIndex();
        panel2.isFrontPanel = !panel1.isFrontPanel;

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

        UpdateTabVisuals();
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
        if (!Game.instance.finishedTutorial)
            return;

        if (matchView.gameObject.activeSelf)
        {
            if (Game.instance.followerData != null)
            {
                string spellName = LocalizedText.Get(Game.instance.followerData.spell.friendlyName);
                string quirkName = LocalizedText.Get(Game.instance.followerData.quirk.friendlyName);

                spellName = PigLatinQuirk.ApplyQuirkIfPresent(spellName);
                quirkName = PigLatinQuirk.ApplyQuirkIfPresent(quirkName);

                NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, "Spell: " + spellName, NumberPopupReason.Good, 0f);
                NumberPopupGenerator.instance.GeneratePopup(Game.instance.avatar.gameObject, "Quirk: " + quirkName, NumberPopupReason.Good, 0.7f);
            }
        }

        Close();
    }

    public void SetTab(int tab)
    {
        int prev = mSelectedTab;
        mSelectedTab = Mathf.Clamp(tab, 0, VisibleTabs() - 1);

        if (prev != mSelectedTab)
        {
            UpdateTabVisuals();
        }
    }

    public override void Update()
    {
        if (panel1.isAnimating || panel2.isAnimating)
            return;

        if (Game.instance.actionSet.CloseMenu.WasPressed)
        {
            Close();
        }

        if (matchView.activeSelf && Game.instance.actionSet.Activate.WasPressed)
        {
            Close();
        }

        // Don't allow tab switching if we're in match view
        if (matchView.activeSelf)
            return;

        if (Game.instance.actionSet.MoveDown.WasPressed && !tutorialMode)
        {
            SetTab(mSelectedTab + 1);
        }
        else if (Game.instance.actionSet.MoveUp.WasPressed && !tutorialMode)
        {
            SetTab(mSelectedTab - 1);
        }
    }

    private int VisibleTabs()
    {
        int sum = 0;
        for (int i = 0; i < tabs.Length; ++i)
        {
            if (tabs[i].gameObject.activeSelf)
            {
                ++sum;
            }
        }
        return sum;
    }
    public void AcceptCharacter(CharacterData characterData)
    {
        Game.instance.playerData.followerUid = characterData.characterUniqueId;

        matchView.SetActive(true);
        standardView.SetActive(false);

        rigModel1.ChangeModel(Game.instance.playerData.model);
        rigModel2.ChangeModel(characterData);

        Game.instance.soundManager.PlaySound("confirm_special");

        UpdateTabVisuals();
    }

    private void UpdateTabVisuals()
    {
        // In Match View, hide all tabs and prevent moving away ...
        for (int i = 0; i < tabs.Length; ++i)
        {
            tabs[i].gameObject.SetActive(!matchView.gameObject.activeSelf);
        }

        // NOTE bdsowers - was intending on implementing these two tabs
        // but it leads to some weird use-cases and ends up more trouble than it's
        // worth. Keeping a rough pathway in in case I ever change my mind (but I doubt it).
        tabs[3].gameObject.SetActive(false);

        if (!Game.instance.InDungeon())
        {
            tabs[2].gameObject.SetActive(false);
        }

        matchMakerContainer.gameObject.SetActive(mSelectedTab == 0);
        personalContainer.gameObject.SetActive(mSelectedTab == 1);
        mapContainer.gameObject.SetActive(mSelectedTab == 2);
        inventoryContainer.gameObject.SetActive(mSelectedTab == 3);

        for (int i = 0; i < tabIcons.Length; ++i)
        {
            Color color = (i == mSelectedTab) ? Color.white : new Color(0.7f, 0.7f, 0.7f, 1f);
            tabIcons[i].color = color;
            tabs[i].color = color;
        }

        // Ensure the panels are setup if we have to swap out images to display current
        // character details
        panel1.ReestablishModels();
        panel2.ReestablishModels();

        // If we switch to personal view, show the personal character ...
        if (personalContainer.gameObject.activeSelf)
        {
            frontCameraRig.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.avatar.characterData, false);
        }
    }
}
