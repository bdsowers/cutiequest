using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private List<Dialog> mOpenDialogs = new List<Dialog>();

    public bool AnyDialogsOpen()
    {
        return mOpenDialogs.Count > 0;
    }

    public void DialogOpened(Dialog dialog)
    {
        mOpenDialogs.Add(dialog);
    }

    public void DialogClosed(Dialog dialog)
    {
        mOpenDialogs.Remove(dialog);
    }
}
