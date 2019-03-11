using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Thin wrapper around status effects.
/// It's up to the individual effect to listen for any events it needs to know about and act
/// on them accordingly.
/// </summary>
public abstract class StatusEffect : MonoBehaviour
{
    protected float mDuration;
    protected float mTimeLeft;

    public float duration
    {
        get { return mDuration; }
        protected set
        {
            mDuration = value;
            mTimeLeft = value;
        }
    }

    public float timeLeft
    {
        get { return mTimeLeft; }
    }

    public virtual void OnAdded()
    {

    }

    public virtual void OnRemoved()
    {

    }

    protected virtual void Start()
    {
        OnAdded();
    }

    protected virtual void OnDestroy()
    {
        OnRemoved();
    }

    protected virtual void Update()
    {
        mTimeLeft -= Time.deltaTime;
        if (mTimeLeft <= 0f)
        {
            Destroy(this);
        }
    }
}
