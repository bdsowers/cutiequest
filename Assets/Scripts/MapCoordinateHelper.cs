using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class MapCoordinateHelper
{
    public static Vector2Int WorldToMapCoords(Vector3 worldCoords)
    {
        Vector2Int mapCoords = worldCoords.AsVector2IntUsingXZ();
        mapCoords.y = -mapCoords.y;

        return mapCoords;
    }

    public static Vector3 MapToWorldCoords(Vector2Int mapCoords, float y = 0.5f)
    {
        return new Vector3(mapCoords.x, y, -mapCoords.y);
    }
}
