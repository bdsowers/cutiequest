using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            // todo bdsowers - indicate movement to the next floor in some fashion; likely on the Game object

            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
    }
}
