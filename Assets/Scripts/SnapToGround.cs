using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapToGround : MonoBehaviour
{
    private void LateUpdate()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        RaycastHit hitInfo;
        int layerMask = (1 << LayerMask.NameToLayer("Environment"));
        
        if (Physics.Raycast(ray, out hitInfo, 2f, layerMask))
        {
            if (!hitInfo.collider.isTrigger)
            {
                Vector3 pos = transform.position;
                pos.y = hitInfo.point.y;
                transform.position = pos;
            }
        }
    }
}
