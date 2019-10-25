using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieQuirk : Quirk
{
    public override void Start()
    {
        base.Start();

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

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
