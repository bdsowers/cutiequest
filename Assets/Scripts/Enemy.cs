using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : CharacterComponentBase
{
    private EnemyAI mEnemyAI;
    private RevealWhenAvatarIsClose mReveal;

    public float actionCooldown;
    private float mActionCooldownTimer = -1;
    private bool mActivated = false;

    public bool isBoss;

    // Start is called before the first frame update
    void Start()
    {
        Game.instance.enemyDirector.RegisterEnemy(this);

        mEnemyAI = GetComponent<EnemyAI>();
        mReveal = GetComponentInChildren<RevealWhenAvatarIsClose>();

        commonComponents.killable.onDeath += OnDeath;
        commonComponents.killable.onHit += OnHit;

        Game.instance.centralEvents.FireEnemyCreated(this);
    }

    private void OnHit(Killable entity)
    {
        mActivated = true;
    }

    private void OnDestroy()
    {
        commonComponents.killable.onDeath -= OnDeath;
        commonComponents.killable.onHit -= OnHit;

        Game.instance.enemyDirector.UnregisterEnemy(this);
    }

    public void SetEnemyAI(EnemyAI ai)
    {
        mEnemyAI = ai;
    }

    private void OnDeath(Killable entity)
    {
        commonComponents.simpleMovement.ClearSpaceMarking();

        DropsItems[] di = GetComponentsInChildren<DropsItems>();
        for (int i = 0; i < di.Length; ++i)
        {
            di[i].Drop();
        }
    }

    private void OnTurnGranted()
    {
        UpdateAI();
    }

    // Update is called once per frame
    public void UpdateEnemy()
    {
        if (!mActivated)
        {
            float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
            if (distance < 7f)
            {
                float secondDistanceAmount = 7f;

                // If the player has a fake nose on, decrease activation distance
                // Still keep this inside the first distance check for performance
                if (Game.instance.playerStats.IsItemEquipped<FakeNose>() && !isBoss)
                {
                    secondDistanceAmount = 3.5f;
                }

                if (distance < secondDistanceAmount)
                {
                    mActivated = true;
                }
            }
        }

        if (mActivated)
        {
            if (mActionCooldownTimer >= 0f)
                mActionCooldownTimer -= Time.deltaTime;

            if (CanUpdateAI())
            {
                UpdateAI();
                mActionCooldownTimer = actionCooldown;
            }
        }
    }

    public bool CanUpdateAI()
    {
        if (mEnemyAI == null)
            return false;

        if (!mReveal.fullyRevealed)
            return false;

        if (commonComponents.killable.isDead)
            return false;
        if (commonComponents.killable.isReviving)
            return false;
        if (!mEnemyAI.enabled)
            return false;
        if (Game.instance.realTime && mActionCooldownTimer > 0f)
            return false;
        if (commonComponents.simpleMovement.isMoving)
            return false;

        if (!Game.instance.avatar.isAlive)
            return false;
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return false;
        if (Game.instance.transitionManager.isTransitioning)
            return false;

        return mEnemyAI.CanUpdateAI();
    }

    private void UpdateAI()
    {
        if (!CanUpdateAI())
            return;

        // If the enemy is dancin' or has brain freeze or scared, that's their AI update
        // TODO PRERELEASE - make this more efficient; this is just silly
        ForceDance fd = GetComponent<ForceDance>();
        if (fd != null)
        {
            fd.UpdateAI(this);
            return;
        }

        BrainFreeze bf = GetComponent<BrainFreeze>();
        if (bf != null)
        {
            bf.UpdateAI(this);
            return;
        }

        Scared scared = GetComponent<Scared>();
        if (scared != null)
        {
            scared.TryMoveAwayFromPlayer(this);
            return;
        }

        mEnemyAI.UpdateAI();
    }

    // Turn-based support only
    public bool ReadyForTurn()
    {
        return mEnemyAI.CanUpdateAI() && !commonComponents.simpleMovement.isMoving;
    }
}
