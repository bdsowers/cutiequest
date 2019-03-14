using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    private bool mIsTransitioning;

    private void OnTriggerEnter(Collider other)
    {
        if (mIsTransitioning)
            return;

        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsTransitioning = true;
            Invoke("Transition", 0.5f);
        }
    }

    private void Transition()
    {
        
        // todo bdsowers - indicate movement to the next floor in some fashion; likely on the Game object
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
