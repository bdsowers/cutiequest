using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : CharacterComponentBase
{
    private bool mFrozen;
    private Vector3 mFreezePosition;

    // Start is called before the first frame update
    void Start()
    {
        string followerId = Game.instance.playerData.followerUid;
        if (string.IsNullOrEmpty(followerId))
            return;

        CharacterData followerData  = Game.instance.followerData;
        commonComponents.characterModel.ChangeModel(followerData);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Freeze()
    {
        mFrozen = true;
        mFreezePosition = transform.position;
    }

    private void LateUpdate()
    {
        if (mFrozen)
        {
            transform.position = mFreezePosition;
        }
    }
}
