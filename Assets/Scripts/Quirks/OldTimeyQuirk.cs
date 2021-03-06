﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class OldTimeyQuirk : Quirk
{
    Grayscale mGrayscale;
    Grain mGrain;
    PostProcessVolume mVolume;

    public override void OnEnable()
    {
        base.OnEnable();
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // If the player has the monocle, vision-based quirks don't kick in
        if (Game.instance.playerStats.IsItemEquipped<Monocle>())
            return;

        mGrayscale = ScriptableObject.CreateInstance<Grayscale>();
        mGrayscale.enabled.Override(true);

        mGrain = ScriptableObject.CreateInstance<Grain>();
        mGrain.enabled.Override(true);
        mGrain.intensity.Override(1f);
        mGrain.size.Override(1.5f);
        mGrain.lumContrib.Override(1f);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mGrayscale, mGrain);

        VFXVolumeCreated();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void OnDisable()
    {
        base.OnDisable();

        DestroyVolume();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        DestroyVolume();
    }

    public override void DestroyVolume()
    {
        if (mVolume == null)
            return;

        RuntimeUtilities.DestroyVolume(mVolume, true, true);
        mVolume = null;

        VFXVolumeDestroyed();
    }
}
