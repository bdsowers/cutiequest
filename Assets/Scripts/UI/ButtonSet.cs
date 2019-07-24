﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSet : MonoBehaviour
{
    public List<Button> buttons;

    private void OnEnable()
    {
        Reset();
    }

    public void Reset()
    {
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
