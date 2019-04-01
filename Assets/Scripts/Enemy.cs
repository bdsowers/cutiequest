using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private EnemyAI mEnemyAI;

    private Killable mKillable;

    public float actionCooldown;
    private float mActionCooldownTimer = -1;

    // Start is called before the first frame update
    void Start()
    {
        mEnemyAI = GetComponent<EnemyAI>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mKillable = GetComponent<Killable>();

        mTurnBasedMovement.onTurnGranted += OnTurnGranted;

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
        if (mActionCooldownTimer > 0f)
            return false;
        if (mSimpleMovement.isMoving)
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

        mEnemyAI.UpdateAI();
    }
}
