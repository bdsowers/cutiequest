using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public delegate void PlayerDataChanged(PlayerData newData);
    public event PlayerDataChanged onPlayerDataChanged;

    private string mModel;
    private string mMaterial;

    private string mFollowerUid = "1";
    private int mNumHearts = 0;
    private int mNumCoins = 0;
    private int mHealth = 100;
    private int mAttractiveness = 1;

    private List<string> mFlags = new List<string>();

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

    public string model
    {
        get { return mModel; }
        set
        {
            if (mModel != value)
            {
                mModel = value;

                MarkDirty();
            }
        }
    }

    public string material
    {
        get { return mMaterial; }
        set
        {
            if (mMaterial != value)
            {
                mMaterial = value;

                MarkDirty();
            }
        }
    }

    public List<string> flags
    {
        get { return mFlags; }
        set
        {
            if (value != mFlags)
            {
                mFlags = value;

                MarkDirty();
            }
        }
    }

    public void SetFlag(string flag)
    {
        if (!mFlags.Contains(flag))
        {
            mFlags.Add(flag);

            MarkDirty();
        }
    }

    public bool IsFlagSet(string flag)
    {
        return mFlags.Contains(flag);
    }

    public void MarkDirty()
    {
        if (suppressDirtyUpdates)
            return;

        if (onPlayerDataChanged != null)
        {
            onPlayerDataChanged(this);
        }
    }
}
