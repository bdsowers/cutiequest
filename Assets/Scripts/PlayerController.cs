using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class PlayerController : MonoBehaviour
{
    public Follower follower;
    public GameObject modelContainer;
    public GameObject highlightLight;

    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;
    private ExternalCharacterStatistics mCharacterStats;
    private Killable mKillable;
    
    public bool isAlive { get; set; }

    private string mFollowerId;

    public Vector3 direction { get; private set; }

    private bool mSpellQueued;
    private bool mDanceQueued;

    private bool mTeleportQueued;
    private Vector2Int mTeleportTarget;

    public GameObject buttonPromptCanvas;

    public bool transitioning { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        CharacterData characterData = ScriptableObject.CreateInstance<CharacterData>();
        characterData.model = Game.instance.playerData.model;
        characterData.material = Game.instance.companionBuilder.MaterialByName(Game.instance.playerData.material);
        GetComponentInChildren<CharacterModel>().ChangeModel(characterData);

        mCharacterStats = GetComponent<ExternalCharacterStatistics>();
        mCharacterStats.externalReference = Game.instance.playerStats;
        follower.GetComponent<ExternalCharacterStatistics>().externalReference = Game.instance.playerStats;

        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mKillable = GetComponent<Killable>();

        mSimpleMovement.onMoveFinished += OnMoveFinished;
        mSimpleAttack.onAttackFinished += OnAttackFinished;

        mKillable.onDeath += OnDeath;

        mFollowerId = Game.instance.playerData.followerUid;
        
        OnFollowerChanged();
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;

        isAlive = true;

        GetComponent<Killable>().health = Game.instance.playerData.health;

        if (DancePartyQuirk.quirkEnabled)
        {
            GetComponentInChildren<Animator>().SetBool("Dancing", true);
            GetComponentInChildren<Animator>().SetInteger("DanceNumber", Random.Range(0, 4));
        }

        Game.instance.whoseTurn = 0;
    }

    private bool HasFollower()
    {
        return Game.instance.followerData != null;
    }

    private void OnPlayerDataChanged(PlayerData newData)
    {
        if (mFollowerId != newData.followerUid)
        {
            mFollowerId = newData.followerUid;
            OnFollowerChanged();
        }
    }

    private void OnDestroy()
    {
        Game.instance.playerData.onPlayerDataChanged -= OnPlayerDataChanged;
    }

    private void OnDeath(Killable entity)
    {
        if (!isAlive)
            return;

        isAlive = false;

        if (Game.instance.finishedTutorial)
        {
            if (Game.instance.playerData.attractiveness < 3)
            {
                // First two runs will unlock new stuff regardless of how poorly the player does
                Game.instance.playerData.attractiveness++;
            }
            else if (Game.instance.currentDungeonFloor > 1)
            {
                Game.instance.playerData.attractiveness++;
            }
        }

        GetComponentInChildren<Animator>().Play("Death");

        Invoke("TransitionAfterDelay", 2.5f);
    }

    void TransitionAfterDelay()
    {
        Game.instance.transitionManager.TransitionToScreen("HUB");
    }

    void OnFollowerChanged()
    {
        // Remove any spells & quirks currently attached
        Spell spell = GetComponentInChildren<Spell>();
        if (spell != null)
            Destroy(spell.gameObject);

        Quirk quirk = GetComponentInChildren<Quirk>();
        if (quirk != null)
            Destroy(quirk.gameObject);

        if (!HasFollower())
            return;

        Follower follower = GameObject.FindObjectOfType<Follower>();
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData);

        AttachFollowerComponents();
        PlaceFollowerInCorrectPosition();
    }

    public void PlaceFollowerInCorrectPosition()
    {
        follower.transform.position = transform.position + new Vector3(-0.25f, 0f, 0.25f);
    }

    private void AttachFollowerComponents()
    {
        Spell oldSpell = GetComponentInChildren<Spell>();
        if (oldSpell != null)
        {
            Destroy(oldSpell.gameObject);
        }

        Quirk oldQuirk = GetComponentInChildren<Quirk>();
        if (oldQuirk != null)
        {
            Destroy(oldQuirk.gameObject);
        }

        if (Game.instance.playerData.followerUid != null)
        {
            CharacterData followerData = Game.instance.characterDataList.CharacterWithUID(Game.instance.playerData.followerUid);
            if (followerData.spell != null)
            {
                GameObject spell = GameObject.Instantiate(followerData.spell.gameObject, transform);
                spell.SetLayerRecursive(LayerMask.NameToLayer("Player"));
            }

            if (followerData.quirk != null)
            {
                GameObject quirk = GameObject.Instantiate(followerData.quirk.gameObject, transform);
                quirk.SetLayerRecursive(LayerMask.NameToLayer("Player"));
            }
        }
    }

    private void OnAttackFinished(GameObject attacker, GameObject target)
    {
        Game.instance.whoseTurn = 1;
    }

    private void OnMoveFinished()
    {
        Game.instance.whoseTurn = 1;
    }

    private void CastSpellIfPossible()
    {
        if (!Game.instance.InDungeon())
            return;

        Spell spell = GetComponentInChildren<Spell>();
        if (spell != null && spell.canActivate)
        {
            spell.Activate(gameObject);
        }

        if (HasFollower())
        {
            spell = follower.GetComponentInChildren<Spell>();
            if (spell != null && spell.canActivate)
            {
                spell.Activate(gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;
        if (Game.instance.transitionManager.isTransitioning)
            return;
        if (!isAlive)
            return;
        if (transitioning)
            return;

        MinimapCamera minimapCamera = GameObject.FindObjectOfType<MinimapCamera>();
        if (minimapCamera.showingWholeMap)
            return;

        if (DialogManager.AnyDialogsOpen())
            return;

        // Queue spells while moving / attack to be executed once those actions end.
        if (Game.instance.actionSet.Spell.WasPressed)
        {
            mSpellQueued = true;
        }

        if (Game.instance.actionSet.Dance.WasPressed && Game.instance.playerData.IsFlagSet("dance_purchased"))
        {
            mDanceQueued = true;
        }

        if (mSimpleMovement.isMoving || mSimpleAttack.isAttacking)
            return;

        SpellCaster caster = GetComponentInChildren<SpellCaster>();
        if (caster != null && caster.isCasting)
            return;

        ProjectileThrower thrower = GetComponentInChildren<ProjectileThrower>();
        if (thrower != null && thrower.isThrowing)
            return;

        if (mSpellQueued)
        {
            mSpellQueued = false;
            CastSpellIfPossible();
            return;
        }

        if (mTeleportQueued)
        {
            mTeleportQueued = false;
            TeleportIfPossible();
            return;
        }

        if (mDanceQueued)
        {
            mDanceQueued = false;
            PlayRandomDance();
            return;
        }

        if (!Game.instance.realTime && Game.instance.whoseTurn != 0)
            return;

        Vector3 followerDirection = transform.position - follower.transform.position;
        followerDirection.y = 0f;
        followerDirection.Normalize();

        Vector3 intendedDirection = Vector3.zero;
        
        float moveThreshold = 0.5f;
        if (Game.instance.actionSet.Move.Y > moveThreshold)
        {
            intendedDirection = new Vector3(0f, 0f, 1f);
        }
        else if (Game.instance.actionSet.Move.Y < -moveThreshold)
        {
            intendedDirection = new Vector3(0f, 0f, -1f);
        }
        else if (Game.instance.actionSet.Move.X < -moveThreshold)
        {
            intendedDirection = new Vector3(-1f, 0f, 0f);
        }
        else if (Game.instance.actionSet.Move.X > moveThreshold)
        {
            intendedDirection = new Vector3(1f, 0f, 0f);
        }

        intendedDirection = SpaceCadetQuirk.ApplyQuirkIfPresent(intendedDirection);

        if (intendedDirection.magnitude > 0.8f)
        {
            direction = intendedDirection;

            if (Game.instance.actionSet.HoldPosition.IsPressed)
            {
                SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, intendedDirection);
            }
            else
            {
                if (mSimpleAttack.CanAttack(intendedDirection))
                {
                    mSimpleAttack.Attack(intendedDirection);
                    AttackFollower(intendedDirection);
                }
                else if (mSimpleMovement.CanMove(intendedDirection))
                {
                    mSimpleMovement.Move(intendedDirection);
                    GetComponentInChildren<Animator>().Play("Idle");
                    MoveFollower(intendedDirection, transform.position + intendedDirection);
                }
                else
                {
                    SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, intendedDirection);

                    if (HasFollower())
                    {
                        SimpleMovement.OrientToDirection(follower.GetComponentInChildren<Animator>().gameObject, intendedDirection);
                    }
                }
            }
        }
    }

    private void PlayRandomDance()
    {
        SimpleMovement.OrientToDirection(mSimpleMovement.subMesh, Vector3.back);
        int dance = Random.Range(1, 5);
        GetComponentInChildren<Animator>().Play("Dance" + dance);
    }

    public void QueueTeleportation(Vector2Int target)
    {
        mTeleportQueued = true;
        mTeleportTarget = target;
    }

    private void TeleportIfPossible()
    {
        CollisionMap collisionMap = Game.instance.levelGenerator.collisionMap;
        List<Vector2Int> viablePositions = collisionMap.EmptyPositionsNearPosition(mTeleportTarget, 1);

        // todo bdsowers - what to do here? error message?
        if (viablePositions.Count == 0)
            return;

        Vector2Int newTarget = viablePositions.Find(i => (i.x == mTeleportTarget.x && i.y == mTeleportTarget.y + 1));

        if (newTarget.x != 0 || newTarget.y != 0)
            mTeleportTarget = newTarget;
        else
            mTeleportTarget = viablePositions[0];

        Vector2Int currentPos = MapCoordinateHelper.WorldToMapCoords(transform.position);

        if (!collisionMap.RemoveMarking(mSimpleMovement.uniqueCollisionIdentity))
        {
            Debug.LogError("CM error in PlayerController");
        }

        collisionMap.MarkSpace(mTeleportTarget.x, mTeleportTarget.y, mSimpleMovement.uniqueCollisionIdentity);

        Game.instance.avatar.transform.position = MapCoordinateHelper.MapToWorldCoords(mTeleportTarget);
        Game.instance.avatar.follower.transform.position = Game.instance.avatar.transform.position + new Vector3(-0.25f, 0f, 0.25f);

        GameObject effect = PrefabManager.instance.InstantiatePrefabByName("CFX2_WWExplosion_C");
        effect.transform.position = Game.instance.avatar.transform.position;
        effect.AddComponent<DestroyAfterTimeElapsed>().time = 2f;
    }

    void MoveFollower(Vector3 direction, Vector3 playerTargetPosition)
    {
        if (!HasFollower())
            return;

        StartCoroutine(MoveFollowerCoroutine(direction, 0.1f, playerTargetPosition));
    }

    IEnumerator MoveFollowerCoroutine(Vector3 direction, float delay, Vector3 playerTargetPosition)
    {
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        follower.GetComponent<SimpleMovement>().Move(direction, playerTargetPosition + new Vector3(-0.25f, 0f, 0.25f));
        yield break;
    }

    void AttackFollower(Vector3 direction)
    {
        if (!HasFollower())
            return;

        StartCoroutine(AttackFollowerCoroutine(direction, 0.1f));
    }

    IEnumerator AttackFollowerCoroutine(Vector3 direction, float delay)
    {
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        follower.GetComponent<SimpleAttack>().Attack(direction);

        yield break;
    }

    public bool IsDancing()
    {
        Animator animator = GetComponentInChildren<Animator>();
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Dance1") || stateInfo.IsName("Dance2") || stateInfo.IsName("Dance3") || stateInfo.IsName("Dance4");
    }
}
