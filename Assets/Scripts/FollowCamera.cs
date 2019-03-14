using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public GameObject target;
    public Vector3 offset;
    public bool useCinematicOffset;
    public bool trackY = true;

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

        transform.position = trackPosition + offset + cinematicOffset;
        transform.LookAt(trackPosition);
    }
}
