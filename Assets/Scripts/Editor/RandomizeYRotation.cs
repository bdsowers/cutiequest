using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RandomizeYRotation : MonoBehaviour
{
    [MenuItem("HSR/Randomize Rotation")]
    public static void RandomizeRotation()
    {
        GameObject[] objects = UnityEditor.Selection.gameObjects;
        foreach(GameObject obj in objects)
        {
            Vector3 rotation = obj.transform.localRotation.eulerAngles;
            rotation.y = Random.Range(0, 360f);
            obj.transform.localRotation = Quaternion.Euler(rotation);
        }
    }
}
