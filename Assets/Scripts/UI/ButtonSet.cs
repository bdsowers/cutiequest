using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSet : MonoBehaviour
{
    public List<Button> buttons;

    public bool autoSelect = true;

    private void OnEnable()
    {
        if (autoSelect)
        {
            StartCoroutine(Reset());
        }
    }

    public IEnumerator Reset()
    {
        // Waiting a single frame is more reliable...
        yield return null;

        // note bdsowers - this is a bit of a hack
        // Reselecting the same button does nothing, even if that button was disabled/enabled.
        // To work around this, select a different and re-select the first button.
        if (buttons.Count > 1)
        {
            buttons[1].Select();
        }

        if (buttons.Count > 0)
        {
            buttons[0].Select();
        }

        yield break;
    }

    public void TakeFocus()
    {
        StartCoroutine(Reset());
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
