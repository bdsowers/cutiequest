using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    private string mFollowerUid;
    private int mNumHearts;
    private int mNumCoins;

    public string followerUid
    {
        get { return mFollowerUid; }
        set { mFollowerUid = value; }
    }

    public int numHearts
    {
        get { return mNumHearts; }
        set { mNumHearts = value; }
    }

    public int numCoins
    {
        get { return mNumCoins; }
        set { mNumCoins = value; }
    }
}
