using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokerQuirk : Quirk
{
    public GameObject smokePrefab;

    private GameObject mSmoke;

    public override void Start()
    {
        base.Start();

        mSmoke = GameObject.Instantiate(smokePrefab);    
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        Destroy(mSmoke);
    }
}
