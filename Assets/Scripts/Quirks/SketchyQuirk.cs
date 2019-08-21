using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using SCPE;

public class SketchyQuirk : Quirk
{
    EdgeDetection mEdgeDetection;
    Sketch mSketch;

    PostProcessVolume mVolume;

    public Texture strokeTexture;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mEdgeDetection = ScriptableObject.CreateInstance<EdgeDetection>();
        mEdgeDetection.enabled.Override(true);
        mEdgeDetection.mode.Override(EdgeDetection.EdgeDetectMode.LuminanceBased);

        mSketch = ScriptableObject.CreateInstance<Sketch>();
        mSketch.enabled.Override(true);
        mSketch.strokeTex.Override(strokeTexture);
        mSketch.blendMode.Override(Sketch.SketchMode.Add);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mEdgeDetection, mSketch);
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

    void DestroyVolume()
    {
        if (mVolume == null)
            return;

        RuntimeUtilities.DestroyVolume(mVolume, true, true);
        mVolume = null;
    }
}
