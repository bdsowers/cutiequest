﻿using System.Collections;
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
    private BasicActionSet mActionSet;

    public bool isAlive { get; set; }

    // todo bdsowers - this probably shouldn't be part of the PlayerController so the same
    // controls can be universal to the game itself.
    public BasicActionSet actionSet {  get { return mActionSet; } }

    private string mFollowerId;

    public Vector3 direction { get; private set; }

    private bool mSpellQueued;

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

        mActionSet = new BasicActionSet();

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

        if (Game.instance.currentDungeonFloor > 1 && Game.instance.finishedTutorial)
            Game.instance.playerData.attractiveness++;

        GetComponentInChildren<Animator>().Play("Death");

        Invoke("TransitionAfterDelay", 2.5f);
    }

    void TransitionAfterDelay()
    {
        Game.instance.transitionManager.TransitionToScreen("HUB");
    }

    void OnFollowerChanged()
    {
        if (!HasFollower())
            return;

        Follower follower = GameObject.FindObjectOfType<Follower>();
        follower.GetComponentInChildren<CharacterModel>().ChangeModel(Game.instance.followerData);

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
        mActionSet.DetectController();

        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return;
        if (!isAlive)
            return;

        // Queue spells while moving / attack to be executed once those actions end.
        if (mActionSet.Spell.WasPressed)
        {
            mSpellQueued = true;
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

        if (!Game.instance.realTime && Game.instance.whoseTurn != 0)
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

        intendedDirection = SpaceCadetQuirk.ApplyQuirkIfPresent(intendedDirection);

        if (intendedDirection.magnitude > 0.8f)
        {
            direction = intendedDirection;

            if (mActionSet.HoldPosition.IsPressed)
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
}
