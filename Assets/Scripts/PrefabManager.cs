using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    private Dictionary<string, GameObject> mPrefabMap = new Dictionary<string, GameObject>();
    private static PrefabManager sInstance = null;

    public static PrefabManager instance {  get { return sInstance; } }

    public GameObject PrefabByName(string name)
    {
        return mPrefabMap[name];
    }

    private void Awake()
    {
        sInstance = this;

        for (int i = 0; i < prefabs.Length; ++i)
        {
            mPrefabMap[prefabs[i].name] = prefabs[i];
        }
    }
}
