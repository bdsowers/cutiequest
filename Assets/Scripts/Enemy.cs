using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;
    private Killable mKillable;

    public float actionCooldown;
    private float mActionCooldownTimer = -1;

    // Start is called before the first frame update
    void Start()
    {
        mSimpleMovement = GetComponent<SimpleMovement>();
        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mKillable = GetComponent<Killable>();

        mTurnBasedMovement.onTurnGranted += OnTurnGranted;
        mSimpleMovement.onMoveFinished += OnMoveFinished;
        mSimpleAttack.onAttackFinished += OnAttackFinished;
        mKillable.onDeath += OnDeath;
    }

    private void OnDeath(Killable entity)
    {
        // todo bdsowers - random chance to drop coins & hearts based on player luck
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

                if (!mSimpleMovement.isMoving && !mSimpleAttack.isAttacking && mActionCooldownTimer <= 0f)
                {
                    mActionCooldownTimer = actionCooldown;
                    UpdateAI();
                }
            }
        }
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
        if (!Game.instance.avatar.isAlive)
            return;

        Vector3 direction = OrthogonalDirection(transform, Game.instance.avatar.transform, true);
        
        if (mSimpleAttack.CanAttack(direction))
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
