using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class SpaceCadetQuirk : Quirk
{
    private static bool mEnabled;

    private static Vector3? mRandomDirection;

    private void Start()
    {
        mEnabled = true;
    }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDestroy()
    {
        mEnabled = false;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    public static Vector3 ApplyQuirkIfPresent(Vector3 intendedDirection)
    {
        if (mEnabled)
        {
            if (intendedDirection.magnitude > 0.1f)
            {
                if (mRandomDirection.HasValue)
                {
                    return mRandomDirection.Value;
                }
                else
                {
                    if (Random.Range(0, 100) < 2)
                    {
                        mRandomDirection = VectorHelper.RandomOrthogonalVectorXZ();
                        Debug.Log(mRandomDirection);
                    }
                }
            }
            else
            {
                mRandomDirection = null;
            }
        }

        return intendedDirection;
    }
}
