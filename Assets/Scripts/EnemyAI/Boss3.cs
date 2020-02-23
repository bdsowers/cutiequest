using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using DG.Tweening;
using VectorExtensions;

// Base class that overseas the multiple enemies that compose Boss3
public abstract class Boss3 : EnemyAI
{
    protected CharacterStatistics mStatistics;
    protected Killable mKillable;

    List<Boss3> subBosses = new List<Boss3>();
    int totalHealth;

    private static bool healthShown = false;

    protected virtual void Start()
    {
        mStatistics = GetComponent<CharacterStatistics>();
        mKillable = GetComponent<Killable>();
        mKillable.onDeath += OnDeath;

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

        Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
        Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);

        GameObject[] allEnemiesInScene = GameObject.FindGameObjectsWithTag("Boss3");
        for (int i = 0; i < allEnemiesInScene.Length; ++i)
        {
            Boss3 comp = allEnemiesInScene[i].GetComponentInChildren<Boss3>(true);
            subBosses.Add(comp);
        }

        totalHealth = 0;
        for (int i = 0; i < subBosses.Count; ++i)
        {
            totalHealth += subBosses[i].MaxHealth();
        }

        if (!healthShown)
        {
            healthShown = true;

            Game.instance.hud.bossHealth.gameObject.SetActive(true);
            Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);

            Game.instance.hud.bossHealth.transform.localScale = Vector3.zero;
            Game.instance.hud.bossHealth.transform.DOScale(1f, 0.5f);
        }
    }

    private static bool enableCalled = false;
    private void OnEnable()
    {
        if (enableCalled) return;
        enableCalled = true;

        // Reveal the whole map when this boss becomes active
        RevealWhenAvatarIsClose[] revealers = GameObject.FindObjectsOfType<RevealWhenAvatarIsClose>();
        for (int i = 0; i < revealers.Length; ++i)
        {
            revealers[i].Reveal();
        }
    }

    private bool AllBossesDead()
    {
        for (int i = 0; i < subBosses.Count; ++i)
        {
            if (subBosses[i] != null && !subBosses[i].IsDead())
            {
                return false;
            }
        }

        return true;
    }

    private void OnDeath(Killable entity)
    {
        if (AllBossesDead())
        {
            // Kill all the enemies, make the player invulnerable
            Game.instance.avatar.GetComponent<Killable>().invulnerable = true;

            Game.instance.cinematicDirector.PostCinematicEvent("boss3_death");

            Invoke("SpawnTeleporter", 3f);
        }
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

    protected Vector3 TowardPlayer()
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

    private void OnDestroy()
    {
        Game.instance.hud.bossHealth.gameObject.SetActive(false);
    }

    protected int MaxHealth()
    {
        if (mStatistics == null)
            mStatistics = GetComponent<CharacterStatistics>();

        return mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject);
    }

    protected int CurrentHealth()
    {
        return mKillable.health;
    }

    protected bool IsDead()
    {
        return mKillable.isDead;
    }

    protected virtual void Update()
    {
        int remainingHealth = 0;
        for (int i = 0; i < subBosses.Count; ++i)
        {
            Boss3 subBoss = subBosses[i];
            remainingHealth += (subBoss == null || subBoss.IsDead() ? 0 : subBoss.CurrentHealth());
        }

        Game.instance.hud.bossHealth.SetWithValues(0, totalHealth, remainingHealth);
    }
}
