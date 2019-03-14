using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    static Game mInstance = null;

    private GameObject mAvatar;
    private PlayerData mPlayerData = new PlayerData();
    private CharacterStatistics mPlayerStats;
    private SaveManager mSaveManager;

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

    public PlayerData playerData { get { return mPlayerData; } }

    public CharacterStatistics playerStats {  get { return mPlayerStats; } }

    public CharacterData followerData
    {
        get
        {
            if (playerData.followerUid != null)
            {
                return characterDataList.CharacterWithUID(playerData.followerUid);
            }

            return null;
        }
    }

    public CharacterDataList characterDataList
    {
        get
        {
            return GetComponent<CharacterDataList>();
        }
    }

    // note bdsowers - eventually turn-based support will likely be deprecated
    public bool realTime
    {
        get { return true; }
    }

    private void Awake()
    {
        if (mInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        mInstance = this;
        DontDestroyOnLoad(gameObject);

        mPlayerStats = GetComponentInChildren<CharacterStatistics>();
        mSaveManager = GetComponent<SaveManager>();

        mSaveManager.LoadGame();

        playerData.onPlayerDataChanged += OnPlayerDataChanged;
        playerStats.onCharacterStatisticsChanged += OnPlayerStatsChanged;
    }

    private void OnPlayerStatsChanged(CharacterStatistics stats)
    {
        mSaveManager.SaveGame();
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        mSaveManager.SaveGame();
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
