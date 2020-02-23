using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using DG.Tweening;
using VectorExtensions;

public class Boss3b : Boss3
{
    Enemy mEnemy;

    EnemyAI mCurrentActiveModule;
    EnemyTeleport mTeleport;
    SimpleMovement mSimpleMovement;
    SimpleAttack mSimpleAttack;

    protected override void Start()
    {
        base.Start();

        mEnemy = GetComponent<Enemy>();
        mTeleport = GetComponent<EnemyTeleport>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();

        mEnemy.SetEnemyAI(this);
        mKillable.onHit += OnHit;
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

        return true;
    }

    public override void UpdateAI()
    {

    }

    private void SwitchState()
    {

    }

    public override void AIStructureChanged()
    {
    }

    private void OnDestroy()
    {
        Game.instance.hud.bossHealth.gameObject.SetActive(false);
    }
}
