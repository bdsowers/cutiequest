using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Killable : MonoBehaviour
{
    public enum DeathResponse
    {
        Destroy,
        MakeInactive,
    }

    public delegate void Died(Killable entity);
    public event Died onDeath;

    public int health;
    public DeathResponse deathResponse;
    public bool showNumberPopups = true;

    public string deathEffect = "CFX2_EnemyDeathSkull";
    public float deathEffectScale = 0.75f;

    private Enemy mEnemy;

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
    }

    private bool CanTakeDamage()
    {
        if (Game.instance.transitionManager.isTransitioning)
            return false;
        
        return true;
    }

    public void TakeDamage(int damage)
    {
        if (!CanTakeDamage())
            return;

        // No attack can ever do less than 1 damage, no matter strength/defense equations
        damage = Mathf.Max(damage, 1);

        CreateHitVFX();

        // If we're talking about the player, use their info that's stashed away in a saveable place
        if (GetComponent<PlayerController>())
        {
            Game.instance.playerData.health -= damage;
        }

        health -= damage;
        if (mEnemy != null)
        {
            Game.instance.centralEvents.FireEnemyHit(mEnemy, damage);
        }

        if (showNumberPopups)
        {
            NumberPopupGenerator.instance.GeneratePopup(transform.position + Vector3.up * 0.7f, damage, NumberPopupReason.TakeDamage);
        }

        if (health <= 0f)
        {
            HandleDeath();
        }
    }

    private void CreateHitVFX()
    {
        GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX3_Hit_SmokePuff");
        vfx.transform.position = transform.position + Vector3.up * 0.5f;
        vfx.AddComponent<DestroyAfterTimeElapsed>().time = 1f;

        GameObject vfx2 = PrefabManager.instance.InstantiatePrefabByName("CFX_Hit_C White");
        vfx2.transform.position = transform.position + Vector3.up * 0.5f;
        vfx2.AddComponent<DestroyAfterTimeElapsed>().time = 1f;

        if (GetComponent<PlayerController>() != null)
        {
            HitFlash hitFlash = GameObject.FindObjectOfType<HitFlash>();
            hitFlash.Flash();
        }
    }

    private void HandleDeath()
    {
        if (onDeath != null)
        {
            onDeath(this);
        }

        if (deathResponse == DeathResponse.Destroy)
        {
            float destroyTime = 2f;
            if (deathEffect == "CFX2_EnemyDeathSkull" && GothQuirk.quirkEnabled)
            {
                deathEffect = "CFX2_BatsCloud";
                destroyTime = 5f;
            }

            GameObject vfx = PrefabManager.instance.InstantiatePrefabByName(deathEffect);
            vfx.transform.position = transform.position + Vector3.up * 0.5f;
            vfx.transform.localScale = Vector3.one * deathEffectScale;
            vfx.AddComponent<DestroyAfterTimeElapsed>().time = destroyTime;

            Destroy(gameObject);
        }
        else
        {
            GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX2_BrokenHeart");
            vfx.transform.position = transform.position + Vector3.up * 0.5f;

            // gameObject.SetActive(false);
        }
    }
}
