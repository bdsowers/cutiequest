using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorTile : MonoBehaviour
{
    public List<GameObject> removableDecroations;

    public void RemoveDecorations()
    {
        for (int i = 0; i < removableDecroations.Count; ++i)
        {
            removableDecroations[i].SetActive(false);
        }
    }
}
