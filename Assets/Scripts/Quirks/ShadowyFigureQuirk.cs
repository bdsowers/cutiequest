using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowyFigureQuirk : Quirk
{
    public override void Start()
    {
        base.Start();

        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        enemy.GetComponentInChildren<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
