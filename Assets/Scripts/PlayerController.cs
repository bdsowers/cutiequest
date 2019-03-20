using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Follower follower;

    private TurnBasedMovement mTurnBasedMovement;
    private SimpleMovement mSimpleMovement;
    private SimpleAttack mSimpleAttack;
    private ExternalCharacterStatistics mCharacterStats;
    private Killable mKillable;
    private BasicActionSet mActionSet;

    public bool isAlive { get; set; }

    // todo bdsowers - this probably shouldn't be part of the PlayerController so the same
    // controls can be universal to the game itself.
    public BasicActionSet actionSet {  get { return mActionSet; } }

    private string mFollowerId;

    // Start is called before the first frame update
    void Start()
    {
        mCharacterStats = GetComponent<ExternalCharacterStatistics>();
        mCharacterStats.externalReference = Game.instance.playerStats;
        follower.GetComponent<ExternalCharacterStatistics>().externalReference = Game.instance.playerStats;

        mTurnBasedMovement = GetComponent<TurnBasedMovement>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mSimpleAttack = GetComponent<SimpleAttack>();
        mKillable = GetComponent<Killable>();

        mTurnBasedMovement.ActivateTurnMovement();
        mSimpleMovement.onMoveFinished += OnMoveFinished;
        mSimpleAttack.onAttackFinished += OnAttackFinished;

        mKillable.onDeath += OnDeath;

        mFollowerId = Game.instance.playerData.followerUid;
        Game.instance.playerData.onPlayerDataChanged += OnPlayerDataChanged;

        OnFollowerChanged();

        isAlive = true;

        mActionSet = new BasicActionSet();
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
        // todo bdsowers - need a fancier effect, including coin loss.

        isAlive = false;

        Game.instance.playerData.numCoins = 0;
        Game.instance.playerData.health = mCharacterStats.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject);

        Game.instance.transitionManager.TransitionToScreen("HUB");
    }

    void OnFollowerChanged()
    {
        Follower follower = GameObject.FindObjectOfType<Follower>();
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData.model);

        AttachFollowerComponents();
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
            }

            if (followerData.quirk != null)
            {
                GameObject quirk = GameObject.Instantiate(followerData.quirk.gameObject, transform);
            }
        }
    }

    private void OnAttackFinished(GameObject attacker, GameObject target)
    {
        mTurnBasedMovement.TurnFinished();
    }

    private void OnMoveFinished()
    {
        mTurnBasedMovement.TurnFinished();
    }

    private void CastSpellIfPossible()
    {
        Spell spell = GetComponentInChildren<Spell>();
        if (spell != null && spell.canActivate)
        {
            spell.Activate(gameObject);
        }

        spell = follower.GetComponentInChildren<Spell>();
        if (spell != null && spell.canActivate)
        {
            spell.Activate(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mActionSet.DetectController();

        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;

        // todo bdsowers - these need to be queued up for when the player movement ends
        if (mActionSet.Spell.WasPressed)
        {
            CastSpellIfPossible();
        }

        if (mSimpleMovement.isMoving || mSimpleAttack.isAttacking)
            return;

        // Disable for real-time play
        if (!mTurnBasedMovement.isMyTurn && !Game.instance.realTime)
            return;

        Vector3 followerDirection = transform.position - follower.transform.position;
        followerDirection.y = 0f;
        followerDirection.Normalize();

        Vector3 intendedDirection = Vector3.zero;

        float moveThreshold = 0.5f;
        if (mActionSet.Move.Y > moveThreshold)
        {
            intendedDirection = new Vector3(0f, 0f, 1f);
        }
        else if (mActionSet.Move.Y < -moveThreshold)
        {
            intendedDirection = new Vector3(0f, 0f, -1f);
        }
        else if (mActionSet.Move.X < -moveThreshold)
        {
            intendedDirection = new Vector3(-1f, 0f, 0f);
        }
        else if (mActionSet.Move.X > moveThreshold)
        {
            intendedDirection = new Vector3(1f, 0f, 0f);
        }

        if (intendedDirection.magnitude > 0.8f)
        {
            if (mActionSet.HoldPosition.IsPressed)
            {
                SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, intendedDirection);
            }
            else
            {
                if (mSimpleAttack.CanAttack(intendedDirection))
                {
                    mSimpleAttack.Attack(intendedDirection);
                }
                else if (mSimpleMovement.CanMove(intendedDirection))
                {
                    mSimpleMovement.Move(intendedDirection);
                    MoveFollower(followerDirection);
                }
                else
                {
                    SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, intendedDirection);
                }
            }
        }
    }

    void MoveFollower(Vector3 direction)
    {
        StartCoroutine(MoveFollowerCoroutine(direction, 0.1f));
    }

    IEnumerator MoveFollowerCoroutine(Vector3 direction, float delay)
    {
        while (delay > 0f)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        follower.GetComponent<SimpleMovement>().Move(direction);
        yield break;
    }
}
