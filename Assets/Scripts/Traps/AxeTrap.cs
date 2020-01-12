using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class AxeTrap : PlacedTrap
{
    public GameObject[] groups;

    public override bool CanSpawn(List<Vector2Int> region)
    {
        return true;
    }

    public override void Spawn(List<Vector2Int> region, LevelGenerator levelGenerator)
    {
        int maxTries = 50;
        while (maxTries > 0)
        {
            Vector2Int pos = region.Sample();
            maxTries--;

            if (levelGenerator.collisionMap.SpaceMarking(pos.x, pos.y) == 0)
            {
                // Placed successfully
                maxTries = 0;

                GameObject trapObj = levelGenerator.PlaceMapPrefab("AxeTrap", pos.x, pos.y);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int straddleMul = Random.Range(0, 2);
        for (int i = 0; i < groups.Length; ++i)
        {
            groups[i].GetComponent<AxeTrapGroup>().setup = true;
            groups[i].GetComponent<AxeTrapGroup>().delay = 1f + 1 * straddleMul * i;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
