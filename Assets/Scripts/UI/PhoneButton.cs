using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneButton : MonoBehaviour
{
    public GameObject phoneInterface;

    public void OnPressed()
    {
        phoneInterface.gameObject.SetActive(true);
    }
}
