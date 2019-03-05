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
    }
}
