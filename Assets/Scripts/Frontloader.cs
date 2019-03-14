using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frontloader : MonoBehaviour
{
    public void Start()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("HUB");
    }
}
