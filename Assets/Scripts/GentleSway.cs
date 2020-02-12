using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleSway : MonoBehaviour
{
    private Vector3 mStartPosition;
    private float mTimer = 0f;
    private float mSpeed;
    private float mStrength = 5f;

    // Start is called before the first frame update
    void Start()
    {
        mStartPosition = transform.position;

        mSpeed = Random.Range(0.2f, 1.0f);
        mTimer = Random.Range(0f, 200f);
    }

    // Update is called once per frame
    void Update()
    {
        mTimer += Time.deltaTime * mSpeed;

        transform.position = mStartPosition + Vector3.right * Mathf.Sin(mTimer) * mStrength + Vector3.up * Mathf.Cos(mTimer / 2f) * 5f;
    }
}
