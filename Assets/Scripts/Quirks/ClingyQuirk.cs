using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingyQuirk : Quirk
{
    public override void Start()
    {
        base.Start();
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        CharacterStatModifier modifier = enemy.gameObject.AddComponent<CharacterStatModifier>();
        modifier.SetRelativeModification(CharacterStatType.Speed, 4);
        enemy.actionCooldown /= 2;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
