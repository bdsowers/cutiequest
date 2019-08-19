using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokerQuirk : Quirk
{
    public GameObject smokePrefab;

    private GameObject mSmoke;

    private void Start()
    {
        mSmoke = GameObject.Instantiate(smokePrefab);    
    }

    private void OnDestroy()
    {
        Destroy(mSmoke);
    }
}
