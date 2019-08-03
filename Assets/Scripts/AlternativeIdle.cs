using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeIdle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInChildren<Animator>().Play("Idle 0");
    }
}
