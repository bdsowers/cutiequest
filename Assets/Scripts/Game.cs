using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    static Game mInstance = null;

    GameObject mAvatar;

    public static Game instance
    {
        get { return mInstance; }
    }

    public GameObject avatar
    {
        get
        {
            if (mAvatar == null)
            {
                mAvatar = GameObject.Find("Avatar");
            }

            return mAvatar;
        }
    }

    // note bdsowers - eventually turn-based support will likely be deprecated
    public bool realTime
    {
        get { return true; }
    }

    private void Awake()
    {
        mInstance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
