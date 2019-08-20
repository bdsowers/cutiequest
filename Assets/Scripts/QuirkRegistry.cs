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
        // note bdsowers - in practice, this list is only ever 1 or 2 items long,
        // so this shouldn't really be a performance concern.
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
