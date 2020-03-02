using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialog : MonoBehaviour
{
    public virtual void OnEnable()
    {
        Game.instance.dialogManager.DialogOpened(this);
    }

    public virtual void OnDisable()
    {
        if (Game.instance.dialogManager != null) Game.instance.dialogManager.DialogClosed(this);
    }

    public virtual void OnDestroy()
    {
        if (Game.instance.dialogManager != null) Game.instance.dialogManager.DialogClosed(this);
    }

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
}
