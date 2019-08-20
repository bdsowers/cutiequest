using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
