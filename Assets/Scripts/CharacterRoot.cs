using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script that lives on the top of all characters in the game and exposes commonly-used
/// components so they can be cached once.
/// </summary>
public class CharacterRoot : MonoBehaviour
{
    private CharacterModel mCharacterModel;

    public CharacterModel characterModel
    {
        get
        {
            CacheComponentsIfNecessary();

            return mCharacterModel;
        }
    }
    
    private void CacheComponentsIfNecessary()
    {
        if (mCharacterModel != null)
            return;

        mCharacterModel = GetComponentInChildren<CharacterModel>();
    }
}
