using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VectorExtensions;
using ArrayExtensions;
using UnityEngine.UI;

public class QuestRPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public bool isFrontPanel;
    public int characterOffset;

    public GameObject cameraRig;

    public Text nameLabel;
    public Text taglineLabel;

    private List<CharacterData> mAvailableCharacters;
    private int mCurrentCharacter;

    private bool mIsMouseOver = false;
    private Vector2 mStartDragPosition;
    private static bool mIsQuestPanelAnimating = false;
    private float mThreshold = 500f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        mAvailableCharacters = Game.instance.GetComponent<CharacterDataList>().AllCharactersWithinLevelRange(0, Game.instance.playerData.attractiveness);
        
        mCurrentCharacter = characterOffset;
        SetupForCharacter(mAvailableCharacters[mCurrentCharacter]);
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    IEnumerator FlyToPosition(Vector3 endPosition, bool swapAfter, bool accepted)
    {
        Vector3 startPosition = transform.localPosition;
        mIsQuestPanelAnimating = true;
        
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 10f;
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
            GetComponentInParent<QuestR>().AcceptCharacter(mAvailableCharacters[character]);
        }
        
        mIsQuestPanelAnimating = false;
        yield break;
    }

    void Swap()
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
        if (mIsQuestPanelAnimating)
            return;

        if (!isFrontPanel)
            return;

        mIsMouseOver = true;
        mStartDragPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mIsMouseOver = false;

        if (isFrontPanel && Mathf.Abs(transform.localPosition.x) < mThreshold)
        {
            StartCoroutine(FlyToPosition(Vector3.zero, false, false));
        }
    }

    public void SetupForCharacter(CharacterData characterData)
    {
        nameLabel.text = characterData.characterName + "   <color=#AAAAAA>" + characterData.age.ToString() + "</color>";
        taglineLabel.text = characterData.tagline;

        cameraRig.GetComponentInChildren<CharacterModel>().ChangeModel(characterData.model);
    }
}
