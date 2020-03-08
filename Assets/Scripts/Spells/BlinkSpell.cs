using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;
using VectorExtensions;
using ArrayExtensions;

public class BlinkSpell : Spell
{
    public override void Activate(GameObject caster)
    {
        base.Activate(caster);

        // Find a walkable tile nearby that is empty and move there immediately, then play an effect.
        CollisionMap collisionMap = GameObject.FindObjectOfType<CollisionMap>();

        int x = Mathf.RoundToInt(Game.instance.avatar.transform.position.x);
        int y = Mathf.RoundToInt(Game.instance.avatar.transform.position.z);
        y = -y;

        List<Vector2Int> viablePositions = new List<Vector2Int>();
        for (int xOffset = -13; xOffset <= 13; ++xOffset)
        {
            for (int yOffset = -13; yOffset <= 13; ++yOffset)
            {
                int testX = x + xOffset;
                int testY = y + yOffset;
                if (testX >= 0 && testY >= 0 && testX < collisionMap.width && testY < collisionMap.height &&
                    collisionMap.SpaceMarking(testX, testY) == 0)
                {
                    viablePositions.Add(new Vector2Int(testX, testY));
                }
            }
        }

        if (viablePositions.Count == 0)
            return;


        Vector2Int pos = viablePositions.Sample();

        collisionMap.RemoveMarking(Game.instance.avatar.commonComponents.simpleMovement.uniqueCollisionIdentity);
        collisionMap.MarkSpace(pos.x, pos.y, Game.instance.avatar.GetComponent<SimpleMovement>().uniqueCollisionIdentity);

        Game.instance.avatar.transform.position = new Vector3(pos.x, 0, -pos.y);
        Game.instance.avatar.follower.transform.position = Game.instance.avatar.transform.position + new Vector3(-0.25f, 0f, 0.25f);

        GameObject effect = PrefabManager.instance.InstantiatePrefabByName("CFX2_WWExplosion_C");
        effect.transform.position = Game.instance.avatar.transform.position;
        effect.AddComponent<DestroyAfterTimeElapsed>().time = 2f;

        Game.instance.soundManager.PlaySound("teleport");
    }
}
