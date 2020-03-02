using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;
using GameObjectExtensions;
using DG.Tweening;
using System.IO;

public class Game : MonoBehaviour
{
    private int mBuildNumber = -1;
    public int BUILD_NUMBER
    {
        get
        {
            if (mBuildNumber == -1)
            {
                try
                {
                    string text = File.ReadAllText(Path.Combine(Application.streamingAssetsPath, "build.txt"));
                    text = text.Trim();
                    mBuildNumber = int.Parse(text);
                }
                catch (System.Exception) { }
            }

            return mBuildNumber;
        }
    }

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
    private QuirkRegistry mQuirkRegistry;
    private ActionGlyphMapper mActionGlyphMapper;

    private BasicActionSet mActionSet;

    // Used only for dev purposes so we can start on the Dungeon scene.
    public DungeonData defaultDungeonData;
    public DungeonData debugDungeonData;

    private const float mPreviewDelay = 2.5f * 60f;
    private float mLastInputTime = -1f;

    private bool mClosingGame = false;
    private int mHubEntriesForRatingDialog = 0;

    private string mActiveScene;
    public string activeScene
    {
        get { return mActiveScene; }
    }

    private FocusFixer mFocusFixer;
    public FocusFixer focusFixer
    {
        get
        {
            if (mFocusFixer == null)
                mFocusFixer = GetComponent<FocusFixer>();

            return mFocusFixer;
        }
    }

    public static Game instance
    {
        get { return mInstance; }
    }

