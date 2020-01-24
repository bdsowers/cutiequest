using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GentleWobble : MonoBehaviour
{
    Vector3 mStartPosition;
    float mTimer;

    // Start is called before the first frame update
    void Start()
    {
        mStartPosition = transform.localPosition;

        mTimer = Random.Range(0f, 50f);
    }

    // Update is called once per frame
    void Update()
    {
        mTimer += Time.deltaTime;
        transform.localPosition = mStartPosition + new Vector3(Mathf.Cos(mTimer), Mathf.Sin(mTimer * 2), 0f) * 0.05f;
    }
}
