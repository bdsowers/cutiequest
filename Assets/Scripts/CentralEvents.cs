using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentralEvents
{
    public delegate void EnemyCreated(Enemy enemy);
    public event EnemyCreated onEnemyCreated;
    public void FireEnemyCreated(Enemy enemy)
    {
        if (onEnemyCreated != null)
        {
            onEnemyCreated(enemy);
        }
    }

    public delegate void SceneChanged(string newScene);
    public event SceneChanged onSceneChanged;
    public void FireSceneChanged(string newScene)
    {
        if (onSceneChanged != null)
        {
            onSceneChanged(newScene);
        }
    }

    public delegate void EnemyHit(Enemy enemy, int damage, DamageReason damageSource);
    public event EnemyHit onEnemyHit;
    public void FireEnemyHit(Enemy enemy, int damage, DamageReason damageSource)
    {
        if (onEnemyHit != null)
        {
            onEnemyHit(enemy, damage, damageSource);
        }
    }

    public delegate void PlayerHit(GameObject damageSource, int damage, DamageReason damageReason);
    public event PlayerHit onPlayerHit;
    public void FirePlayerHit(GameObject damageSource, int damage, DamageReason damageReason)
    {
        if (onPlayerHit != null)
        {
            onPlayerHit(damageSource, damage, damageReason);
        }
    }
}
