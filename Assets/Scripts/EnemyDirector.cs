using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDirector : MonoBehaviour
{
    List<Enemy> mEnemies = new List<Enemy>();

    private bool isMyTurn = false;

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateEnemies();
    }

    void UpdateEnemies()
    {
        for (int i = 0; i < mEnemies.Count; ++i)
        {
            mEnemies[i].UpdateEnemy();
        }
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
