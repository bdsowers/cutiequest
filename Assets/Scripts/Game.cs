using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

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
                CharacterData cd = characterDataList.CharacterWithUID(playerData.followerUid);
                if (cd == null)
                {
                    playerData.suppressDirtyUpdates = true;
                    playerData.followerUid = characterDataList.characterData[0].characterUniqueId;
                    playerData.suppressDirtyUpdates = false;

                    return characterDataList.characterData[0];
                }
                else
                {
                    return cd;
                }
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

    public CharacterData BuildRandomCharacter()
    {
        CharacterData randomCharacter = ScriptableObject.CreateInstance<CharacterData>();

        string gender = Random.Range(0, 2) == 0 ? "MALE" : "FEMALE";

        // todo bdsowers - younger characters should be more common; this shouldn't be evenly distributed
        randomCharacter.age = Random.Range(18, 100);
        randomCharacter.bio = LocalizedText.GetKeysInList("[" + gender + "_BIO]").Sample();
        randomCharacter.characterName = LocalizedText.GetKeysInList("[" + gender + "_NAME]").Sample();
        randomCharacter.levelRequirement = 1;
        randomCharacter.model = PrefabManager.instance.characterPrefabs.Sample().name;
        randomCharacter.tagline = LocalizedText.GetKeysInList("[" + gender + "_TAGLINE]").Sample();
        randomCharacter.characterUniqueId = randomCharacter.characterName + ":::" + randomCharacter.bio + ":::" + randomCharacter.tagline + ":::" + randomCharacter.model;
        randomCharacter.quirk = PrefabManager.instance.quirkPrefabs.Sample().GetComponent<Quirk>();
        randomCharacter.spell = PrefabManager.instance.spellPrefabs.Sample().GetComponent<Spell>();
        randomCharacter.statBoost = (CharacterStatType)Random.Range(1, 6);
        randomCharacter.statBoostAmount = 1 + Random.Range(0, Game.instance.playerData.attractiveness);

        return randomCharacter;
    }
}
