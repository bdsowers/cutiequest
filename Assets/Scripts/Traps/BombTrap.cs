using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrap : MonoBehaviour
{
    private float mDelayTimer = 0.5f;
    private bool mHasDetonated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (mHasDetonated)
            return;

        mDelayTimer -= Time.deltaTime;
        if (mDelayTimer < 0f)
        {
            Detonate();
        }
    }

    private void Detonate()
    {
        mHasDetonated = true;
        GetComponent<SpellCaster>().CastSpell(8);
        Invoke("DestroySelf", 1.5f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
