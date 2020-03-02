using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finale : MonoBehaviour
{
    public PlayerController playerController;
    public Follower follower;

    // Start is called before the first frame update
    void Start()
    {
        playerController.commonComponents.animator.Play("Falling");
        follower.commonComponents.animator.Play("Falling");

        Game.instance.cinematicDirector.PostCinematicEvent("finale_end");
    }

    // Update is called once per frame
    void Update()
    {
        playerController.commonComponents.animator.Play("Falling");
        follower.commonComponents.animator.Play("Falling");
    }
}
