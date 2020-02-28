using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepetitiveDamageTrap : MonoBehaviour
{
    public int strength { get; set; }

    private List<Killable> mKillablesInTrap = new List<Killable>();
    private float mTimer = 1.5f;
    private float mDamageFrequency = 1.5f;

    private void OnTriggerEnter(Collider other)
    {
        Killable killable = other.gameObject.GetComponentInParent<Killable>();
        if (killable != null)
        {
            mKillablesInTrap.Add(killable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Killable killable = other.gameObject.GetComponentInParent<Killable>();
        if (killable != null)
        {
            mKillablesInTrap.Remove(killable);
        }
    }

    private void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            mTimer = mDamageFrequency;

            mKillablesInTrap.RemoveAll((i) => (i == null));

            for (int i = 0; i < mKillablesInTrap.Count; ++i)
            {
                mKillablesInTrap[i].TakeDamage(strength, DamageReason.Trap);
            }
        }
    }
}
