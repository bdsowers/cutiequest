using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : MonoBehaviour
{
    Killable mKillable;

    private float mDamageTime = 1.5f;
    private int mDamageAmount = 1;
    private float mNumberDamages = 10;

    private float mDamageTimer;
    private int mTimesDamaged;

    // Start is called before the first frame update
    void Start()
    {
        mKillable = GetComponentInParent<Killable>();
        if (mKillable == null)
            Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (mKillable.isDead)
            return;

        mDamageTimer += Time.deltaTime;
        if (mDamageTimer > mDamageTime)
        {
            mDamageTimer = 0f;
            mKillable.TakeDamage(mDamageAmount, DamageReason.StatusEffect);

            mTimesDamaged++;
            if (mTimesDamaged >= mNumberDamages)
            {
                Destroy(this);
            }
        }
    }
}
