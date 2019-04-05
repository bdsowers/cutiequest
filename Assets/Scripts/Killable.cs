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

    public void TakeDamage(int damage)
    {
        CreateHitVFX();

        // If we're talking about the player, use their info that's stashed away in a saveable place
        if (GetComponent<PlayerController>())
        {
            Game.instance.playerData.health -= damage;
        }

        health -= damage;

        NumberPopupGenerator.instance.GeneratePopup(transform.position + Vector3.up * 0.7f, damage, NumberPopupReason.TakeDamage);

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
            GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX2_EnemyDeathSkull");
            vfx.transform.position = transform.position + Vector3.up * 0.5f;
            vfx.transform.localScale = Vector3.one * 0.75f;
            vfx.AddComponent<DestroyAfterTimeElapsed>().time = 2f;

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
