using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancePartyQuirk : Quirk
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        // note bdsowers - cut
        //enemy.GetComponentInChildren<Animator>().SetBool("Dancing", true);
        //enemy.GetComponentInChildren<Animator>().SetInteger("DanceNumber", Random.Range(0, 4));
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;
    }
}
