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

    int mCurrentPhase = 0;

    protected override void Start()
    {
        base.Start();

        mEnemy = GetComponent<Enemy>();
        mTeleport = GetComponent<EnemyTeleport>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();

        mEnemy.SetEnemyAI(this);
        mKillable.onHit += OnHit;

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

        Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
        Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);
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
        int prevPhase = mCurrentPhase;

        if (mKillable.health < mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject) * 0.33f)
            mCurrentPhase = 2;
        else if (mKillable.health < mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject) * 0.66f)
            mCurrentPhase = 1;
        else
            mCurrentPhase = 0;

        if (prevPhase == mCurrentPhase)
            return;

        if (mCurrentPhase == 0)
            Game.instance.hud.bossHealth.fullImage.color = new Color(121 / 255.0f, 100 / 255.0f, 64 / 255.0f);
        else if (mCurrentPhase == 1)
            Game.instance.hud.bossHealth.fullImage.color = new Color(1, 1, 0);
        else
            Game.instance.hud.bossHealth.fullImage.color = new Color(1, 0, 0);

        if (mCurrentPhase == 0)
        {

        }
        else if (mCurrentPhase == 1)
        {
            mTeleport.teleportCooldown /= 2;
        }
        else if (mCurrentPhase == 2)
        {
            mTeleport.teleportCooldown /= 2;
        }
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
