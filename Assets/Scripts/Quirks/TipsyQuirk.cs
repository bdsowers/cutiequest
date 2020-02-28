using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TipsyQuirk : Quirk
{
    LensDistortion mLensDistortion;
    PostProcessVolume mVolume;

    float mIntensity = 0f;
    float mDirection = 1f;
    bool mMovedCenter = false;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        // If the player has the monocle, vision-based quirks don't kick in
        if (Game.instance.playerStats.IsItemEquipped<Monocle>())
            return;

        mLensDistortion = ScriptableObject.CreateInstance<LensDistortion>();
        mLensDistortion.enabled.Override(true);
        mLensDistortion.intensity.Override(0f);
        mLensDistortion.centerX.Override(0.1f);
        mLensDistortion.centerY.Override(0.1f);

        mVolume = PostProcessManager.instance.QuickVolume(LayerMask.NameToLayer("VFXVolume"), 100f, mLensDistortion);
    }

    // Update is called once per frame
    void Update()
    {
        if (mVolume == null)
            return;

        float influence = 50f;
        float speed = 50f;

        mIntensity += mDirection * Time.deltaTime * speed;
        if (mIntensity > influence || mIntensity < -influence)
        {
            mIntensity = Mathf.Clamp(mIntensity, -influence, influence);
            mDirection = -mDirection;
            mMovedCenter = false;
        }

        if (Mathf.Abs(mIntensity) < 5f && !mMovedCenter)
        {
            mMovedCenter = true;
            mLensDistortion.centerX.Override(Random.Range(-0.2f, 0.2f));
            mLensDistortion.centerY.Override(Random.Range(-0.2f, 0.2f));
        }

        mLensDistortion.intensity.value = mIntensity;
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
