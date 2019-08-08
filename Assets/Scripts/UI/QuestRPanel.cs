using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VectorExtensions;
using ArrayExtensions;
using UnityEngine.UI;

public class QuestRPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public GameObject standardView;
    public GameObject moreInfoView;

    public bool isFrontPanel;
    public int characterOffset;

    public GameObject cameraRig;

    public Text[] nameLabels;
    public Text[] taglineLabels;
    public Image[] spellImages;
    public Image[] boostImages;
    public Image[] quirkImages;
    public Text bioLabel;

    public Text[] spellTitleLabels;
    public Text[] spellDescLabels;
    public Text[] boostTitleLabels;
    public Text[] boostDescLabels;
    public Text[] quirkTitleLabels;
    public Text[] quirkDescLabels;

    private List<CharacterData> mAvailableCharacters;
    private int mCurrentCharacter;

    private bool mIsMouseOver = false;
    private Vector2 mStartDragPosition;
    private static bool mIsQuestPanelAnimating = false;
    private float mThreshold = 500f;

    private QuestR mParent;

    private bool mJoystickNeedsReset;

    public List<CharacterData> availableCharacters
    {
        get { return mAvailableCharacters; }
        set { mAvailableCharacters = value; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    // Start is called before the first frame update
    public void Setup()
    {
        mParent = GetComponentInParent<QuestR>();
        mParent.moreInfoMode = false;
        mCurrentCharacter = characterOffset;
        SetupForCharacter(mAvailableCharacters[mCurrentCharacter]);
    }

    // Update is called once per frame
    void Update()
    {
        if (mParent.tutorialMode)
            return;

        if (mIsQuestPanelAnimating)
            return;

        if (mIsMouseOver)
        {
            Vector2 mousePosition = Input.mousePosition.AsVector2();
            Vector2 diff = mousePosition - mStartDragPosition;
            diff.y = 0;

            transform.localPosition += diff.AsVector3();
            
            mStartDragPosition = mousePosition;

            if (Mathf.Abs(transform.localPosition.x) >= mThreshold)
            {
                StartCoroutine(FlyToPosition(Mathf.Sign(transform.localPosition.x) * 1500f * Vector3.right, true, transform.localPosition.x > 0f));
                mIsMouseOver = false;
            }
        }

        if (Game.instance.actionSet.Like.WasPressed)
        {
            OnLikePressed();
        }

        if (Game.instance.actionSet.Dislike.WasPressed)
        {
            OnPassPressed();
        }

        if (Game.instance.actionSet.boundDevice != null)
        {
            UpdateJoystick();
        }

        if (Game.instance.actionSet.ToggleMap.WasPressed)
        {
            if (moreInfoView.gameObject.activeSelf)
                OnLessInfoPressed();
            else
                OnMoreInfoPressed();
        }
    }

    private void UpdateJoystick()
    {
        if (!isFrontPanel)
            return;

        Vector2 moveValue = Game.instance.actionSet.Move.Value;

        if (mParent.joystickNeedsReset)
        {
            if (Mathf.Abs(moveValue.x) < 0.1f)
            {
                mParent.joystickNeedsReset = false;
            }
            else
            {
                return;
            }
        }

        transform.localPosition = Vector3.right * moveValue.x * 500;
        if (Mathf.Abs(moveValue.x) > 0.7f)
        {
            StartCoroutine(FlyToPosition(Mathf.Sign(transform.localPosition.x) * 1500f * Vector3.right, true, transform.localPosition.x > 0f));
            mParent.joystickNeedsReset = true;
        }
    }

    IEnumerator FlyToPosition(Vector3 endPosition, bool swapAfter, bool accepted)
    {
        Vector3 startPosition = transform.localPosition;
        mIsQuestPanelAnimating = true;
        
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 4f; 
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time);
            yield return null;
        }

        transform.localPosition = endPosition;

        int character = mCurrentCharacter;

        if (swapAfter)
        {
            Swap();
        }

        if (accepted)
        {
            mParent.AcceptCharacter(mAvailableCharacters[character]);
        }
        
        mIsQuestPanelAnimating = false;
        yield break;
    }

    public void Swap()
    {
        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
        transform.localPosition = Vector3.zero;
        QuestRPanel[] allPanels = GameObject.FindObjectsOfType<QuestRPanel>();
        for (int i = 0; i < allPanels.Length; ++i)
            allPanels[i].isFrontPanel = !allPanels[i].isFrontPanel;

        mCurrentCharacter = (mCurrentCharacter + 2) % mAvailableCharacters.Count;
        SetupForCharacter(mAvailableCharacters[mCurrentCharacter]);
    }

    void LateUpdate()
    {
        float rotation = transform.localPosition.x * -0.1f;
        if (rotation < -30f) rotation = -30f;
        if (rotation > 30f) rotation = 30f;

        transform.localRotation = Quaternion.Euler(0f, 0f, rotation);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (mParent.tutorialMode)
            return;

        if (mIsQuestPanelAnimating)
            return;

        if (!isFrontPanel)
            return;

        mIsMouseOver = true;
        mStartDragPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (mParent.tutorialMode)
            return;

        mIsMouseOver = false;

        if (isFrontPanel && Mathf.Abs(transform.localPosition.x) < mThreshold)
        {
            StartCoroutine(FlyToPosition(Vector3.zero, false, false));
        }
    }

    public void SetupForCharacter(CharacterData characterData)
    {
        SetupLabelInfo(characterData);

        cameraRig.GetComponentInChildren<CharacterModel>().ChangeModel(characterData, false);

        standardView.gameObject.SetActive(!mParent.moreInfoMode);
        standardView.GetComponent<CanvasGroup>().alpha = 1f;
        moreInfoView.gameObject.SetActive(mParent.moreInfoMode);
    }

    private void SetupLabelInfo(CharacterData characterData)
    {
        string nameText = LocalizedText.Get(characterData.characterName) + "   <color=#AAAAAA>" + characterData.age.ToString() + "</color>";
        string taglineText = LocalizedText.Get(characterData.tagline);

        System.Array.ForEach(nameLabels, (label) => label.text = nameText);
        System.Array.ForEach(taglineLabels, (label) => label.text = taglineText);

        if (characterData.spell != null)
        {
            System.Array.ForEach(spellImages, (image) => image.sprite = characterData.spell.icon);
            System.Array.ForEach(spellTitleLabels, (label) => label.text = "Spell: " + LocalizedText.Get(characterData.spell.friendlyName));
            System.Array.ForEach(spellDescLabels, (label) => label.text = LocalizedText.Get(characterData.spell.description));
        }

        if (characterData.quirk != null)
        {
            System.Array.ForEach(quirkImages, (image) => image.sprite = characterData.quirk.icon);
            System.Array.ForEach(quirkTitleLabels, (label) => label.text = "Quirk: " + LocalizedText.Get(characterData.quirk.friendlyName));
            System.Array.ForEach(quirkDescLabels, (label) => label.text = LocalizedText.Get(characterData.quirk.description));
        }

        CharacterStatData statData = Game.instance.characterStatInfo.DataForStat(characterData.statBoost);
        System.Array.ForEach(boostImages, (image) => image.sprite = statData.icon);
        System.Array.ForEach(boostTitleLabels, (label) => label.text = "Passive Boost: " + LocalizedText.Get(statData.name) + " + " + characterData.statBoostAmount.ToString());
        System.Array.ForEach(boostDescLabels, (label) => label.text = LocalizedText.Get(statData.description));
        
        bioLabel.text = LocalizedText.Get(characterData.bio);


    }

    public void MoreInfo()
    {
        moreInfoView.gameObject.SetActive(true);
        mParent.moreInfoMode = true;

        StartCoroutine(FadeCanvasGroup(standardView.GetComponent<CanvasGroup>(), 1f, 0f));
        StartCoroutine(FadeCanvasGroup(moreInfoView.GetComponent<CanvasGroup>(), 0f, 1f));
    }

    public void LessInfo()
    {
        standardView.gameObject.SetActive(true);
        mParent.moreInfoMode = false;

        StartCoroutine(FadeCanvasGroup(standardView.GetComponent<CanvasGroup>(), 0f, 1f));
        StartCoroutine(FadeCanvasGroup(moreInfoView.GetComponent<CanvasGroup>(), 1f, 0f));
    }

    public void OnMoreInfoPressed()
    {
        if (mParent.tutorialMode)
            return;

        MoreInfo();
    }

    public void OnLessInfoPressed()
    {
        if (mParent.tutorialMode)
            return;

        LessInfo();
    }

    public void OnLikePressed()
    {
        if (mParent.tutorialMode)
            return;

        if (!isFrontPanel)
            return;

        StartCoroutine(FlyToPosition(1 * 1500f * Vector3.right, true, true));
    }

    public void OnPassPressed()
    {
        if (mParent.tutorialMode)
            return;

        if (!isFrontPanel)
            return;

        StartCoroutine(FlyToPosition(-1 * 1500f * Vector3.right, true, false));

    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float sourceAlpha, float targetAlpha)
    {
        group.alpha = sourceAlpha;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            group.alpha = Mathf.Lerp(sourceAlpha, targetAlpha, time);
            yield return null;
        }

        group.alpha = targetAlpha;

        if (targetAlpha < 0.1f)
            group.gameObject.SetActive(false);

        yield break;
    }
}
