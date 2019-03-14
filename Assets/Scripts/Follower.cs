using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follower : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string followerId = Game.instance.playerData.followerUid;
        if (string.IsNullOrEmpty(followerId))
            return;

        string followerModel = Game.instance.characterDataList.CharacterWithUID(followerId).model;
        GetComponentInChildren<CharacterModel>().ChangeModel(followerModel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
