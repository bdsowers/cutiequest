using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float cooldown;
    private float mCooldownTimer;

    public Sprite icon;

    public bool canActivate
    {
        get { return mCooldownTimer <= 0f; }
    }

    public float cooldownTimer
    {
        get { return mCooldownTimer; }
    }

    public virtual void Activate(GameObject caster)
    {
        mCooldownTimer = cooldown;
    }

    private void Update()
    {
        if (mCooldownTimer >= 0)
        {
            mCooldownTimer -= Time.deltaTime;
        }
    }
}
