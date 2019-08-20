using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansplainQuirk : Quirk
{
    public GameObject mansplainCanvas;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        GameObject newCanvas = GameObject.Instantiate(mansplainCanvas, enemy.transform);
        newCanvas.SetActive(true);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;   
    }
}
