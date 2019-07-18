using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    // Update is called once per frame
    public virtual void Update()
    {
        if (Game.instance.actionSet.CloseMenu.WasPressed)
        {
            Close();
        }
    }

    public virtual void Close()
    {
        StartCoroutine(CloseCoroutine());
    }

    IEnumerator CloseCoroutine()
    {
        yield return null;
        yield return null;

        gameObject.SetActive(false);
        yield break;
    }

    public static bool AnyDialogsOpen()
    {
        Dialog dialog = GameObject.FindObjectOfType<Dialog>();
        return dialog != null;
    }
}
