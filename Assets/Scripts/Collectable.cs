using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// todo bdsowers - rewrite to support any collectable
public class Collectable : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<PlayerController>() != null)
        {
            Game.instance.playerData.numHearts++;

            Destroy(gameObject);
        }
    }
}
