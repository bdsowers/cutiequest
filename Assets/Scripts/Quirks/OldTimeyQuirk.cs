using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OldTimeyQuirk : Quirk
{
    Grayscale mGrayscale;
    Grain mGrain;
    PostProcessVolume mVolume;

    private static bool mEnabled;

    public static bool enabled {  get { return mEnabled; } }

    private void OnEnable()
    {
        mEnabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        mGrayscale = ScriptableObject.CreateInstance<Grayscale>();
        mGrayscale.enabled.Override(true);

        mGrain = ScriptableObject.CreateInstance<Grain>();
        mGrain.enabled.Override(true);
        mGrain.intensity.Override(1f);
        mGrain.size.Override(1.5f);
        mGrain.lumContrib.Override(1f);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mGrayscale, mGrain);

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
