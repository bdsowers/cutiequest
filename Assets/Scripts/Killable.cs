using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Killable : MonoBehaviour
{
    public enum DeathResponse
    {
        Destroy,
        MakeInactive,
        MakeInactiveSilent,
    }

    public delegate void Died(Killable entity);
    public event Died onDeath;

    public delegate void Hit(Killable entity);
    public event Hit onHit;

    public int health;
    public DeathResponse deathResponse;
    public bool showNumberPopups = true;

    public string deathEffect = "CFX2_EnemyDeathSkull";
    public float deathEffectScale = 0.75f;

    public bool allowZeroDamage;

    private Enemy mEnemy;
    private SimpleMovement mMovement;

    public bool invulnerable { get; set; }
    public bool isDead { get; private set; }
    public bool isReviving { get; private set; }

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
        mMovement = GetComponent<SimpleMovement>();
    }

    private bool CanTakeDamage()
    {
        if (Game.instance.transitionManager.isTransitioning)
            return false;
        if (invulnerable)
            return false;
        if (isDead)
            return false;
        if (isReviving)
            return false;

        return true;
    }

    public void TakeDamage(int damage)
    {
        if (!CanTakeDamage())
            return;

        // No attack can ever do less than 1 damage, no matter strength/defense equations
        if (allowZeroDamage)
            damage = Mathf.Max(damage, 0);
        else
            damage = Mathf.Max(damage, 1);

        CreateHitVFX();

        // If we're talking about the player, use their info that's stashed away in a saveable place
        // Also if they're dancing, they take less damage
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (playerController.IsDancing())
            {
                damage /= 2;
            }

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

        if (onHit != null)
        {
            onHit(this);
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
        isDead = true;

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
            if (deathResponse != DeathResponse.MakeInactiveSilent)
            {
                GameObject vfx = PrefabManager.instance.InstantiatePrefabByName("CFX2_BrokenHeart");
                vfx.transform.position = transform.position + Vector3.up * 0.5f;
            }
            else
            {
                mMovement.mesh.transform.DOLocalMoveY(-0.15f, 1f).SetDelay(2f);
            }

            if (mEnemy)
            {
                GetComponentInChildren<Animator>().Play("Death");
            }
        }
    }

    public void Revive()
    {
        isDead = false;
        isReviving = true;
        mMovement.mesh.transform.DOLocalMoveY(0f, 1f).SetDelay(2f);
        health = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.MaxHealth, gameObject);

        mMovement.MarkSpaceUnwalkable();

        GetComponentInChildren<Animator>().Play("Revive");

        Invoke("FinishRevive", 4f);
    }

    private void FinishRevive()
    {
        isReviving = false;
    }
}
