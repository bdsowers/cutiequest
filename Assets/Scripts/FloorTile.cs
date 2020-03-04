using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public List<GameObject> removableDecroations;

    public void RemoveDecorations()
    {
        // Should never happen, but for sanity's sake...
        if (removableDecroations == null)
            return;

        for (int i = 0; i < removableDecroations.Count; ++i)
        {
            removableDecroations[i].SetActive(false);
        }
    }
}
