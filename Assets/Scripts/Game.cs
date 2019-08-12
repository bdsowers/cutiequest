using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using GameObjectExtensions;

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
    private CompanionBuilder mCompanionBuilder;
    private HUD mHUD;

    private DungeonData mCurrentDungeonData;
    private int mCurrentDungeonFloor;
    private int mAttractivenessWhenDungeonEntered;
    private CentralEvents mCentralEvents;
    private EnemyDirector mEnemyDirector;
    private LevelGenerator mLevelGenerator;
    private SoundManager mSoundManager;

    private BasicActionSet mActionSet;

    // Used only for dev purposes so we can start on the Dungeon scene.
    public DungeonData defaultDungeonData;
    public DungeonData debugDungeonData;

    private const float mPreviewDelay = 2.5f * 60f;
    private float mLastInputTime = -1f;

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

    public SoundManager soundManager
    {
        get { return mSoundManager; }
    }

    public PlayerData playerData { get { return mPlayerData; } }

    public CharacterStatistics playerStats {  get { return mPlayerStats; } }

    public CharacterData followerData
    {
        get
        {
            // The tutorial character is encoded using a number, all others
            // have their traits baked into the followerUid.
            if (playerData.followerUid == "1")
            {
                return characterDataList.tutorialCharacter;
            }

            if (playerData.followerUid != null)
            {
                CharacterData cd = characterDataList.CharacterWithUID(playerData.followerUid);
                return cd;
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

    public CompanionBuilder companionBuilder
    {
        get
        {
            return mCompanionBuilder;
        }
    }

    public EnemyDirector enemyDirector
    {
        get
        {
            return mEnemyDirector;
        }
    }

    public HUD hud
    {
        get
        {
            if (mHUD == null)
                mHUD = GameObject.Find("HUD").GetComponent<HUD>();

            return mHUD;
        }
    }

    public SaveManager saveManager
    {
        get { return mSaveManager; }
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

    public int attractivenessWhenDungeonEntered
    {
        get { return mAttractivenessWhenDungeonEntered; }
        set { mAttractivenessWhenDungeonEntered = value; }
    }

    public CentralEvents centralEvents
    {
        get { return mCentralEvents; }
    }

    // todo bdsowers - not super crazy about this
    public LevelGenerator levelGenerator
    {
        get
        {
            if (mLevelGenerator == null)
            {
                mLevelGenerator = GameObject.FindObjectOfType<LevelGenerator>();
            }

            return mLevelGenerator;
        }

    }

    public BasicActionSet actionSet { get { return mActionSet; } }

    public void EnterDungeon(DungeonData dungeonData)
    {
        Game.instance.playerData.numHearts = 0;
        Game.instance.playerData.numCoins = 0;

        mCurrentDungeonData = dungeonData;
        mCurrentDungeonFloor = 1;
        mAttractivenessWhenDungeonEntered = playerData.attractiveness;
    }

    public bool InDungeon()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Dungeon";
    }

    public bool InPreview()
    {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Preview";
    }

    public bool finishedTutorial { get; set; }

    public int whoseTurn { get; set; }

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
        mCompanionBuilder = GetComponentInChildren<CompanionBuilder>();
        mEnemyDirector = GetComponentInChildren<EnemyDirector>();
        mSoundManager = GetComponentInChildren<SoundManager>();

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

        mActionSet = new BasicActionSet();
    }

    void CheckPreviewMode()
    {
        if (mLastInputTime < 0f)
        {
            mLastInputTime = Time.time;
        }

        if (InControl.InputManager.ActiveDevice.AnyButtonIsPressed ||
            InControl.InputManager.ActiveDevice.AnyButtonWasPressed ||
            InControl.InputManager.AnyKeyIsPressed || 
            Input.anyKeyDown ||
            InControl.InputManager.ActiveDevice.LeftStick.HasChanged ||
            InControl.InputManager.ActiveDevice.RightStick.HasChanged ||
            InControl.InputManager.ActiveDevice.DPad.HasChanged)
        {
            mLastInputTime = Time.time;
        }

        if (Time.time - mLastInputTime > mPreviewDelay && !transitionManager.isTransitioning &&
            !InPreview())
        {
            transitionManager.TransitionToScreen("Preview");
        }
    }

    // Update is called once per frame
    void Update()
    {
        mActionSet.DetectController();

        if (Game.instance.actionSet.Pause.WasPressed)
        {
            // In some control schemes, pause & close are the same button (Escape)
            // Make sure these don't overlap
            if (DialogManager.AnyDialogsOpen())
                return;

            GameObject hud = GameObject.Find("HUD");
            if (hud == null)
                return;

            hud.GetComponent<HUD>().pauseDialog.gameObject.SetActive(true);
        }

        CheckPreviewMode();
    }

    public void NewGame()
    {
        mPlayerData = new PlayerData();

        cinematicDataProvider.Reset();

        // todo bdsowers - there's a lot of work here to do...
        playerStats.ChangeBaseStat(CharacterStatType.MaxHealth, 100);
        playerStats.ChangeBaseStat(CharacterStatType.Defense, 2);
        playerStats.ChangeBaseStat(CharacterStatType.Luck, 2);
        playerStats.ChangeBaseStat(CharacterStatType.Magic, 2);
        playerStats.ChangeBaseStat(CharacterStatType.Speed, 2);
        playerStats.ChangeBaseStat(CharacterStatType.Strength, 2);
        
        mPlayerData.followerUid = "1";

        finishedTutorial = false;

        mCurrentDungeonData = defaultDungeonData;
        mCurrentDungeonFloor = 1;
    }

    public void ForcePreviewMode()
    {
        transitionManager.TransitionToScreen("Preview");
    }
}
