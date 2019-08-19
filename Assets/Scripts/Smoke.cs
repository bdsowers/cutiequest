using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{
    private bool mInDungeon;

    // Start is called before the first frame update
    void Start()
    {
        mInDungeon = Game.instance.InDungeon();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = Game.instance.avatar.transform.position;
        pos.y = mInDungeon ? 2.5f : 3.5f;
        transform.position = pos;

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
            Camera.main.transform.rotation * Vector3.up);
    }
}
