using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    public float speed;
    public Vector3 axis = new Vector3(0f, 1f, 0f);

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(axis, speed * Time.deltaTime);
    }
}
