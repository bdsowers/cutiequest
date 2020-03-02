using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Base class for all Quirks.
/// Doesn't really do anything atm, but may in the future.
/// Most quirks have very specific behavior and need to listen for a lot of game events
/// to do their work; there's no convenient contract we can bind them to.
/// </summary>
public class Quirk : MonoBehaviour
{
    public string friendlyName;
    public string description;
    public Sprite icon;
    public int requiredLevel = 1;
    public int requiredScoutLevel = 0;

    public virtual void Start()
    {
        Game.instance.quirkRegistry.RegisterQuirkActive(this);
    }

    public virtual void OnEnable()
    {
        Game.instance.quirkRegistry.RegisterQuirkActive(this);
    }

    public virtual void OnDestroy()
    {
        Game.instance.quirkRegistry.RegisterQuirkInactive(this);
    }

    public virtual void OnDisable()
    {
        Game.instance.quirkRegistry.RegisterQuirkInactive(this);
    }

    public virtual void DestroyVolume()
    {
        // Generally does nothing, but some quirks have vision VFX setup that may need to be destroyed
    }

    // These two must be called AFTER creating/destroying VFX volumes
    public void VFXVolumeCreated()
    {
        FollowCamera.main.postProcessLayer.enabled = true;
    }

    public void VFXVolumeDestroyed()
    {
        // TODO bdsowers - disable post process layer if possible
        // This isn't really urgent; this will happen automatically between runs, and we don't
        // really disable VFX mid-run.
    }
}
