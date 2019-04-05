using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTimeElapsed : MonoBehaviour
{
    public float time;
    private float mTimer = -1f;

    private void Start()
    {
        mTimer = time;
    }

    // Update is called once per frame
    void Update()
    {
        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            Destroy(gameObject);
        }
    }
}
