using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class AxeTrap : PlacedTrap
{
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
