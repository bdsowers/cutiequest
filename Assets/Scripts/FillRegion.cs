using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class FillRegion : MonoBehaviour
{
    public GameObject[] fillPrefabs;
    public Vector2 separation;
    public Vector2 scale;
    public float heightDeviation;
    public bool randomRotation = true;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 min = GetComponent<BoxCollider>().bounds.min;
        Vector3 max = GetComponent<BoxCollider>().bounds.max;

        for (float x = min.x; x < max.x; x += separation.x)
        {
            for (float z = min.z; z < max.z; z += separation.y)
            {
                GameObject obj = GameObject.Instantiate(fillPrefabs.Sample(), transform);
                obj.transform.localScale = new Vector3(scale.x, 1f, scale.y);
                obj.transform.localPosition = new Vector3(x, transform.position.y + Random.Range(0f, heightDeviation), z);

                if (randomRotation)
                {
                    obj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                }
            }
        }

        GetComponent<BoxCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
