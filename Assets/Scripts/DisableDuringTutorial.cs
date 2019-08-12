using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableDuringTutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!Game.instance.finishedTutorial)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
