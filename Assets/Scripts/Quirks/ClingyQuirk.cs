using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClingyQuirk : Quirk
{
    private static bool mEnabled;
    public static bool quirkEnabled { get { return mEnabled; } }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    void Start()
    {
        mEnabled = true;
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        CharacterStatModifier modifier = enemy.gameObject.AddComponent<CharacterStatModifier>();
        modifier.SetRelativeModification(CharacterStatType.Speed, 4);
        enemy.actionCooldown /= 2;
    }

    private void OnDestroy()
    {
        mEnabled = false;
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
