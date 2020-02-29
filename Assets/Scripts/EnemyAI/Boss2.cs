using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using DG.Tweening;
using VectorExtensions;

public class Boss2 : EnemyAI
{
    // Core actions:
    //  * Teleport around wildly
    //  * Teleport far away, charge forward, deal massive damage
    //  * Teleport far away, cast screen-filling spell with narrow cut-out
    //  * Cast screen-filling spell with rigid pattern

    // Progression:
    //  * Gets faster at each stage

    Enemy mEnemy;

    CharacterStatistics mStatistics;
    Killable mKillable;
    EnemyAI mCurrentActiveModule;
    EnemyTeleport mTeleport;
    SimpleMovement mSimpleMovement;
    SimpleAttack mSimpleAttack;
    Animator mAnimator;

    public SpellCaster mSpell1;
    public SpellCaster mSpell2;

    public enum AIState
    {
        Teleporting,
        Charging,
        ForwardCast,
        ScreenCast,
        Stun,
    }

    int mCurrentPhase = 0;

    private AIState mCurrentState = AIState.Teleporting;
    private int mNumTeleports = 0;
    private Vector3 mChargeDirection;
    private float mStunTimer = 0f;

    private bool mTeleportedIntoPlaceForState = false;

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
        mStatistics = GetComponent<CharacterStatistics>();
        mKillable = GetComponent<Killable>();
        mTeleport = GetComponent<EnemyTeleport>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mAnimator = GetComponentInChildren<Animator>();

        mEnemy.SetEnemyAI(this);
        mKillable.onHit += OnHit;
        mKillable.onDeath += OnDeath;

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

        Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
        Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);
    }

    private void OnEnable()
    {
        // Reveal the whole map when this boss becomes active
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }

    private void OnDeath(Killable entity)
    {
        // Kill all the enemies, make the player invulnerable
        Game.instance.avatar.GetComponent<Killable>().invulnerable = true;

        Enemy[] allEnemies = GameObject.FindObjectsOfType<Enemy>();
        for (int i = 0; i < allEnemies.Length; ++i)
        {
            allEnemies[i].GetComponent<Killable>().showNumberPopups = false;
            allEnemies[i].GetComponent<Killable>().TakeDamage(null, 10000, DamageReason.ForceKill);
        }

        Game.instance.cinematicDirector.PostCinematicEvent("boss2_death");

        Invoke("SpawnTeleporter", 3f);
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
            mSpell1.castSpeed *= 1.25f;
            mSpell2.castSpeed *= 1.25f;
        }
        else if (mCurrentPhase == 2)
        {
            mTeleport.teleportCooldown /= 2;
            mSpell1.castSpeed *= 1.25f;
            mSpell2.castSpeed *= 1.25f;
        }
    }

    public override bool CanUpdateAI()
    {
        if (!this.enabled)
            return false;
        if (mKillable.isDead)
            return false;
        if (mSimpleMovement.isMoving)
            return false;
        if (mSimpleAttack.isAttacking)
            return false;
        if (mSpell1.isCasting)
            return false;
        if (mSpell2.isCasting)
            return false;

        return true;
    }

    public override void UpdateAI()
    {
        if (mCurrentState == AIState.Stun)
        {
            if (mStunTimer > 2f)
            {
                SwitchState();
            }
        }
        else if (!mTeleportedIntoPlaceForState)
        {
            mTeleport.Teleport(true);

            mTeleportedIntoPlaceForState = true;

            // If we're going into the charging state, get ready to head toward the player ...
            mChargeDirection = TowardPlayer();
        }
        else if (mCurrentState == AIState.Teleporting)
        {
            if (mTeleport.CanTeleport())
            {
                ++mNumTeleports;
                int maxTeleports = 5 + mCurrentPhase;

                mTeleport.Teleport();
                SimpleMovement.OrientToDirection(mSimpleMovement.subMesh, TowardPlayer());

                if (mNumTeleports >= maxTeleports)
                {
                    SwitchState();
                }
            }

        }
        else if (mCurrentState == AIState.Charging)
        {
            if (mSimpleAttack.CanAttack(mChargeDirection))
            {
                mSimpleAttack.Attack(mChargeDirection);
                Camera.main.GetComponent<FollowCamera>().SimpleShake();

                SwitchState();
            }
            else if (mSimpleMovement.CanMove(mChargeDirection))
            {
                mSimpleMovement.Move(mChargeDirection);
            }
            else
            {
                mStunTimer = 0f;
                mCurrentState = AIState.Stun;

                Camera.main.GetComponent<FollowCamera>().SimpleShake();
            }
        }
        else if (mCurrentState == AIState.ForwardCast)
        {
            SimpleMovement.OrientToDirection(mSimpleMovement.subMesh, TowardPlayer());

            mSpell2.CastSpell(20);

            SwitchState();
        }
        else if (mCurrentState == AIState.ScreenCast)
        {
            SimpleMovement.OrientToDirection(mSimpleMovement.subMesh, TowardPlayer());

            mSpell1.CastSpell(20);

            SwitchState();
        }
    }

    private void SwitchState()
    {
        AIState newState = mCurrentState;

        // Don't repeat states
        // Charging requires extra love since it transitions immediately into stun
        while (newState == mCurrentState || (mCurrentState == AIState.Stun && newState == AIState.Charging))
        {
            int val = Random.Range(0, 4);
            switch(val)
            {
                case 0: newState = AIState.Teleporting; break;
                case 1: newState = AIState.ScreenCast; break;
                case 2: newState = AIState.ForwardCast; break;
                case 3: newState = AIState.Charging; break;
            }
        }

        mCurrentState = newState;
        mNumTeleports = 0;

        Debug.Log(mCurrentState);

        mTeleportedIntoPlaceForState = false;
    }

    private Vector3 TowardPlayer()
    {
        Vector3 towardPlayer = Game.instance.avatar.transform.position - transform.position;
        towardPlayer.y = 0f;

        if (Mathf.Abs(towardPlayer.x) > Mathf.Abs(towardPlayer.z))
            towardPlayer.z = 0;
        else
            towardPlayer.x = 0;

        if (towardPlayer.magnitude < 0.1f)
            towardPlayer = Vector3.right;

        towardPlayer.Normalize();
        return towardPlayer;
    }

    public override void AIStructureChanged()
    {
    }

    private void OnDestroy()
    {
        if (Game.instance != null && Game.instance.hud != null)
            Game.instance.hud.bossHealth.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mCurrentState == AIState.Stun)
        {
            mStunTimer += Time.deltaTime;
            mAnimator.Play("Dizzy");
        }

        Game.instance.hud.bossHealth.SetWithValues(0, mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject), mKillable.health);
    }
}
