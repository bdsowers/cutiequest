using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicId : MonoBehaviour
{
    public string cinematicId;

    public static GameObject FindObjectWithId(string id)
    {
        CinematicId[] avaialble = GameObject.FindObjectsOfType<CinematicId>();
        for (int i = 0; i < avaialble.Length; ++i)
        {
            Debug.Log(avaialble[i].cinematicId);
            if (avaialble[i].cinematicId == id)
            {
                return avaialble[i].gameObject;
            }
        }

        return null;
    }
}
