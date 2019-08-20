using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuirkRegistry : MonoBehaviour
{
    List<Quirk> mActiveQuirks = new List<Quirk>();

    public void RegisterQuirkActive(Quirk quirk)
    {
        if (!mActiveQuirks.Contains(quirk))
        {
            mActiveQuirks.Add(quirk);
        }
    }

    public void RegisterQuirkInactive(Quirk quirk)
    {
        mActiveQuirks.Remove(quirk);
    }

    public bool IsQuirkActive<T>()
    {
        for (int i = 0; i < mActiveQuirks.Count; ++i)
        {
            if (mActiveQuirks[i] is T)
            {
                return true;
            }
        }

        return false;
    }
}
