using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    Killable mKillable;
    CollisionMap mCollisionMap;

    float mReviveTime = 120f;
    int mNumRevives = 0;

    // Start is called before the first frame update
    void Start()
    {
        mKillable = GetComponent<Killable>();
        mCollisionMap = GameObject.FindObjectOfType<CollisionMap>();

        mReviveTime = Random.Range(10f, 25f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!mKillable.isDead)
            return;

        if (Input.GetKeyDown(KeyCode.J))
            mReviveTime = 0f;

        mReviveTime -= Time.deltaTime;
        if (mReviveTime < 0f)
        {
            // See if we can get up - if something is on top of us, we can't
            Vector2Int mapPos = MapCoordinateHelper.WorldToMapCoords(transform.position);
            if (mCollisionMap.SpaceMarking(mapPos.x, mapPos.y) == 0)
            {
                ++mNumRevives;

                mReviveTime = Random.Range(60, 120) * mNumRevives;

                Revive();
            }
        }
    }

    private void Revive()
    {
        mKillable.Revive();
    }
}
