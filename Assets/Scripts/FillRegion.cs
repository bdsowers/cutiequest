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

    List<float> CreateYValuesToAvoidFighting()
    {
        List<float> validValues = new List<float>();
        int numValues = 3000;
        float inc = heightDeviation / numValues;
        for (int i = 0; i < numValues; ++i)
        {
            validValues.Add(i * inc);
        }

        // Shuffle the big array a bunch...
        for (int i = 0; i < numValues; ++i)
        {
            int pos1 = Random.Range(0, numValues);
            int pos2 = Random.Range(0, numValues);

            float temp = validValues[pos1];
            validValues[pos1] = validValues[pos2];
            validValues[pos2] = temp;
        }

        return validValues;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector3 min = GetComponent<BoxCollider>().bounds.min;
        Vector3 max = GetComponent<BoxCollider>().bounds.max;

        int yIndex = 0;
        List<float> yValues = CreateYValuesToAvoidFighting();

        for (float x = min.x; x < max.x; x += separation.x)
        {
            for (float z = min.z; z < max.z; z += separation.y)
            {
                GameObject obj = GameObject.Instantiate(fillPrefabs.Sample(), transform);
                obj.transform.localScale = new Vector3(scale.x, 1f, scale.y);
                obj.transform.localPosition = new Vector3(x, transform.position.y + yValues[yIndex++], z);

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
