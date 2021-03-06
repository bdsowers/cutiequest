﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameObjectExtensions
{
    public static class GameObjectExtensions
    {
        public static void SetLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            for (int i = 0; i < gameObject.transform.childCount; ++i)
            {
                SetLayerRecursive(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        public static T AddComponentIfNecessary<T>(this GameObject gameObject) where T:Component
        {
            T prevComponent = gameObject.GetComponent<T>();
            if (prevComponent != null)
                return prevComponent;

            T newComponent = gameObject.AddComponent<T>();
            return newComponent;
        }

        public static void RemoveAllChildren(this GameObject gameObject)
        {
            for (int i = 0; i < gameObject.transform.childCount; ++i)
            {
                GameObject.Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}