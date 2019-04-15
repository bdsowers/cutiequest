using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    private static Decoy mInstance;
    public static Decoy instance
    {
        get { return mInstance; }
    }

    private void Awake()
    {
        mInstance = this;
    }

    private void OnDestroy()
    {
        mInstance = null;
    }
}
