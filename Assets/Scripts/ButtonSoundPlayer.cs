using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSoundPlayer : MonoBehaviour, ISelectHandler
{
    public static bool suppressSound = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(OnClick);
    }

    void OnClick()
    {
        Game.instance.soundManager.PlaySound("button_press");
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!suppressSound)
            Game.instance.soundManager.PlaySound("button_select");
    }
}
