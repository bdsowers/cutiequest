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
}
