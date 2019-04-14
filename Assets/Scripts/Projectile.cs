using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int strength { get; set; }
    public bool destroyOnEnemyHit = true;

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
            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
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
            Destroy(gameObject);
        }
    }
}
