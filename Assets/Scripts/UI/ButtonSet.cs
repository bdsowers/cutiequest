using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonSet : MonoBehaviour
{
    public List<Button> buttons;

    public bool autoSelect = true;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (autoSelect)
        {
            StartCoroutine(Reset());
        }
    }

    private void OnDisable()
    {
        // If there are any active button sets in the scene, make sure they become active again.
        ButtonSet[] allButtonSets = GameObject.FindObjectsOfType<ButtonSet>();
        for (int i = 0; i < allButtonSets.Length; ++i)
        {
            if (allButtonSets[i] != this && allButtonSets[i].gameObject.activeInHierarchy)
            {
                allButtonSets[i].TakeFocus();
                return;
            }
        }
    }

    public IEnumerator Reset()
    {
        // Waiting a single frame is more reliable...
        yield return null;

        // note bdsowers - this is a bit of a hack
        // Reselecting the same button does nothing, even if that button was disabled/enabled.
        // To work around this, select a different and re-select the first button.
        EventSystem.current.SetSelectedGameObject(null);
        yield return null;

        if (buttons.Count > 0)
        {
            Button button = FirstAvailableButton();
            button.Select();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
        }

        yield break;
    }

    public Button FirstAvailableButton()
    {
        for (int i = 0; i < buttons.Count; ++i)
        {
            if (buttons[i].interactable && buttons[i].gameObject.activeInHierarchy)
            {
                return buttons[i];
            }
        }

        return buttons[0];
    }

    public void TakeFocus()
    {
        StartCoroutine(Reset());
    }

    public void ForceUnfocus()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private IEnumerator SelectButtonAfterTick()
    {
        yield return null;
    }

    public void Clear()
    {
        buttons.Clear();
    }

    public void AddButton(Button button)
    {
        buttons.Add(button);
    }
}
