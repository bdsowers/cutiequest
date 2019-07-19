using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ArrayExtensions;

public class DuckpicUI : MonoBehaviour
{
    public Text message;
    public Image image;

    string[] messages = new string[]
    {
        "Check out this huge duck. Like what you see?",
        "On a scale from like it to love it, plz rate this duck.",
        "Have you ever seen a duck this great before?",
        "Would you like to pet the duck?",
        "Hey why aren't you responding to my duck pics?",
    };

    public void OnEnable()
    {
        message.text = messages.Sample();
    }

    public void OnClosePressed()
    {
        Close();
    }

    private void Update()
    {
        if (Game.instance.actionSet.CloseMenu.WasPressed)
        {
            Close();
        }
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }
}
