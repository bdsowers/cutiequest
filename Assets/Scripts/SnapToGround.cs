using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class SnapToGround : MonoBehaviour
{
    public GameObject[] ignoreList;

    private static RaycastHit[] mCollisionList = new RaycastHit[10];

    private void LateUpdate()
    {
        if (ignoreList == null || ignoreList.Length == 0)
        {
            SimpleSnap();
        }
        else
        {
            ComplexSnap();
        }
    }

    private void SimpleSnap()
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

    private void ComplexSnap()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);

        int layerMask = (1 << LayerMask.NameToLayer("Environment"));

        int numCollisions = Physics.RaycastNonAlloc(ray, mCollisionList, 2f, layerMask);

        for (int i = 0; i < numCollisions; ++i)
        {
            if (System.Array.IndexOf(ignoreList, mCollisionList[i].collider.gameObject) != -1)
                continue;

            if (!mCollisionList[i].collider.isTrigger)
            {
                Vector3 pos = transform.position;
                pos.y = mCollisionList[i].point.y;
                transform.position = pos;
            }
        }
    }
}
