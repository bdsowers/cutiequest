using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private SimpleMovement mSimpleMovement;
    private EnemyAI mEnemyAI;
    private RevealWhenAvatarIsClose mReveal;

    private Killable mKillable;

    public float actionCooldown;
    private float mActionCooldownTimer = -1;
    private bool mActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        Game.instance.enemyDirector.RegisterEnemy(this);

        mEnemyAI = GetComponent<EnemyAI>();
        mSimpleMovement = GetComponent<SimpleMovement>();
        mKillable = GetComponent<Killable>();
        mReveal = GetComponentInChildren<RevealWhenAvatarIsClose>();

        mKillable.onDeath += OnDeath;

        Game.instance.centralEvents.FireEnemyCreated(this);
    }

    private void OnDestroy()
    {
        Game.instance.enemyDirector.UnregisterEnemy(this);    
    }

    public void SetEnemyAI(EnemyAI ai)
    {
        mEnemyAI = ai;
    }

    private void OnDeath(Killable entity)
    {
        DropsItems[] di = GetComponentsInChildren<DropsItems>();
        for (int i = 0; i < di.Length; ++i)
        {
            di[i].Drop();
        }
    }

    private void OnTurnGranted()
    {
        UpdateAI();
    }

    // Update is called once per frame
    public void UpdateEnemy()
    {
        if (!mActivated)
        {
            float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
            if (distance < 7f)
            {
                mActivated = true;
            }
        }

        if (mActivated)
        {
            if (mActionCooldownTimer >= 0f)
                mActionCooldownTimer -= Time.deltaTime;

            if (CanUpdateAI())
            {   
                UpdateAI();
                mActionCooldownTimer = actionCooldown;
            }
        }
    }

    public bool CanUpdateAI()
    {
        if (mEnemyAI == null)
            return false;

        if (!mReveal.fullyRevealed)
            return false;

        if (!mEnemyAI.enabled)
            return false;
        if (Game.instance.realTime && mActionCooldownTimer > 0f)
            return false;
        if (mSimpleMovement.isMoving)
            return false;
        
        if (!Game.instance.avatar.isAlive)
            return false;
        if (Game.instance.cinematicDirector.IsCinematicPlaying())
            return false;
        if (Game.instance.transitionManager.isTransitioning)
            return false;

        return mEnemyAI.CanUpdateAI();
    }

    private void UpdateAI()
    {
        if (!CanUpdateAI())
            return;

        mEnemyAI.UpdateAI();
    }

    // Turn-based support only
    public bool ReadyForTurn()
    {
        return mEnemyAI.CanUpdateAI() && !mSimpleMovement.isMoving;
    }
}
