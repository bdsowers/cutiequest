using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    private static int mAnyDialogsOpen = -1;

    public static bool AnyDialogsOpen()
    {
        if (mAnyDialogsOpen == -1)
        {
            Dialog dialog = GameObject.FindObjectOfType<Dialog>();
            mAnyDialogsOpen = (dialog != null ? 1 : 0);
        }

        return mAnyDialogsOpen == 1;
    }

    private void LateUpdate()
    {
        mAnyDialogsOpen = -1;
    }
}
