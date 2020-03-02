using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Resolves a problem where the mouse could steal input and never give it back.
public class FocusFixer : MonoBehaviour
{
    private GameObject mLastSelection;

    public void ClearSelection()
    {
        mLastSelection = null;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current == null)
            return;

        if (EventSystem.current.currentSelectedGameObject != null)
        {
            mLastSelection = EventSystem.current.currentSelectedGameObject;
        }
        else
        {
            if (mLastSelection != null && mLastSelection.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(mLastSelection);
            }
        }
    }
}
