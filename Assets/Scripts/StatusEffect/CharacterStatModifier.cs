using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatModifier : MonoBehaviour
{
    [SerializeField]
    private CharacterStatType mStatType;

    [SerializeField]
    private int mRelativeModification = -1;

    [SerializeField]
    private int mAbsoluteModification = -1;
    
    public CharacterStatType statType
    {
        get { return mStatType; }
    }

    public int modification
    {
        get { return (isAbsolute ? mAbsoluteModification : mRelativeModification); }
    }

    public bool isAbsolute { get { return mAbsoluteModification != -1; } }
    public bool isRelative {  get { return mRelativeModification != -1; } }
    
    public void SetRelativeModification(CharacterStatType statType, int modification)
    {
        mStatType = statType;
        mRelativeModification = modification;
        mAbsoluteModification = -1;
    }

    public void SetAbsoluteModification(CharacterStatType statType, int modification)
    {
        mStatType = statType;
        mRelativeModification = -1;
        mAbsoluteModification = modification;
    }
}
