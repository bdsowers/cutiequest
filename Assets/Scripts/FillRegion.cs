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

    public bool fillGradually { get; set; }

    private BoxCollider mBoxCollider;

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
        mBoxCollider = GetComponent<BoxCollider>();

        if (fillGradually)
        {
            StartCoroutine(FillWithPrefabsCoroutine());
        }
        else
        {
            FillWithPrefabs();
        }
    }

    void FillWithPrefabs()
    {
        Vector3 min = mBoxCollider.bounds.min;
        Vector3 max = mBoxCollider.bounds.max;
        mBoxCollider.enabled = false;

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
    }

    IEnumerator FillWithPrefabsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        Vector3 min = mBoxCollider.bounds.min;
        Vector3 max = mBoxCollider.bounds.max;
        mBoxCollider.enabled = false;

        int yIndex = 0;
        List<float> yValues = CreateYValuesToAvoidFighting();
        List<Vector3> positions = new List<Vector3>();

        for (float x = min.x; x < max.x; x += separation.x)
        {
            for (float z = min.z; z < max.z; z += separation.y)
            {
                positions.Add(new Vector3(x, transform.position.y + yValues[yIndex++], z));
            }
        }

        positions.Shuffle();

        for (int i = 0; i < positions.Count; ++i)
        {
            GameObject obj = GameObject.Instantiate(fillPrefabs.Sample(), transform);
            obj.transform.localScale = new Vector3(scale.x, 1f, scale.y);
            obj.transform.localPosition = positions[i];

            if (randomRotation)
            {
                obj.transform.localRotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
            }

            yield return null;
        }

        yield break;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
