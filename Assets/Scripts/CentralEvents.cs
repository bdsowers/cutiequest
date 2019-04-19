using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo bdsowers - I'm not super happy with this rig, hoping to keep its use limited,
// but if it bloats I want to do something else.
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

    public delegate void EnemyHit(Enemy enemy, int damage);
    public event EnemyHit onEnemyHit;
    public void FireEnemyHit(Enemy enemy, int damage)
    {
        if (onEnemyHit != null)
        {
            onEnemyHit(enemy, damage);
        }
    }
}
