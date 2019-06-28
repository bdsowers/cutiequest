using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VectorExtensions
{
    public static class VectorExtensions
    {
        public static Vector2 AsVector2(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.y);
        }

        public static Vector2 AsVector2UsingXZ(this Vector3 vec)
        {
            return new Vector2(vec.x, vec.z);
        }

        public static Vector2Int AsVector2Int(this Vector3 vec)
        {
            return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.y));
        }

        public static Vector2Int AsVector2IntUsingXZ(this Vector3 vec)
        {
            return new Vector2Int(Mathf.RoundToInt(vec.x), Mathf.RoundToInt(vec.z));
        }

        public static Vector3 AsVector3(this Vector2 vec)
        {
            return new Vector3(vec.x, vec.y, 0f);
        }

        public static Vector3 WithZeroY(this Vector3 vec)
        {
            return new Vector3(vec.x, 0f, vec.z);
        }
    }

    public class VectorHelper
    {
        public static Vector3 RandomNormalizedVector3()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            Vector3 vec = new Vector3(x, y, z);
            vec.Normalize();

            if (vec.magnitude > 0.01f)
            {
                return vec;
            }
            else
            {
                return Vector3.right;
            }
        }

        public static Vector3 RandomNormalizedXZVector3()
        {
            float x = Random.Range(-1f, 1f);
            float y = 0f;
            float z = Random.Range(-1f, 1f);
            Vector3 vec = new Vector3(x, y, z);
            vec.Normalize();

            if (vec.magnitude > 0.01f)
            {
                return vec;
            }
            else
            {
                return Vector3.right;
            }
        }

        public static Vector3 RandomNormalizedXYVector3()
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            float z = 0f;
            Vector3 vec = new Vector3(x, y, z);
            vec.Normalize();

            if (vec.magnitude > 0.01f)
            {
                return vec;
            }
            else
            {
                return Vector3.right;
            }
        }

        public static float DistanceXY(Vector3 v1, Vector3 v2)
        {
            v1.z = 0f;
            v2.z = 0f;
            return Vector3.Distance(v1, v2);
        }

        public static float DistanceXZ(Vector3 v1, Vector3 v2)
        {
            v1.y = 0f;
            v2.y = 0f;
            return Vector3.Distance(v1, v2);
        }

        public static Vector3 RandomOrthogonalVectorXZ()
        {
            float x = 0f;
            float z = 0f;

            if (Random.Range(0, 2) == 0)
            {
                x = 0f;
                z = (Random.Range(0, 2) == 0 ? -1 : 1);
            }
            else
            {
                x = (Random.Range(0, 2) == 0 ? -1 : 1);
                z = 0f;
            }

            return new Vector3(x, 0f, z);
        }

        public static float OrthogonalDistance(Vector2 v1, Vector2 v2)
        {
            Vector2 diff = v1 - v2;
            return Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
        }

        public static float OrthogonalDistance(Vector3 v1, Vector3 v2)
        {
            Vector3 diff = v1 - v2;
            return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z);
        }
    }
}
