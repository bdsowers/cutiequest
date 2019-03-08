using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArrayExtensions
{
    public static class ArrayExtensions
    {
        public static T Sample<T>(this List<T> list)
        {
            return list[Random.Range(0, list.Count)];
        }

        public static T Sample<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count * 3; ++i)
            {
                int pos1 = Random.Range(0, list.Count);
                int pos2 = Random.Range(0, list.Count);

                T temp = list[pos1];
                list[pos1] = list[pos2];
                list[pos2] = temp;
            }
        }

        public static void Shuffle<T>(this T[] array)
        {
            for (int i = 0; i < array.Length * 3; ++i)
            {
                int pos1 = Random.Range(0, array.Length);
                int pos2 = Random.Range(0, array.Length);

                T temp = array[pos1];
                array[pos1] = array[pos2];
                array[pos2] = temp;
            }
        }
    }
}
