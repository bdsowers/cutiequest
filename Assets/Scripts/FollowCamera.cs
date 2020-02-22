using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    public bool useCinematicOffset;
    public bool trackY = true;

    public Vector3 animationOffset;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        Vector3 cinematicOffset = Vector3.zero;
        if (useCinematicOffset)
        {
            if (target.transform.position.z >= 0f)
            {
                cinematicOffset.y = -target.transform.position.z * 0.5f;
                if (cinematicOffset.y < -2.5f)
                    cinematicOffset.y = -2.5f;
            }
        }

        Vector3 trackPosition = target.transform.position;
        if (!trackY)
            trackPosition.y = 0f;

        transform.position = trackPosition + offset + cinematicOffset + animationOffset;
        transform.LookAt(trackPosition);
    }

    public void SimpleShake()
    {
        StartCoroutine(SimpleShakeCoroutine());
    }

    IEnumerator SimpleShakeCoroutine()
    {
        float totalSeconds = 0.25f;
        float speed = 5f;
        float intensity = 0.25f;

        float seconds = 0f;
        while (seconds < totalSeconds)
        {
            Vector3 offset = animationOffset;
            offset.x = Mathf.Sin(seconds * speed * 2f * 3.1415f) * intensity;
            offset.y = Mathf.Sin(seconds * speed * 2f * 3.1415f * 0.25f) * intensity;

            animationOffset = offset;
            seconds += Time.deltaTime;
            yield return null;
        }

        while (animationOffset.magnitude > 0.1f)
        {
            animationOffset *= 0.5f;
            yield return null;
        }

        animationOffset = Vector3.zero;
    }
}
