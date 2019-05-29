using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArrayExtensions;

public class Boss1 : EnemyAI
{
    // Core actions:
    // * Spawn skeletons
    // * Keep some distance between it & player
    // * Large spell
    // * High-damage projectile when player isn't close
    // * Spell gets bigger & # skeletons become higher as health decreases

    Enemy mEnemy;
    EnemySummoner mSummonerAI;
    EnemyProjectileThrower mProjectileThrowerAI;
    EnemySpellCaster mSpellCasterAI;
    List<EnemyAI> mAllAIModules = new List<EnemyAI>();
    CharacterStatistics mStatistics;
    Killable mKillable;
    EnemyAI mCurrentActiveModule;

    public GameObject[] phaseContainers;

    private float mSwitchTimer = 2f;
    private int mCurrentPhase = 0;

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
        mSummonerAI = GetComponent<EnemySummoner>();
        mProjectileThrowerAI = GetComponent<EnemyProjectileThrower>();
        mSpellCasterAI = GetComponent<EnemySpellCaster>();
        mStatistics = GetComponent<CharacterStatistics>();
        mKillable = GetComponent<Killable>();

        mAllAIModules = new List<EnemyAI>() { mSummonerAI, mProjectileThrowerAI, mSpellCasterAI };

        mSummonerAI.enabled = true;
        mCurrentActiveModule = mSummonerAI;

        mEnemy.SetEnemyAI(this);
        mKillable.onHit += OnHit;

        Game.instance.hud.bossHealth.gameObject.SetActive(true);
        Game.instance.hud.bossHealth.SetWithValues(0, mKillable.health, mKillable.health);
    }

    private void OnHit(Killable entity)
    {
        SwitchPhaseIfNecessary();
    }

    public override bool CanUpdateAI()
    {
        if (mSummonerAI.enabled)
            return mSummonerAI.CanUpdateAI();
        if (mProjectileThrowerAI.enabled)
            return mProjectileThrowerAI.CanUpdateAI();
        if (mSpellCasterAI.enabled)
            return mSpellCasterAI.CanUpdateAI();

        return false;
    }

    public override void UpdateAI()
    {
        if (mSummonerAI.enabled)
            mSummonerAI.UpdateAI();
        if (mProjectileThrowerAI.enabled)
            mProjectileThrowerAI.UpdateAI();
        if (mSpellCasterAI.enabled)
            mSpellCasterAI.UpdateAI();
    }

    public override void AIStructureChanged()
    {
    }

    private void SwitchPhaseIfNecessary()
    {
        int prevPhase = mCurrentPhase;

        if (mKillable.health < mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject) * 0.33f)
            mCurrentPhase = 2;
        else if (mKillable.health < mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject) * 0.66f)
            mCurrentPhase = 1;
        else
            mCurrentPhase = 0;

        if (prevPhase == mCurrentPhase)
            return;

        if (mCurrentPhase == 0)
            Game.instance.hud.bossHealth.fullImage.color = new Color(0, 1, 0);
        else if (mCurrentPhase == 1)
            Game.instance.hud.bossHealth.fullImage.color = new Color(1, 1, 0);
        else
            Game.instance.hud.bossHealth.fullImage.color = new Color(1, 0, 0);

        phaseContainers[0].SetActive(false);
        phaseContainers[1].SetActive(false);
        phaseContainers[2].SetActive(false);
        phaseContainers[mCurrentPhase].SetActive(true);

        SwitchAIBehavior();
    }

    private void OnDestroy()
    {
        Game.instance.hud.bossHealth.gameObject.SetActive(false);
    }

    private void Update()
    {
        Game.instance.hud.bossHealth.SetWithValues(0, mStatistics.ModifiedStatValue(CharacterStatType.MaxHealth, gameObject), mKillable.health);

        mSwitchTimer -= Time.deltaTime;
        if (mSwitchTimer < 0f)
        {
            if (CanUpdateAI())
            {
                mSwitchTimer = Random.Range(2f, 4f);

                SwitchAIBehavior();
            }
        }
    }

    void SwitchAIBehavior()
    {
        for (int i = 0; i < mAllAIModules.Count; ++i)
            mAllAIModules[i].enabled = false;

        List<EnemyAI> ignoreList = new List<EnemyAI>() { mCurrentActiveModule };

        mCurrentActiveModule = mAllAIModules.Sample(ignoreList);
        mCurrentActiveModule.enabled = true;
        mCurrentActiveModule.AIStructureChanged();
    }
}
