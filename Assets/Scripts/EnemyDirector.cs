using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirector : MonoBehaviour
{
    List<Enemy> mEnemies = new List<Enemy>();

    private bool mUpdatingEnemies = false;

    public int NumEnemies
    {
        get { return mEnemies.Count; }
    }

    private void Awake()
    {
        mUpdatingEnemies = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.realTime)
        {
            RealTimeUpdate();
        }
        else
        {
            TurnBasedUpdate();
        }
    }

    void RealTimeUpdate()
    {
        UpdateEnemies();
    }

    void TurnBasedUpdate()
    {
        if (Game.instance.whoseTurn == 0)
            return;

        if (!mUpdatingEnemies)
        {
            mUpdatingEnemies = true;
            UpdateEnemies();
        }

        if (mUpdatingEnemies && AllEnemiesReady())
        {
            mUpdatingEnemies = false;
            Game.instance.whoseTurn = 0;
        }
    }

    void UpdateEnemies()
    {
        for (int i = 0; i < mEnemies.Count; ++i)
        {
            mEnemies[i].UpdateEnemy();
        }
    }

    bool AllEnemiesReady()
    {
        for (int i = 0; i < mEnemies.Count; ++i)
        {
            if (!mEnemies[i].ReadyForTurn())
            {
                return false;
            }
        }

        return true;
    }

    public void RegisterEnemy(Enemy enemy)
    {
        mEnemies.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        mEnemies.Remove(enemy);
    }
}
