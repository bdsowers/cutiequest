using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using ArrayExtensions;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    private AudioSource[] mAudioSources;
    private List<string> mAudioSourceNames = new List<string>();
    private Dictionary<string, AudioSource> mAudioSourceMap = new Dictionary<string, AudioSource>();

    private AudioSource mCurrentMusic;

    private Dictionary<string, float> mMaxFrequencies = new Dictionary<string, float>();
    private Dictionary<string, float> mFrequencyTrackers = new Dictionary<string, float>();

    private float mMusicVolume;
    private float mSFXVolume;

    public float MusicVolume
    {
        get { return mMusicVolume; }
        set
        {
            mMusicVolume = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetFloat("music", mMusicVolume);
        }
    }

    public float SFXVolume
    {
        get { return mSFXVolume; }
        set
        {
            mSFXVolume = Mathf.Clamp(value, 0, 1);
            PlayerPrefs.SetFloat("sfx", mSFXVolume);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        mMusicVolume = PlayerPrefs.GetFloat("music", 1f);
        mSFXVolume = PlayerPrefs.GetFloat("sfx", 1f);

        mAudioSources = GetComponentsInChildren<AudioSource>();

        for (int i = 0; i < mAudioSources.Length; ++i)
        {
            mAudioSourceNames.Add(mAudioSources[i].name);
            mAudioSourceMap.Add(mAudioSources[i].name, mAudioSources[i]);
        }

        mMaxFrequencies.Add("boing", 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < mAudioSourceNames.Count; ++i)
        {
            if (mFrequencyTrackers.ContainsKey(mAudioSourceNames[i]))
            {
                mFrequencyTrackers[mAudioSourceNames[i]] -= Time.deltaTime;
            }
        }
    }

    public void PlaySound(string soundName)
    {
        float maxFrequency = 0f;
        if (mMaxFrequencies.TryGetValue(soundName, out maxFrequency))
        {
            float frequencyTracker = 0f;
            mFrequencyTrackers.TryGetValue(soundName, out frequencyTracker);
            if (frequencyTracker <= 0.001f)
            {
                // Play the sound, set the frequency
                mFrequencyTrackers[soundName] = maxFrequency;
            }
            else
            {
                return;
            }
        }

        mAudioSourceMap[soundName].pitch = 1f + Random.Range(-0.1f, 0.1f);
        mAudioSourceMap[soundName].volume = SFXVolume;
        mAudioSourceMap[soundName].Play();
    }

    public void PlayRandomMusicInCategory(string categoryName)
    {
        // todo bdsowers - some ugly garbage generation herein
        Transform child = transform.Find(categoryName);
        AudioSource[] sources = child.GetComponentsInChildren<AudioSource>();
        PlayMusic(sources.Sample().name);
    }

    public void PlayMusic(string musicName)
    {
        if (mCurrentMusic != null)
        {
            AudioSource prevMusic = mCurrentMusic;
            AudioSource nextMusic = mAudioSourceMap[musicName];
            StartCoroutine(ChangeMusicCoroutine(mCurrentMusic, mAudioSourceMap[musicName]));

            mCurrentMusic = nextMusic;
        }
        else
        {
            mCurrentMusic = mAudioSourceMap[musicName];
            mCurrentMusic.volume = MusicVolume;
            mCurrentMusic.Play();
        }
    }

    private bool coroutineInProgress = false;
    private IEnumerator ChangeMusicCoroutine(AudioSource prevMusic, AudioSource nextMusic)
    {
        if (coroutineInProgress)
            Debug.LogError("Trying to play music while switching music, which may be an issue");

        coroutineInProgress = true;
        nextMusic.Play();

        prevMusic.volume = 1f * MusicVolume;
        nextMusic.volume = 0f;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime;

            prevMusic.volume = (1f - t) * MusicVolume;
            nextMusic.volume = t * MusicVolume;
            yield return null;
        }

        prevMusic.volume = 0f;
        prevMusic.Stop();

        nextMusic.volume = MusicVolume;

        coroutineInProgress = false;
        yield break;
    }

    public void StopMusic()
    {
        if (mCurrentMusic != null)
        {
            mCurrentMusic.Stop();
            mCurrentMusic = null;
        }
    }
}
