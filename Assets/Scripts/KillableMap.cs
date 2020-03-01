using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OMM.RDG;

/// <summary>
/// Each frame, stuffs each Killable into a 2d grid the same size as the dungeon so that they're
/// faster to query. Setup so this query is done before anything else to ensure that it's fresh
/// when the time comes.
/// </summary>
public class KillableMap : MonoBehaviour
{
    private Killable[,] mMap = null;
    private static KillableMap mInstance;
    private List<Killable> mAllKillables = new List<Killable>();

    public static KillableMap instance {  get { return mInstance; } }

    public List<Killable> allKillables
    {
        get { return mAllKillables; }
    }

    private void Awake()
    {
        if (mInstance == null)
            mInstance = this;
    }

    public void SetupWithDungeon(RandomDungeon dungeon)
    {
        mMap = new Killable[dungeon.width, dungeon.height];
    }

    public Killable KillableAtWorldPosition(Vector3 worldPosition)
    {
        int x = Mathf.RoundToInt(worldPosition.x);
        int y = Mathf.RoundToInt(worldPosition.z);
        y = -y;

        if (x < 0 || y < 0 || x >= mMap.GetLength(0) || y >= mMap.GetLength(1))
            return null;

        return mMap[x, y];
    }

    public void RegisterKillable(Killable killable)
    {
        if (killable.killableMapIndex != -1)
        {
            Debug.LogError("Re-registering a killable for some reason");
            return;
        }

        mAllKillables.Add(killable);
        killable.killableMapIndex = mAllKillables.Count - 1;
    }

    public void UnregisterKillable(Killable killable)
    {
        if (killable.killableMapIndex == -1)
        {
            Debug.LogError("Trying to double-unregister a killable for some reason");
            return;
        }

        // Pop & Swap
        Killable last = mAllKillables[mAllKillables.Count - 1];
        int removeIndex = killable.killableMapIndex;
        mAllKillables[removeIndex] = last;
        killable.killableMapIndex = -1;
        mAllKillables.RemoveAt(mAllKillables.Count - 1);
    }

    private void Update()
    {
        if (mMap == null)
            return;

        int width = mMap.GetLength(0);
        int height = mMap.GetLength(1);
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                mMap[x, y] = null;
            }
        }

        for (int i = 0; i < mAllKillables.Count; ++i)
        {
            Vector3 worldPosition = mAllKillables[i].transform.position;
            int x = Mathf.RoundToInt(worldPosition.x);
            int y = Mathf.RoundToInt(worldPosition.z);
            y = -y;

            mMap[x, y] = mAllKillables[i];
        }
    }
}
