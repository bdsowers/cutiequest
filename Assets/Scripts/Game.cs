using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    static Game mInstance = null;

    private PlayerController mAvatar;
    private PlayerData mPlayerData = new PlayerData();
    private CharacterStatistics mPlayerStats;
    private SaveManager mSaveManager;
    private ScreenTransitionManager mTransitionManager;
    private CinematicDirector mCinematicDirector;
    private CinematicDataProvider mCinematicDataProvider;
    private CharacterStatInfo mCharacterStatInfo;

    private DungeonData mCurrentDungeonData;
    private int mCurrentDungeonFloor;
    private CentralEvents mCentralEvents;

    // Used only for dev purposes so we can start on the Dungeon scene.
    public DungeonData defaultDungeonData;

    public static Game instance
    {
        get { return mInstance; }
    }

    public PlayerController avatar
    {
        get
        {
            if (mAvatar == null)
            {
                GameObject avatar = GameObject.Find("Avatar");
                if (avatar != null)
                {
                    mAvatar = avatar.GetComponent<PlayerController>();
                }
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

    public ScreenTransitionManager transitionManager
    {
        get
        {
            return mTransitionManager;
        }

    }

    public CinematicDirector cinematicDirector
    {
        get
        {
            return mCinematicDirector;
        }
    }

    public CinematicDataProvider cinematicDataProvider
    {
        get
        {
            return mCinematicDataProvider;
        }
    }

    public CharacterStatInfo characterStatInfo
    {
        get
        {
            return mCharacterStatInfo;
        }
    }

    // note bdsowers - eventually turn-based support will likely be deprecated
    public bool realTime
    {
        get { return true; }
    }

    public DungeonData currentDungeonData
    {
        get { return mCurrentDungeonData; }
    }

    public int currentDungeonFloor
    {
        get { return mCurrentDungeonFloor; }
        set { mCurrentDungeonFloor = value; }
    }

    public CentralEvents centralEvents
    {
        get { return mCentralEvents; }
    }

    public void EnterDungeon(DungeonData dungeonData)
    {
        Game.instance.playerData.numHearts = 0;
        Game.instance.playerData.numCoins = 0;

        mCurrentDungeonData = dungeonData;
        mCurrentDungeonFloor = 1;
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
        mTransitionManager = GetComponentInChildren<ScreenTransitionManager>();
        mCinematicDirector = GetComponentInChildren<CinematicDirector>();
        mCinematicDataProvider = GetComponentInChildren<CinematicDataProvider>();
        mCharacterStatInfo = GetComponentInChildren<CharacterStatInfo>();
        mCentralEvents = new CentralEvents();

        mSaveManager.LoadGame();

        playerData.onPlayerDataChanged += OnPlayerDataChanged;
        playerStats.onCharacterStatisticsChanged += OnPlayerStatsChanged;
    }

    private void OnPlayerStatsChanged(CharacterStatistics stats)
    {
        mSaveManager.TriggerSave();
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        mSaveManager.TriggerSave();
    }

    // Start is called before the first frame update
    void Start()
    {
        mCurrentDungeonData = defaultDungeonData;
        mCurrentDungeonFloor = 1;

        cinematicDirector.LoadCinematicFromResource("Cinematics/characters");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
