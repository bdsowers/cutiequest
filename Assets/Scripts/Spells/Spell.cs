using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public float cooldown;
    private float mCooldownTimer;

    public bool canActivate
    {
        get { return mCooldownTimer <= 0f; }
    }

    public virtual void Activate(GameObject caster)
    {
        mCooldownTimer = cooldown;
    }
}
