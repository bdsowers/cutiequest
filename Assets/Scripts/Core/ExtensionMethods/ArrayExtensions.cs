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

        /// <summary>
        /// Samples from a List, but will not return anything in the ignoreList.
        /// This is done without generating any garbage.
        /// This is also not guaranteed - there's a chance that something in the
        /// ignoreList will be returned if the sampling process spends too long
        /// attempting to choose something not in the ignoreList.
        /// </summary>
        public static T Sample<T>(this List<T> list, List<T> ignoreList)
        {
            int attempts = 500;
            while (attempts > 0)
            {
                T sampledItem = list[Random.Range(0, list.Count)];
                if (!ignoreList.Contains(sampledItem))
                {
                    return sampledItem;
                }

                attempts--;
            }

            return list.Sample();
        }

        public static int SamplePosition<T>(this List<T> list, List<int> ignoredPositions)
        {
            int attempts = 500;
            while (attempts > 0)
            {
                int sampledPosition = Random.Range(0, list.Count);
                if (!ignoredPositions.Contains(sampledPosition))
                {
                    return sampledPosition;
                }

                attempts--;
            }

            return Random.Range(0, list.Count);
        }


        public static T Sample<T>(this T[] array)
        {
            return array[Random.Range(0, array.Length)];
        }

        /// <summary>
        /// Samples from an Array, but will not return anything in the ignoreList.
        /// This is done without generating any garbage.
        /// This is also not guaranteed - there's a chance that something in the
        /// ignoreList will be returned if the sampling process spends too long
        /// attempting to choose something not in the ignoreList.
        /// </summary>
        public static T Sample<T>(this T[] array, List<T> ignoreList)
        {
            int attempts = 500;
            while (attempts > 0)
            {
                T sampledItem = array[Random.Range(0, array.Length)];
                if (!ignoreList.Contains(sampledItem))
                {
                    return sampledItem;
                }

                attempts--;
            }

            return array.Sample();
        }

        public static int SamplePosition<T>(this List<T> list)
        {
            return Random.Range(0, list.Count);
        }

        public static int SamplePosition<T>(this T[] array)
        {
            return Random.Range(0, array.Length);
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

        public static void AddWindowed<T>(this List<T> list, T item, int windowSize)
        {
            list.Add(item);

            while (list.Count > windowSize)
            {
                list.RemoveAt(0);
            }
        }
    }
}
