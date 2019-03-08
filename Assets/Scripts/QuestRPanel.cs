using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VectorExtensions;

public class QuestRPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool mIsMouseOver = false;
    private Vector2 mStartDragPosition;
    private static bool mIsQuestPanelAnimating = false;
    private float mThreshold = 500f;

    public bool isFrontPanel;

    public void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
                StartCoroutine(FlyToPosition(Mathf.Sign(transform.localPosition.x) * 1500f * Vector3.right));
                mIsMouseOver = false;
            }
        }
    }

    IEnumerator FlyToPosition(Vector3 endPosition)
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

        mIsQuestPanelAnimating = false;
        yield break;
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

        if (Mathf.Abs(transform.localPosition.x) < mThreshold)
        {
            StartCoroutine(FlyToPosition(Vector3.zero));
        }
    }
}
