using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRandomChild : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }

        int randomChild = Random.Range(0, transform.childCount);
        transform.GetChild(randomChild).gameObject.SetActive(true);
    }
}
