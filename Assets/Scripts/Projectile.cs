using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int strength { get; set; }
    public bool destroyOnEnemyHit = true;
    public bool destroyOnEnvironmentHit = true;
    public string sound;

    private List<Killable> mEnemiesHit = new List<Killable>();

    private void Start()
    {
        if (!string.IsNullOrEmpty(sound))
            Game.instance.soundManager.PlaySound(sound);
    }

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
            // If the victim was the player & the player has a mirror shield, reflect!
            if (targetKillable.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                if (Game.instance.playerStats.IsItemEquipped<MirrorShield>())
                {
                    MirrorShield shield = Game.instance.playerStats.GetComponentInChildren<MirrorShield>();
                    shield.Reflect(this);
                    return;
                }
            }

            CharacterStatistics stats = targetKillable.GetComponent<CharacterStatistics>();
            int defense = stats == null ? 0 : stats.ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);
            int damage = strength * 4 - defense * 2;
            mEnemiesHit.Add(targetKillable);

            targetKillable.TakeDamage(null, damage, DamageReason.Projectile);

            if (destroyOnEnemyHit)
            {
                Destroy(gameObject);

                PlayHitVFX();
            }
        }
        else
        {
            if (destroyOnEnvironmentHit)
            {
                Destroy(gameObject);

                PlayHitVFX();
            }
        }
    }

    void PlayHitVFX()
    {
        GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX_Poof");
        vfx.transform.position = transform.position;
        vfx.transform.localScale = Vector3.one * 0.35f;
    }
}
