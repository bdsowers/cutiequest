using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantBob : MonoBehaviour
{
    private float mTime = 0f;
    private float mSpeed = 0f;

    private void Start()
    {
        mTime = Random.Range(0f, 1f);
        mSpeed = Random.Range(0.15f, 0.35f);
    }

    // Update is called once per frame
    void Update()
    {
        mTime += Time.deltaTime * mSpeed;
        transform.localPosition = Vector3.zero + Vector3.up * Mathf.PingPong(mTime, 0.25f);
    }
}
