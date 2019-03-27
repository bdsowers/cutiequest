using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class ShrineChallenge : Shrine
{
    protected override IEnumerator ActivationCoroutine()
    {
        yield return base.ActivationCoroutine();

        if (WasAccepted())
        {
            CharacterStatModifier modifier = Game.instance.playerStats.gameObject.AddComponent<CharacterStatModifier>();
            modifier.SetRelativeModification(CharacterStatType.Strength, 2);

            CollisionMap collisionMap = GameObject.FindObjectOfType<CollisionMap>();

            // Spawn a bunch of enemies and give the player more strength.
            List<Vector2Int> walkablePositions = new List<Vector2Int>();
            Vector2Int playerPosition = Game.instance.avatar.transform.position.AsVector2IntUsingXZ();
            playerPosition.y = -playerPosition.y;

            for (int xOffset = -4; xOffset <= 4; ++xOffset)
            {
                for (int yOffset = -4; yOffset <= 4; ++yOffset)
                {
                    if (Mathf.Abs(xOffset) < 2 || Mathf.Abs(yOffset) < 2)
                        continue;

                    int x = playerPosition.x + xOffset;
                    int y = playerPosition.y + yOffset;
                    if (collisionMap.SpaceMarking(x,y) == 0)
                    {
                        walkablePositions.Add(new Vector2Int(x, y));
                    }
                }
            }

            DungeonFloorData data = CurrentDungeonFloorData();

            int numEnemies = 5;

            for (int i = 0; i < numEnemies; ++i)
            {
                if (walkablePositions.Count == 0)
                    continue;

                string enemy = data.enemyData.rareEnemy.name;
                GameObject newEnemy = GameObject.Instantiate(PrefabManager.instance.PrefabByName(enemy));
                Vector2Int pos2 = walkablePositions[Random.Range(0, walkablePositions.Count)];
                walkablePositions.Remove(pos2);
                Vector3 pos = new Vector3(pos2.x, 0.5f, -pos2.y);
                newEnemy.transform.position = pos;
                collisionMap.MarkSpace(pos2.x, pos2.y, newEnemy.GetComponent<SimpleMovement>().collisionIdentity);
            }
        }

        yield break;
    }

    private DungeonFloorData CurrentDungeonFloorData()
    {
        DungeonData dungeonData = Game.instance.currentDungeonData;
        int floorIndex = Game.instance.currentDungeonFloor - 1;
        floorIndex = Mathf.Min(floorIndex, dungeonData.floorData.Length - 1);
        return dungeonData.floorData[floorIndex];
    }
}
