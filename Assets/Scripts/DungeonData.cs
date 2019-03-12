using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

[System.Serializable]
public struct DungeonEnemyData
{
    public int minEnemies;
    public int maxEnemies;

    public GameObject commonEnemy;
    public GameObject uncommonEnemy;
    public GameObject rareEnemy;
    public GameObject scaryEnemy;
}

[System.Serializable]
public struct DungeonFloorData
{
    public string roomSet;
    public RandomDungeonGenerationData generationData;
    public DungeonEnemyData enemyData;
}

[CreateAssetMenu(fileName = "Character", menuName = "CutieQuest/DungeonData")]
public class DungeonData : ScriptableObject
{
    public DungeonFloorData[] floorData;
}
