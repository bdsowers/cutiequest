using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject[] characterPrefabs;
    public GameObject[] itemPrefabs;
    public GameObject[] shrinePrefabs;
    public GameObject[] spellEffectPrefabs;
    public GameObject[] vfxPrefabs;
    public GameObject[] debrisPrefabs;

    private Dictionary<string, GameObject> mPrefabMap = new Dictionary<string, GameObject>();
    private static PrefabManager sInstance = null;

    public static PrefabManager instance {  get { return sInstance; } }

    public GameObject PrefabByName(string name)
    {
        return mPrefabMap[name];
    }

    public GameObject InstantiatePrefabByName(string name, Transform parent = null)
    {
        GameObject instance = GameObject.Instantiate(PrefabByName(name), parent);

        return instance;
    }

    private void Awake()
    {
        sInstance = this;

        AddListToDictionary(prefabs);
        AddListToDictionary(characterPrefabs);
        AddListToDictionary(itemPrefabs);
        AddListToDictionary(shrinePrefabs);
        AddListToDictionary(spellEffectPrefabs);
        AddListToDictionary(vfxPrefabs);
        AddListToDictionary(debrisPrefabs);
    }

    private void AddListToDictionary(GameObject[] list)
    {
        for (int i = 0; i < list.Length; ++i)
        {
            mPrefabMap[list[i].name] = list[i];
        }
    }
}
