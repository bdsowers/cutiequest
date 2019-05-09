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

    private float mSwitchTimer = 2f;

    private void Start()
    {
        mEnemy = GetComponent<Enemy>();
        mSummonerAI = GetComponent<EnemySummoner>();
        mProjectileThrowerAI = GetComponent<EnemyProjectileThrower>();
        mSpellCasterAI = GetComponent<EnemySpellCaster>();
        mAllAIModules = new List<EnemyAI>() { mSummonerAI, mProjectileThrowerAI, mSpellCasterAI };

        mSummonerAI.enabled = true;

        mEnemy.SetEnemyAI(this);
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

    private void Update()
    {
        mSwitchTimer -= Time.deltaTime;
        if (mSwitchTimer < 0f)
        {
            if (CanUpdateAI())
            {
                mSwitchTimer = Random.Range(2f, 4f);

                for (int i = 0; i < mAllAIModules.Count; ++i)
                    mAllAIModules[i].enabled = false;

                EnemyAI newActiveModule = mAllAIModules.Sample();
                newActiveModule.enabled = true;
                newActiveModule.AIStructureChanged();
            }
        }
    }
}
