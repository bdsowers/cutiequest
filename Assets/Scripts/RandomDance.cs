using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDance : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int dance = Random.Range(1, 5);
        GetComponentInChildren<Animator>().Play("Dance" + dance);
    }
}
