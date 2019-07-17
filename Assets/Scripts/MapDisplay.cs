using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MapDisplayType
{
    Floor,
    Wall,
    Player,
    Enemy,
    Exit,
    Shop,
    Chest,
    Secret
}

public class MapDisplay : MonoBehaviour
{
    public MapDisplayType displayType;
    public bool isSelected
    {
        get { return mIsSelected; }
        set
        {
            if (mIsSelected != value)
            {
                mIsSelected = value;

                if (mIsSelected)
                {
                    mTeleportIndicator = PrefabManager.instance.InstantiatePrefabByName("MapTeleportIndicator", transform);
                    mTeleportIndicator.transform.position = mTeleportIndicator.transform.position + Vector3.down * 0.01f;
                }
                else
                {
                    if (mTeleportIndicator != null)
                    {
                        Destroy(mTeleportIndicator);
                        mTeleportIndicator = null;
                    }
                }
            }
        }
    }

    private bool mIsSelected;
    private GameObject mTeleportIndicator;

    private Dictionary<MapDisplayType, string> mPrefabMap = new Dictionary<MapDisplayType, string>()
    {
        { MapDisplayType.Floor, "MapFloor" },
        { MapDisplayType.Wall, "MapWall" },
        { MapDisplayType.Player, "MapPlayer" },
        { MapDisplayType.Enemy, "MapEnemy" },
        { MapDisplayType.Exit, "MapExit" },
        { MapDisplayType.Shop, "MapShop" },
        { MapDisplayType.Chest, "MapChest" },
        { MapDisplayType.Secret, "MapSecret" },
    };

    void Start()
    {
        // Create the minimap element and make it a child of this
        string prefabName = null;
        if (mPrefabMap.TryGetValue(displayType, out prefabName))
        {
            PrefabManager.instance.InstantiatePrefabByName(prefabName, transform);
        }
    }

    private void Update()
    {
        
    }
}
