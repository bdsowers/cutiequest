using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CinematicActionSpawnPrefab : CinematicAction
{
    private string mPrefabName;
    private int mMapX;
    private int mMapY;

    public override string actionName
    {
        get { return "spawn_prefab"; }
    }

    public override void InterpretParameters(CinematicDataProvider dataProvider)
    {
        base.InterpretParameters(dataProvider);

        mPrefabName = dataProvider.GetStringData(mParameters, "prefab");
        mMapX = dataProvider.GetIntData(mParameters, "x");
        mMapY = dataProvider.GetIntData(mParameters, "y");
    }

    public override IEnumerator PlayInternal(CinematicDirector player)
    {
        Vector3 worldCoords = MapCoordinateHelper.MapToWorldCoords(new Vector2Int(mMapX, mMapY));
        GameObject newObj = PrefabManager.instance.InstantiatePrefabByName(mPrefabName);
        newObj.transform.position = worldCoords;

        // todo bdsowers - this was rushed in for a cinematic
        newObj.transform.GetChild(0).localPosition = Vector3.up * 7f;
        newObj.transform.GetChild(0).DOLocalMove(Vector3.zero, Random.Range(0.3f, 0.6f)).SetDelay(Random.Range(0.2f, 0.4f));
        GameObject.FindObjectOfType<CollisionMap>().MarkSpace(mMapX, mMapY, 1);

        yield break;
    }
}
