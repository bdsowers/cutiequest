using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CinematicDirector : MonoBehaviour
{
    private List<CinematicAction> mLoadedCinematics = new List<CinematicAction>();
    private CinematicParser mCinematicParser = new CinematicParser();
    private CinematicDataProvider mDataProvider;
    private CinematicObjectMap mObjectMap = null;

    public CinematicDataProvider dataProvider
    {
        get
        {
            if (mDataProvider == null)
            {
                mDataProvider = GameObject.FindObjectOfType<CinematicDataProvider>();
            }

            return mDataProvider;
        }
    }

    public float playbackTimeScale { get; set; }

    public CinematicObjectMap objectMap
    {
        get
        {
            if (mObjectMap == null)
            {
                mObjectMap = GameObject.FindObjectOfType<CinematicObjectMap>();
            }

            return mObjectMap;
        }
    }

    public void Awake()
    {
        playbackTimeScale = 1f;
    }

    public Coroutine PlayAction(CinematicAction action)
    {
        return StartCoroutine(action.Play(this));
    }

    public Coroutine PlayCinematicAnimation(string id)
    {
        for (int i = 0; i < mLoadedCinematics.Count; ++i)
        {
            if (mLoadedCinematics[i].id == id)
            {
                return PlayAction(mLoadedCinematics[i]);
            }
        }

        return null;
    }

    public void PostCinematicEvent(string eventName)
    {
        for (int i = 0; i < mLoadedCinematics.Count; ++i)
        {
            if (mLoadedCinematics[i].triggerEvent == eventName)
            {
                PlayAction(mLoadedCinematics[i]);
                return;
            }
        }
    }

    public void LoadCinematicFromTextAsset(TextAsset asset)
    {
        LoadCinematicFromString(asset.text);
    }

    public void LoadCinematicFromResource(string resourcePath)
    {
        TextAsset textAsset = Resources.Load(resourcePath) as TextAsset;
        LoadCinematicFromString(textAsset.text);
    }

    public void LoadCinematicFromString(string textBlob)
    {
        mLoadedCinematics.AddRange(mCinematicParser.ParseCinematic(textBlob));
    }

    public CinematicAction CinematicActionById(string id)
    {
        for (int i = 0; i < mLoadedCinematics.Count; ++i)
        {
            if (mLoadedCinematics[i].id == id)
            {
                return mLoadedCinematics[i];
            }
        }

        return null;
    }

    public bool IsCinematicPlaying()
    {
        for (int i = 0; i < mLoadedCinematics.Count; ++i)
        {
            if (mLoadedCinematics[i].isPlaying)
            {
                return true;
            }
        }

        return false;
    }

    public void EndAllCinematics()
    {
        StopAllCoroutines();
    }
}
