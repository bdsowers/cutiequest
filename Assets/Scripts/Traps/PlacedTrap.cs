using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlacedTrap : MonoBehaviour
{
    public abstract bool CanSpawn(List<Vector2Int> region);
    public abstract void Spawn(List<Vector2Int> region, LevelGenerator levelGenerator);
}
