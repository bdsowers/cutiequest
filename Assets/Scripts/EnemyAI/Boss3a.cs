using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using DG.Tweening;
using VectorExtensions;

public class Boss3a : Boss3
{
    Enemy mEnemy;

    EnemyAI mCurrentActiveModule;
    EnemyTeleport mTeleport;
    SimpleMovement mSimpleMovement;
    SimpleAttack mSimpleAttack;
    ProjectileThrower mProjectileThrower;

    public enum AIState
    {
        Teleporting,
        Throwing,
        Delay,
    }

    private AIState mCurrentState = AIState.Teleporting;

    private int mNumTeleports;
    private float mDelayTimer;
    private int mMaxTeleports;

    protected override void Start()
    {
        base.Start();

        mEnemy = GetComponent<Enemy>();
        mTeleport = GetComponent<EnemyTeleport>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mProjectileThrower = GetComponent<ProjectileThrower>();

        mEnemy.SetEnemyAI(this);
        mKillable.onHit += OnHit;

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

        Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
        Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);

        mMaxTeleports = Random.Range(4, 6);
    }

    private void SpawnTeleporter()
    {
        // Try to spawn in the center of the boss room, but make sure we spawn a healthy
        // distance away from the player so they can't accidentally leave prematurely.
        LevelGenerator generator = GameObject.FindObjectOfType<LevelGenerator>();
        Vector2Int mapPos = generator.dungeon.PositionForSpecificTile('9');
        mapPos = generator.FindEmptyNearbyPosition(mapPos);
        Vector3 worldPos = MapCoordinateHelper.MapToWorldCoords(mapPos, 0.4f);

        GameObject exit = PrefabManager.instance.InstantiatePrefabByName("DungeonExit");
        exit.transform.position = worldPos;
        exit.GetComponent<RevealWhenAvatarIsClose>().Reveal();
    }

    private void OnHit(Killable entity)
    {
        SwitchPhaseIfNecessary();
    }

    private void SwitchPhaseIfNecessary()
    {

    }

    public override bool CanUpdateAI()
    {
        if (!this.enabled)
            return false;
        if (mKillable == null || mKillable.isDead)
            return false;
        if (mSimpleMovement == null || mSimpleMovement.isMoving)
            return false;
        if (mSimpleAttack == null || mSimpleAttack.isAttacking)
            return false;
        if (mProjectileThrower == null || mProjectileThrower.isThrowing)
            return false;

        return true;
    }

    public override void UpdateAI()
    {
        if (mCurrentState == AIState.Teleporting && mTeleport.CanTeleport())
        {
            mNumTeleports++;
            mTeleport.Teleport(mNumTeleports == mMaxTeleports);

            if (mNumTeleports == mMaxTeleports)
            {
                mCurrentState = AIState.Throwing;

                mMaxTeleports = Random.Range(4, 7) + mCurrentPhase;
            }
        }
        else if (mCurrentState == AIState.Throwing)
        {
            SimpleMovement.OrientToDirection(mSimpleMovement.subMesh, TowardPlayer());
            mProjectileThrower.ThrowProjectile(20, TowardPlayer());

            mCurrentState = AIState.Delay;

            mNumTeleports = 0;
            mDelayTimer = 0f;
        }
        else if (mCurrentState == AIState.Delay)
        {
            if (mDelayTimer > 2f - mCurrentPhase * 0.5f)
            {
                mCurrentState = AIState.Teleporting;
                mNumTeleports = 0;
            }
        }
    }

    public override void AIStructureChanged()
    {
    }

    protected override void Update()
    {
        base.Update();

        if (!mProjectileThrower.isThrowing)
        {
            mDelayTimer += Time.deltaTime;
        }
    }
}
