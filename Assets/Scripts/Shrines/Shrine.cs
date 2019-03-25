using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shrine : MonoBehaviour
{
    public string preActivationMessage;

    private bool mActivated;

    public bool activated
    {
        get { return mActivated; }
    }

    public virtual void Activate()
    {
        mActivated = true;
    }
}
