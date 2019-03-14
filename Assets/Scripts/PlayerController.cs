﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Follower follower;

    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;
    private ExternalCharacterStatistics mCharacterStats;

    // Start is called before the first frame update
    void Start()
    {
        mCharacterStats = GetComponent<ExternalCharacterStatistics>();
        mCharacterStats.externalReference = Game.instance.playerStats;
        follower.GetComponent<ExternalCharacterStatistics>().externalReference = Game.instance.playerStats;

        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();

        mTurnBasedMovement.ActivateTurnMovement();
        mSimpleMovement.onMoveFinished += OnMoveFinished;
        mSimpleAttack.onAttackFinished += OnAttackFinished;

        OnFollowerChanged();
    }

    void OnFollowerChanged()
    {
        AttachFollowerSpell();
    }

    private void AttachFollowerSpell()
    {
        Spell oldSpell = GetComponentInChildren<Spell>();
        if (oldSpell != null)
        {
            Destroy(oldSpell.gameObject);
        }

        if (Game.instance.playerData.followerUid != null)
        {
            CharacterData followerData = Game.instance.characterDataList.CharacterWithUID(Game.instance.playerData.followerUid);
            if (followerData.spell != null)
            {
                GameObject spell = GameObject.Instantiate(followerData.spell.gameObject, transform);
            }
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

    private void CastSpellIfPossible()
    {
        Spell spell = GetComponentInChildren<Spell>();
        if (spell != null && spell.canActivate)
        {
            spell.Activate(gameObject);
        }

        spell = follower.GetComponentInChildren<Spell>();
        if (spell != null && spell.canActivate)
        {
            spell.Activate(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // todo bdsowers - these need to be queued up for when the player movement ends
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CastSpellIfPossible();
        }

        if (mSimpleMovement.isMoving || mSimpleAttack.isAttacking)
            return;

        // Disable for real-time play
        if (!mTurnBasedMovement.isMyTurn && !Game.instance.realTime)
            return;

        Vector3 followerDirection = transform.position - follower.transform.position;
        followerDirection.y = 0f;
        followerDirection.Normalize();

        Vector3 intendedDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            intendedDirection = new Vector3(0f, 0f, 1f);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            intendedDirection = new Vector3(0f, 0f, -1f);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            intendedDirection = new Vector3(-1f, 0f, 0f);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            intendedDirection = new Vector3(1f, 0f, 0f);
        }

        if (intendedDirection.magnitude > 0.8f)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, intendedDirection);
            }
            else
            {
                if (mSimpleAttack.CanAttack(intendedDirection))
                {
                    mSimpleAttack.Attack(intendedDirection);
                }
                else if (mSimpleMovement.CanMove(intendedDirection))
                {
                    mSimpleMovement.Move(intendedDirection);
                    MoveFollower(followerDirection);
                }
            }
        }
    }

    void MoveFollower(Vector3 direction)
    {
        StartCoroutine(MoveFollowerCoroutine(direction, 0.1f));
    }

    IEnumerator MoveFollowerCoroutine(Vector3 direction, float delay)
    {
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        follower.GetComponent<SimpleMovement>().Move(direction);
        yield break;
    }
}
