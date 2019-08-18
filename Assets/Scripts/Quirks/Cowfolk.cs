using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Cowfolk : Quirk
{
    Sepia mSepia;
    PostProcessVolume mVolume;

    private static bool mEnabled;

    public static bool quirkEnabled { get { return mEnabled; } }

    private void OnEnable()
    {
        mEnabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        mSepia = ScriptableObject.CreateInstance<Sepia>();
        mSepia.enabled.Override(true);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mSepia);

        mEnabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDisable()
    {
        mEnabled = false;
        DestroyVolume();
    }

    private void OnDestroy()
    {
        mEnabled = false;
        DestroyVolume();
    }

    void DestroyVolume()
    {
        if (mVolume == null)
            return;

        RuntimeUtilities.DestroyVolume(mVolume, true, true);
        mVolume = null;
    }
}
