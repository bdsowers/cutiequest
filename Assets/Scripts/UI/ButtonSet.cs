using System.Collections;
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
        if (buttons.Count > 0)
        {
            buttons[0].Select();
        }
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
