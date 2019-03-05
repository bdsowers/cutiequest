﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;

    // Start is called before the first frame update
    void Start()
    {
        mSimpleMovement = GetComponent<SimpleMovement>();
        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();

        mTurnBasedMovement.onTurnGranted += OnTurnGranted;
        mSimpleMovement.onMoveFinished += OnMoveFinished;
        mSimpleAttack.onAttackFinished += OnAttackFinished;
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
        MoveTowardAvatar();
    }

    // Update is called once per frame
    void Update()
    {
        if (!mTurnBasedMovement.canTakeTurns)
        {
            float distance = Vector3.Distance(transform.position, GameObject.Find("Avatar").transform.position);
            if (distance < 8f)
            {
                mTurnBasedMovement.ActivateTurnMovement();
            }
        }

        /* Realtime movement
        if (mTurnBasedMovement.canTakeTurns)
        {
            if (!mSimpleMovement.isMoving && !mSimpleAttack.isAttacking)
                MoveTowardAvatar();
        }*/
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

    private void MoveTowardAvatar()
    {
        Vector3 direction = OrthogonalDirection(transform, GameObject.Find("Avatar").transform, true);
        
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
