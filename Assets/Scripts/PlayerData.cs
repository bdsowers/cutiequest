using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public delegate void PlayerDataChanged(PlayerData newData);
    public event PlayerDataChanged onPlayerDataChanged;

    private string mFollowerUid = "1";
    private int mNumHearts = 0;
    private int mNumCoins = 0;
    private int mHealth = 100;
    private int mAttractiveness = 1;

    public bool suppressDirtyUpdates { get; set; }

    public string followerUid
    {
        get { return mFollowerUid; }
        set
        {
            if (mFollowerUid != value)
            {
                mFollowerUid = value;

                MarkDirty();
            }
        }
    }

    public int numHearts
    {
        get { return mNumHearts; }
        set
        {
            if (mNumHearts != value)
            {
                mNumHearts = value;

                MarkDirty();
            }
        }
    }

    public int numCoins
    {
        get { return mNumCoins; }
        set
        {
            if (mNumCoins != value)
            {
                mNumCoins = value;

                MarkDirty();
            }
        }
    }

    public int health
    {
        get { return mHealth; }
        set
        {
            if (mHealth != value)
            {
                mHealth = value;

                MarkDirty();
            }
        }
    }

    public int attractiveness
    {
        get { return mAttractiveness; }
        set
        {
            if (mAttractiveness != value)
            {
                mAttractiveness = value;

                MarkDirty();
            }
        }
    }

    private void MarkDirty()
    {
        if (suppressDirtyUpdates)
            return;

        if (onPlayerDataChanged != null)
        {
            onPlayerDataChanged(this);
        }
    }
}
