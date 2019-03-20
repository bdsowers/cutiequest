using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MansplainQuirk : Quirk
{
    public GameObject mansplainCanvas;

    // Start is called before the first frame update
    void Start()
    {
        Game.instance.centralEvents.onEnemyCreated += OnEnemyCreated;
    }

    private void OnEnemyCreated(Enemy enemy)
    {
        GameObject newCanvas = GameObject.Instantiate(mansplainCanvas, enemy.transform);
        newCanvas.SetActive(true);
    }

    private void OnDestroy()
    {
        Game.instance.centralEvents.onEnemyCreated -= OnEnemyCreated;   
    }
}
