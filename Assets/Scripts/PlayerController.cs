using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Follower follower;

    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;

    // Start is called before the first frame update
    void Start()
    {
        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();

        mTurnBasedMovement.ActivateTurnMovement();
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

    // Update is called once per frame
    void Update()
    {
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
