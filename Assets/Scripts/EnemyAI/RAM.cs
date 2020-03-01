using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using DG.Tweening;
using VectorExtensions;

public class RAM : EnemyAI
{
    // Core actions:
    //  * Teleport around wildly
    //  * Teleport far away, charge forward, deal massive damage
    //  * Teleport far away, cast screen-filling spell with narrow cut-out
    //  * Cast screen-filling spell with rigid pattern

    // Progression:
    //  * Gets faster at each stage

    Enemy mEnemy;

    CharacterStatistics mStatistics;
    Killable mKillable;
    EnemyAI mCurrentActiveModule;
    EnemyTeleport mTeleport;
    SimpleMovement mSimpleMovement;
    SimpleAttack mSimpleAttack;
    Animator mAnimator;

    public enum AIState
    {
        Wandering,
        Charging,
        Stun,
        Running,
    }

    int mCurrentPhase = 0;

    private AIState mCurrentState = AIState.Wandering;
    private int mNumTeleports = 0;
    private Vector3 mChargeDirection;
    private float mStunTimer = 0f;

    private bool mTeleportedIntoPlaceForState = false;

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
        mStatistics = GetComponent<CharacterStatistics>();
        mKillable = GetComponent<Killable>();
        mTeleport = GetComponent<EnemyTeleport>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mAnimator = GetComponentInChildren<Animator>();

        mEnemy.SetEnemyAI(this);

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

        Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
        Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);
    }

    private void OnEnable()
    {
        // Reveal the whole map when this boss becomes active
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }

    public override bool CanUpdateAI()
    {
        if (!this.enabled)
            return false;
        if (mKillable.isDead)
            return false;
        if (mSimpleMovement.isMoving)
            return false;
        if (mSimpleAttack.isAttacking)
            return false;

        return true;
    }

    public override void UpdateAI()
    {
        if (mCurrentState == AIState.Stun)
        {
            if (mStunTimer > 2f)
            {
                mCurrentState = AIState.Wandering;
            }
        }
        else if (mCurrentState == AIState.Wandering)
        {

        }
        else if (mCurrentState == AIState.Charging)
        {
            if (mSimpleAttack.CanAttack(mChargeDirection))
            {
                mSimpleAttack.Attack(mChargeDirection);
                Camera.main.GetComponent<FollowCamera>().SimpleShake();

                mCurrentState = AIState.Running;
            }
            else if (mSimpleMovement.CanMove(mChargeDirection))
            {
                mSimpleMovement.Move(mChargeDirection);
            }
            else
            {
                mStunTimer = 0f;
                mCurrentState = AIState.Stun;

                Camera.main.GetComponent<FollowCamera>().SimpleShake();
            }
        }
        else if (mCurrentState == AIState.Running)
        {

        }
    }

    private Vector3 TowardPlayer()
    {
        Vector3 towardPlayer = Game.instance.avatar.transform.position - transform.position;
        towardPlayer.y = 0f;

        if (Mathf.Abs(towardPlayer.x) > Mathf.Abs(towardPlayer.z))
            towardPlayer.z = 0;
        else
            towardPlayer.x = 0;

        if (towardPlayer.magnitude < 0.1f)
            towardPlayer = Vector3.right;

        towardPlayer.Normalize();
        return towardPlayer;
    }

    private bool StraightLineToPlayer()
    {
        return false;
    }

    public override void AIStructureChanged()
    {
    }

    private void OnDestroy()
    {
        if (Game.instance != null && Game.instance.hud != null)
            Game.instance.hud.bossHealth.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mCurrentState == AIState.Stun)
        {
            mStunTimer += Time.deltaTime;
            mAnimator.Play("Dizzy");
        }

        Game.instance.hud.bossHealth.SetWithValues(0, mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject), mKillable.health);
    }
}
