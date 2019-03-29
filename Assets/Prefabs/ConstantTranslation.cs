using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantTranslation : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    public bool useFixedUpdate;

    private void Update()
    {
        if (useFixedUpdate)
            return;

        transform.position += direction * speed * Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (!useFixedUpdate)
            return;

        transform.position += direction * speed * Time.fixedDeltaTime;
    }
}
