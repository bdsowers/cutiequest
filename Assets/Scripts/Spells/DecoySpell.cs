using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class DecoySpell : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        // Find a valid spot beside the player and spawn the decoy
        CollisionMap map = GameObject.FindObjectOfType<CollisionMap>();
        if (map == null)
            return;

        Vector2Int playerPos = MapCoordinateHelper.WorldToMapCoords(Game.instance.avatar.transform.position);
        Vector2Int decoyPos = FindEmptyNearbyPosition(playerPos, map);

        GameObject decoy = PrefabManager.instance.InstantiatePrefabByName("Decoy");
        decoy.name = "Decoy";
        decoy.transform.position = MapCoordinateHelper.MapToWorldCoords(decoyPos);
        decoy.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData);
    }

    private Vector2Int FindEmptyNearbyPosition(Vector2Int sourcePos, CollisionMap collisionMap)
    {
        for (int xOffset = -1; xOffset <= 1; ++xOffset)
        {
            for (int yOffset = 1; yOffset >= -1; --yOffset)
            {
                Vector2Int pos = sourcePos + new Vector2Int(xOffset, yOffset);
                if (collisionMap.SpaceMarking(pos.x, pos.y) == 0 &&
                    Mathf.Abs(xOffset) != Mathf.Abs(yOffset))
                {
                    return pos;
                }
            }
        }

        return sourcePos;
    }
}
