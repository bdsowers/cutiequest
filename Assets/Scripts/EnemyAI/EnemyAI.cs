using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public abstract void UpdateAI();
    public abstract bool CanUpdateAI();
    public abstract void AIStructureChanged();

    private Enemy mParentEnemy;
    public Enemy parentEnemy
    {
        get
        {
            if (mParentEnemy == null)
            {
                mParentEnemy = GetComponentInParent<Enemy>();
            }

            return mParentEnemy;
        }
    }

    protected Vector3 OrthogonalDirection(Transform source, Transform target, bool useLargeAxis)
    {
        Vector3 direction = target.position - source.position;
        direction.y = 0f;

        if (useLargeAxis)
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                direction.z = 0f;
            }
            else
            {
                direction.x = 0f;
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
            {
                direction.x = 0f;
            }
            else
            {
                direction.z = 0f;
            }
        }

        return direction.normalized;
    }
}
