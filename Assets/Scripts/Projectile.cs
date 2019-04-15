using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int strength { get; set; }
    public bool destroyOnEnemyHit = true;
    public bool destroyOnEnvironmentHit = true;

    private List<Killable> mEnemiesHit = new List<Killable>();

    private void OnCollisionEnter(Collision collision)
    {
        ApplyHit(collision.collider.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        ApplyHit(other.gameObject);
    }

    void ApplyHit(GameObject collider)
    {
        Killable targetKillable = collider.GetComponentInParent<Killable>();
        if (targetKillable != null && targetKillable.gameObject.layer != gameObject.layer && !mEnemiesHit.Contains(targetKillable))
        {
            CharacterStatistics stats = targetKillable.GetComponent<CharacterStatistics>();
            int defense = stats == null ? 0 : stats.ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
            int damage = strength * 4 - defense * 2;
            mEnemiesHit.Add(targetKillable);

            targetKillable.TakeDamage(damage);

            if (destroyOnEnemyHit)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            if (destroyOnEnvironmentHit)
            {
                Destroy(gameObject);
            }
        }
    }
}
