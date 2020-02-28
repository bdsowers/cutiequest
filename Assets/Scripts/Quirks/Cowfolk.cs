using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Cowfolk : Quirk
{
    Sepia mSepia;
    PostProcessVolume mVolume;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // If the player has the monocle, vision-based quirks don't kick in
        if (Game.instance.playerStats.IsItemEquipped<Monocle>())
            return;

        mSepia = ScriptableObject.CreateInstance<Sepia>();
        mSepia.enabled.Override(true);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mSepia);
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
    }
}
