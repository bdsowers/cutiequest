using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class PlayerController : CharacterComponentBase
{
    public Follower follower;
    public GameObject modelContainer;
    public GameObject highlightLight;

    private ExternalCharacterStatistics mCharacterStats;

    public bool isAlive { get; set; }

    private string mFollowerId;

    public Vector3 direction { get; private set; }

    private bool mSpellQueued;
    private bool mDanceQueued;

    private bool mTeleportQueued;
    private Vector2Int mTeleportTarget;

    public GameObject buttonPromptCanvas;

    public bool transitioning { get; set; }

    public CharacterData characterData
    {
        get; private set;
    }

    // Start is called before the first frame update
    void Start()
    {
        characterData = ScriptableObject.CreateInstance<CharacterData>();
        characterData.model = Game.instance.playerData.model;
        characterData.material = Game.instance.companionBuilder.MaterialByName(Game.instance.playerData.material);
        commonComponents.characterModel.ChangeModel(characterData);

        mCharacterStats = GetComponent<ExternalCharacterStatistics>();
        mCharacterStats.externalReference = Game.instance.playerStats;
        follower.GetComponent<ExternalCharacterStatistics>().externalReference = Game.instance.playerStats;

        commonComponents.simpleMovement.onMoveFinished += OnMoveFinished;
        commonComponents.simpleAttack.onAttackFinished += OnAttackFinished;

        commonComponents.killable.onDeath += OnDeath;

        mFollowerId = Game.instance.playerData.followerUid;

        OnFollowerChanged();
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;

        isAlive = true;

        commonComponents.killable.health = Game.instance.playerData.health;

        if (Game.instance.quirkRegistry.IsQuirkActive<DancePartyQuirk>())
        {
            commonComponents.animator.SetBool("Dancing", true);
            commonComponents.animator.SetInteger("DanceNumber", Random.Range(0, 4));
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

        commonComponents.animator.Play("Death");

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
        follower.commonComponents.characterModel.ChangeModel(Game.instance.followerData);

        AttachFollowerComponents();
        PlaceFollowerInCorrectPosition();

        GameObject.FindObjectOfType<InventoryDisplay>().Refresh();
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

        if (commonComponents.simpleMovement.isMoving || commonComponents.simpleAttack.isAttacking)
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
                SimpleMovement.OrientToDirection(commonComponents.animator.gameObject, intendedDirection);
            }
            else
            {
                if (commonComponents.simpleAttack.CanAttack(intendedDirection))
                {
                    commonComponents.simpleAttack.Attack(intendedDirection);
                    AttackFollower(intendedDirection);
                }
                else if (commonComponents.simpleMovement.CanMove(intendedDirection))
                {
                    commonComponents.simpleMovement.Move(intendedDirection);
                    commonComponents.animator.Play("Idle");
                    MoveFollower(intendedDirection, transform.position + intendedDirection);
                }
                else
                {
                    SimpleMovement.OrientToDirection(commonComponents.animator.gameObject, intendedDirection);

                    if (HasFollower())
                    {
                        SimpleMovement.OrientToDirection(follower.commonComponents.animator.gameObject, intendedDirection);
                    }
                }
            }
        }
    }

    private void PlayRandomDance()
    {
        SimpleMovement.OrientToDirection(commonComponents.simpleMovement.subMesh, Vector3.back);
        int dance = Random.Range(1, 5);
        commonComponents.animator.Play("Dance" + dance);
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

        if (viablePositions.Count == 0)
            return;

        Vector2Int newTarget = viablePositions.Find(i => (i.x == mTeleportTarget.x && i.y == mTeleportTarget.y + 1));

        if (newTarget.x != 0 || newTarget.y != 0)
            mTeleportTarget = newTarget;
        else
            mTeleportTarget = viablePositions[0];

        Vector2Int currentPos = MapCoordinateHelper.WorldToMapCoords(transform.position);

        if (!collisionMap.RemoveMarking(commonComponents.simpleMovement.uniqueCollisionIdentity))
        {
            Debug.LogError("CM error in PlayerController");
        }

        collisionMap.MarkSpace(mTeleportTarget.x, mTeleportTarget.y, commonComponents.simpleMovement.uniqueCollisionIdentity);

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

        follower.commonComponents.simpleMovement.Move(direction, playerTargetPosition + new Vector3(-0.25f, 0f, 0.25f));
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

        follower.commonComponents.simpleAttack.Attack(direction);

        yield break;
    }

    public bool IsDancing()
    {
        Animator animator = commonComponents.animator;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName("Dance1") || stateInfo.IsName("Dance2") || stateInfo.IsName("Dance3") || stateInfo.IsName("Dance4");
    }
}