    public ActionGlyphMapper actionGlyphMapper
    {
        get
        {
            if (mActionGlyphMapper == null)
                mActionGlyphMapper = GetComponentInChildren<ActionGlyphMapper>();

            return mActionGlyphMapper;
        }
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
                mHUD = GameObject.FindObjectOfType<HUD>();

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

    public bool isShopKeeperEnemy { get; set; }

    public CentralEvents centralEvents
    {
        get { return mCentralEvents; }
    }

    public QuirkRegistry quirkRegistry
    {
        get { return mQuirkRegistry; }
    }

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

    public bool luckyPennyUsed { get; set; }

    private MinimapCamera mMinimapCamera;
    public MinimapCamera minimapCamera
    {
        get
        {
            if (mMinimapCamera == null)
                mMinimapCamera = GameObject.FindObjectOfType<MinimapCamera>();

            return mMinimapCamera;
        }
    }

    public void EnterDungeon(DungeonData dungeonData, string entranceId)
    {
        dungeonEntranceId = entranceId;

        // Reset hearts & coins unless there's a lucky penny in play, then kill the penny
        if (!Game.instance.luckyPennyUsed)
        {
            Game.instance.playerData.numHearts = 0;
            Game.instance.playerData.numCoins = 0;
        }
        else
        {
            Game.instance.luckyPennyUsed = false;
        }

        mCurrentDungeonData = dungeonData;
        mCurrentDungeonFloor = 1;
        mAttractivenessWhenDungeonEntered = playerData.attractiveness;

        isShopKeeperEnemy = false;
    }

    public bool InDungeon()
    {
        return mActiveScene == "Dungeon";
    }

    public bool InPreview()
    {
        return mActiveScene == "Preview";
    }

    public bool InTitle()
    {
        return mActiveScene == "Title";
    }

    public bool finishedTutorial { get; set; }

    public int whoseTurn { get; set; }

    public string dungeonEntranceId { get; set; }

    private InventoryDisplay mInventoryDisplay;
    public InventoryDisplay inventoryDisplay
    {
        get
        {
            if (mInventoryDisplay == null)
                mInventoryDisplay = GameObject.FindObjectOfType<InventoryDisplay>();

            return mInventoryDisplay;
        }
    }

    private DialogManager mDialogManager;
    public DialogManager dialogManager
    {
        get
        {
            if (mDialogManager == null)
                mDialogManager = GameObject.FindObjectOfType<DialogManager>();

            return mDialogManager;
        }
    }

    private void Awake()
    {
        if (mInstance != null)
        {
            Destroy(gameObject);
            return;
        }

        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;

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
        mQuirkRegistry = GetComponentInChildren<QuirkRegistry>();

        mSaveManager.LoadGame();

        playerData.onPlayerDataChanged += OnPlayerDataChanged;
        playerStats.onCharacterStatisticsChanged += OnPlayerStatsChanged;
    }

    private void OnActiveSceneChanged(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.Scene arg1)
    {
        mActiveScene = arg1.name;
        Debug.Log("Switched to scene " + mActiveScene);
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

#if DEMO
        if (Time.time - mLastInputTime > mPreviewDelay && !transitionManager.isTransitioning &&
            !InPreview())
        {
            transitionManager.TransitionToScreen("Preview");
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        mActionSet.DetectController();

        if (Game.instance.actionSet.Pause.WasPressed && !InTitle())
        {
            // In some control schemes, pause & close are the same button (Escape)
            // Make sure these don't overlap
            if (Game.instance.dialogManager != null && Game.instance.dialogManager.AnyDialogsOpen())
                return;

            DuckpicUI dp = GameObject.FindObjectOfType<DuckpicUI>();
            if (dp != null && dp.gameObject.activeInHierarchy)
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

    public void MakeShopKeeperEnemy()
    {
        GameObject shopKeep = CinematicId.FindObjectWithId("shopkeep");
        if (shopKeep == null)
            return;

        shopKeep.GetComponent<Enemy>().enabled = true;
        shopKeep.GetComponent<Killable>().enabled = true;
        shopKeep.GetComponent<Killable>().invulnerable = false;
        shopKeep.GetComponent<EnemyMelee>().enabled = true;
        shopKeep.GetComponent<SimpleAttack>().enabled = true;
        shopKeep.layer = LayerMask.NameToLayer("Enemy");
        shopKeep.GetComponentInChildren<CharacterModel>().gameObject.layer = LayerMask.NameToLayer("Enemy");

        // Disable all the activation plates that are pointing at the now hostile shopkeeper
        ActivationPlate[] plates = GameObject.FindObjectsOfType<ActivationPlate>();
        for (int i = 0; i < plates.Length; ++i)
        {
            if (plates[i].link == shopKeep)
            {
                plates[i].gameObject.SetActive(false);
            }
        }

        isShopKeeperEnemy = true;
    }

    public void HubEntered()
    {
        mHubEntriesForRatingDialog++;

        CheckRatingDialog();
    }

    public bool CheckRatingDialog(bool closing = false)
    {
        // Disabled forever
        return false;

        mClosingGame = closing;

        if (closing == false && mHubEntriesForRatingDialog < 6)
            return false;

        if (!Game.instance.finishedTutorial)
            return false;

        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return false;

        int lastRatedVersion = PlayerPrefs.GetInt("rate_version", 0);

        if (lastRatedVersion >= BUILD_NUMBER)
            return false;

        PlayerPrefs.SetInt("rate_version", BUILD_NUMBER);

        GameObject dialog = Game.instance.cinematicDirector.objectMap.GetObjectByName("choice_dialog");
        string text = "Could we trouble you to take a short survey to tell us about your experience?";

        DialogButton buttonYes = new DialogButton()
        {
            name = "button_yes",
            text = "Yes",
            icon = null
        };

        DialogButton buttonNo = new DialogButton()
        {
            name = "button_no",
            text = "No",
            icon = null
        };

        dialog.GetComponent<GenericChoiceDialog>().Show(text, new List<DialogButton>() { buttonYes, buttonNo });
        dialog.GetComponent<GenericChoiceDialog>().onDialogButtonPressed += OnRateButtonPressed;
        dialog.transform.localScale = 1.1f * Vector3.one;

        return true;
    }

    private void OnRateButtonPressed(string buttonName)
    {
        GameObject dialog = Game.instance.cinematicDirector.objectMap.GetObjectByName("choice_dialog");
        dialog.GetComponent<GenericChoiceDialog>().onDialogButtonPressed -= OnRateButtonPressed;

        if (buttonName == "button_yes")
        {
            UnityEngine.Application.OpenURL("https://docs.google.com/forms/d/1sPvYirt1wT1DVrOF6z5vXsPps2PXfuJZBlyfHpyEr0A/edit");
        }

        if (mClosingGame)
        {
            CloseGame();
        }
    }

    public void CloseGame()
    {
#if DEMO
        Game.instance.cinematicDirector.EndAllCinematics();

        Time.timeScale = 1f;
        Invoke("DisableScreen", 0.01f);

        Game.instance.transitionManager.TransitionToScreen("Title");
#else
        Application.Quit();
#endif
    }

    public void RefreshInventory()
    {
        inventoryDisplay.Refresh();
    }

    public void RunEnded(bool success)
    {
        // If we have the scout, bump up the scout level
        if (Game.instance.playerStats.IsItemEquipped<Scout>())
        {
            Game.instance.playerData.scoutLevel++;
        }

        // The max attractiveness we can reach is a function of what level & dungeon we're currently in
        int dungeonNum = currentDungeonData.dungeonNum;
        int levelNum = currentDungeonFloor;
        int maxAttractiveness = (dungeonNum - 1) * 10 + levelNum * 2 + 1;

        // On the boss's floor, lock out the 'higher tier' unlocks unless the boss fight was actually
        // successful
        if (levelNum == 5 && !success)
        {
            maxAttractiveness--;
        }

        if (Game.instance.playerData.attractiveness < maxAttractiveness)
        {
            Game.instance.playerData.attractiveness++;
        }

        // If we just killed a boss, play some massive catchup
        if (success && Game.instance.playerData.attractiveness < maxAttractiveness)
        {
            Game.instance.playerData.attractiveness = maxAttractiveness;
        }
    }
}
