using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneButton : MonoBehaviour
{
    public GameObject phoneInterface;

    public void OnPressed()
    {
        phoneInterface.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Game.instance.actionSet.ToggleMap.WasPressed)
        {
            if (!DialogManager.AnyDialogsOpen())
            {
                OnPressed();
            }
        }
    }
}
