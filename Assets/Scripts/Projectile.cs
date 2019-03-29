using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int strength { get; set; }

    private void OnCollisionEnter(Collision collision)
    {
        Killable targetKillable = collision.collider.GetComponentInParent<Killable>();
        if (targetKillable != null)
        {
            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
            int damage = strength * 4 - defense * 2;

            targetKillable.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}
