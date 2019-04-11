using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class FaceBlindQuirk : Quirk
{
    void Start()
    {
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        Debug.Log("!!");
        enemy.GetComponentInChildren<CharacterModel>().ChangeModel(PrefabManager.instance.characterPrefabs.Sample().name);
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
