using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    private Vector3 mIntendedDirection;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 intendedDirection = new Vector3(Game.instance.actionSet.Move.X, 0f, Game.instance.actionSet.Move.Y);
        intendedDirection.Normalize();
        mIntendedDirection = intendedDirection;

        bool moving = (mIntendedDirection.magnitude > 0.1f);
        GetComponentInChildren<Animator>().SetBool("Moving", moving);
        if (moving)
        {
            GetComponentInChildren<Animator>().Play("Jog Forward");
        }
    }

    private void FixedUpdate()
    {
        GetComponent<CharacterController>().Move(mIntendedDirection * Time.fixedDeltaTime * 3f);
        //transform.position += mIntendedDirection * Time.fixedDeltaTime * 3f;

        if (mIntendedDirection.magnitude > 0.1f)
            SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, mIntendedDirection);
    }
}
