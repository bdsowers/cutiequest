﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class FaceBlindQuirk : Quirk
{
    public override void Start()
    {
        base.Start();

        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        if (enemy.isBoss)
            return;

        if (enemy != null && enemy.commonComponents.characterModel != null)
        {
            enemy.commonComponents.characterModel.ChangeModel(PrefabManager.instance.characterPrefabs.Sample().name);
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
