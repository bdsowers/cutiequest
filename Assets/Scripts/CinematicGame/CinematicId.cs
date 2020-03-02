using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicId : MonoBehaviour
{
    public string cinematicId;
    List<CinematicId> mCinematicIdList = new List<CinematicId>();

    public static GameObject FindObjectWithId(string id)
    {
        CinematicId[] avaialble = GameObject.FindObjectsOfType<CinematicId>();

        for (int i = 0; i < avaialble.Length; ++i)
        {
            if (avaialble[i].cinematicId == id)
            {
                return avaialble[i].gameObject;
            }
        }

        return null;
    }
}
