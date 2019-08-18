using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo bdsowers - this whole thing is cut while I work out a way to get the animations smoother
public class ZombieQuirk : Quirk
{
    void Start()
    {
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        if (!enemy.isBoss)
        {
            enemy.GetComponent<Killable>().deathResponse = Killable.DeathResponse.MakeInactiveSilent;
            enemy.gameObject.AddComponent<Zombie>();
        }
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
