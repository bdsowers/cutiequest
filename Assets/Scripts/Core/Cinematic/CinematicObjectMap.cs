using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CinematicNameObjectPair
{
    public string name;
    public GameObject gameObject;
}

public class CinematicObjectMap : MonoBehaviour
{
    public List<CinematicNameObjectPair> mappings = new List<CinematicNameObjectPair>();

    public GameObject GetObjectByName(string name)
    {
        for (int i = 0; i < mappings.Count; ++i)
        {
            if (mappings[i].name == name)
            {
                return mappings[i].gameObject;
            }
        }
        return null;
    }
}
