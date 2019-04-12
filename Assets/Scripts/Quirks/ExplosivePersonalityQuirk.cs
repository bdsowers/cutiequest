using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosivePersonalityQuirk : Quirk
{
    void Start()
    {
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        DropsItems di = enemy.gameObject.AddComponent<DropsItems>();

        DropsItems.DropData bombData = new DropsItems.DropData();
        bombData.itemName = "Bomb";
        bombData.rate = 33;
        bombData.amount = 1;

        di.arbitraryDrops = new DropsItems.DropData[] { bombData };
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
