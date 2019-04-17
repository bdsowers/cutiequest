using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancePartyQuirk : Quirk
{
    private static bool mEnabled;
    public static bool quirkEnabled {  get { return mEnabled; } }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        mEnabled = true;
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        enemy.GetComponentInChildren<Animator>().SetBool("Dancing", true);
        enemy.GetComponentInChildren<Animator>().SetInteger("DanceNumber", Random.Range(0, 4));
    }

    private void OnDestroy()
    {
        mEnabled = false;
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
