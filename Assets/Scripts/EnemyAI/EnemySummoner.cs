using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySummoner : EnemyAI
{
    Summoner mSummoner;
    SimpleMovement mSimpleMovement;

    public float cooldown;
    private float mSummonCooldownTimer;

    private GameObject target
    {
        get
        {
            return Decoy.instance != null ? Decoy.instance.gameObject : Game.instance.avatar.gameObject;
        }
    }

    private void Start()
    {
        mSummoner = GetComponentInChildren<Summoner>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSummonCooldownTimer = Random.Range(1f, 5f);
    }

    public override void AIStructureChanged()
    {
        mSummoner = GetComponentInChildren<Summoner>();
    }

    public override void UpdateAI()
    {
        Vector3 largeDistanceDirection = OrthogonalDirection(transform, target.transform, true);
        Vector3 smallDistanceDirection = OrthogonalDirection(transform, target.transform, false);
        float distance = Vector3.Distance(transform.position, target.transform.position);

        // todo PRERELEASE - this logic will need some refinement
        if (Random.Range(0, 2) == 0 && SummonAllowed())
        {
            // Summoners try to keep their distance, but will also summon when appropriate.

            bool run = Random.Range(0, 2) == 0;

            if (run && mSimpleMovement.CanMove(-largeDistanceDirection))
            {
                mSimpleMovement.Move(-largeDistanceDirection);
            }
            else if (run && mSimpleMovement.CanMove(-smallDistanceDirection))
            {
                mSimpleMovement.Move(-smallDistanceDirection);
            }
            else if (mSummonCooldownTimer <= 0f)
            {
                Summon();
            }
        }
        else if (mSimpleMovement.CanMove(largeDistanceDirection))
        {
            mSimpleMovement.Move(largeDistanceDirection);
        }
        else
        {
            if (mSimpleMovement.CanMove(smallDistanceDirection))
            {
                mSimpleMovement.Move(smallDistanceDirection);
            }
        }
    }

    private void Update()
    {
        if (mSummonCooldownTimer > 0f && parentEnemy.activated)
            mSummonCooldownTimer -= Time.deltaTime;
    }

    private void Summon()
    {
        mSummonCooldownTimer = cooldown;

        mSummoner.CastSummon();
    }

    private bool SummonAllowed()
    {
        // Don't allow too many enemies... this number was picked pretty arbitrarily based
        // on balance though.
        return Game.instance.enemyDirector.NumEnemies < 72;
    }

    public override bool CanUpdateAI()
    {
        if (mSummoner != null && mSummoner.isSummoning)
            return false;

        return true;
    }
}
