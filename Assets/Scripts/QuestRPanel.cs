using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using VectorExtensions;

public class QuestRPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool mIsMouseOver = false;
    private Vector2 mStartDragPosition;

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
        if (mIsMouseOver)
        {
            Vector2 mousePosition = Input.mousePosition.AsVector2();
            Vector2 diff = mousePosition - mStartDragPosition;
            diff.y = 0;

            transform.position += diff.AsVector3();
            float rotation = transform.localPosition.x * -0.1f;
            if (rotation < -30f) rotation = -30f;
            if (rotation > 30f) rotation = 30f;

            transform.localRotation = Quaternion.Euler(0f, 0f, rotation);

            mStartDragPosition = mousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isFrontPanel)
            return;

        mIsMouseOver = true;
        mStartDragPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        mIsMouseOver = false;
    }
}
