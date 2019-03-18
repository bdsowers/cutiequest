using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationPlate : MonoBehaviour
{
    // todo bdsowers - better just to load all the cinematics at game start and fire the appropriate events.
    public string cinematic { get; set; }
    public string cinematicEvent { get; set; }

    private bool mIsPlayerInside;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (mIsPlayerInside)
            {
                Game.instance.cinematicDirector.LoadCinematicFromResource(cinematic);
                Game.instance.cinematicDirector.PostCinematicEvent(cinematicEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // todo bdsowers - I've never thought this method of detecting if we're inside a trigger was safe...
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsPlayerInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            mIsPlayerInside = false;
        }
    }
}
