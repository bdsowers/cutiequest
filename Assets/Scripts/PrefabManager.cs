using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject[] characterPrefabs;
    public GameObject[] itemPrefabs;

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

        AddListToDictionary(prefabs);
        AddListToDictionary(characterPrefabs);
        AddListToDictionary(itemPrefabs);
    }

    private void AddListToDictionary(GameObject[] list)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            mPrefabMap[list[i].name] = list[i];
        }
    }
}
