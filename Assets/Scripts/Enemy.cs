﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;
    private SpellCaster mSpellCaster;
    private ProjectileThrower mProjectileThrower;

    private Killable mKillable;

    public float actionCooldown;
    private float mActionCooldownTimer = -1;

    // Start is called before the first frame update
    void Start()
    {
        mSimpleMovement = GetComponent<SimpleMovement>();
        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mSpellCaster = GetComponent<SpellCaster>();
        mProjectileThrower = GetComponent<ProjectileThrower>();
        mKillable = GetComponent<Killable>();

        mTurnBasedMovement.onTurnGranted += OnTurnGranted;
        mSimpleMovement.onMoveFinished += OnMoveFinished;

        if (mSimpleAttack != null)
        {
            mSimpleAttack.onAttackFinished += OnAttackFinished;
        }

        mKillable.onDeath += OnDeath;

        Game.instance.centralEvents.FireEnemyCreated(this);
    }

    private void OnDeath(Killable entity)
    {
        DropsItems di = GetComponent<DropsItems>();
        if (di != null)
        {
            di.Drop();
        }
    }

    private void OnAttackFinished(GameObject attacker, GameObject target)
    {
        mTurnBasedMovement.TurnFinished();
    }

    private void OnMoveFinished()
    {
        mTurnBasedMovement.TurnFinished();
    }

    private void OnTurnGranted()
    {
        UpdateAI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mTurnBasedMovement.canTakeTurns)
        {
            float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
            if (distance < 7f)
            {
                mTurnBasedMovement.ActivateTurnMovement();
            }
        }

        if (Game.instance.realTime)
        {
            if (mTurnBasedMovement.canTakeTurns)
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
    }

    private bool CanUpdateAI()
    {
        if (mSimpleMovement.isMoving)
            return false;
        if (mSimpleAttack != null && mSimpleAttack.isAttacking)
            return false;
        if (mSpellCaster != null && mSpellCaster.isCasting)
            return false;
        if (mProjectileThrower != null && mProjectileThrower.isThrowing)
            return false;
        if (mActionCooldownTimer > 0f)
            return false;

        if (!Game.instance.avatar.isAlive)
            return false;
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return false;
        if (Game.instance.transitionManager.isTransitioning)
            return false;

        return true;
    }

    private Vector3 OrthogonalDirection(Transform source, Transform target, bool useLargeAxis)
    {
        Vector3 direction = target.position - source.position;
        direction.y = 0f;

        if (useLargeAxis)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                direction.z = 0f;
            }
            else
            {
                direction.x = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                direction.x = 0f;
            }
            else
            {
                direction.z = 0f;
            }
        }

        return direction.normalized;
    }

    private void UpdateAI()
    {
        if (!CanUpdateAI())
            return;

        Vector3 direction = OrthogonalDirection(transform, Game.instance.avatar.transform, true);

        // todo bdsowers - refactor so that melee and spell-based enemies behave differently.
        
        if (mProjectileThrower != null && mProjectileThrower.ShouldThrow())
        {
            mProjectileThrower.ThrowProjectile();
        }
        else if (mSpellCaster != null && mSpellCaster.CanCast())
        {
            mSpellCaster.CastSpell();
        }
        else if (mSimpleAttack != null && mSimpleAttack.CanAttack(direction))
        {
            mSimpleAttack.Attack(direction);
        }
        else if (mSimpleMovement.CanMove(direction))
        {
            mSimpleMovement.Move(direction);
        }
        else
        {
            direction = OrthogonalDirection(transform, GameObject.Find("Avatar").transform, false);
            if (mSimpleMovement.CanMove(direction))
            {
                mSimpleMovement.Move(direction);
            }
            else
            {
                // Cede our turn
                mTurnBasedMovement.TurnFinished();
            }
        }
    }
}
