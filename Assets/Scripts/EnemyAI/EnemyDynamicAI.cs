using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles an enemy that has multiple AI components.
/// This should come as the first AI component in the list.
/// </summary>
public class EnemyDynamicAI : EnemyAI
{
    private List<EnemyAI> mAIModules = new List<EnemyAI>();
    private EnemyAI mActiveModule;

    private void Start()
    {
        GetComponentsInChildren<EnemyAI>(mAIModules);
    }

    public override bool CanUpdateAI()
    {
        if (mActiveModule != null)
        {
            return mActiveModule.CanUpdateAI();
        }

        return false;
    }

    public override void UpdateAI()
    {
        if (mActiveModule == null)
            return;

        mActiveModule.UpdateAI();
    }

    public void ActivateAI<T>() where T:EnemyAI
    {
        for (int i = 0; i < mAIModules.Count; ++i)
        {
            if (mAIModules[i] is T)
            {
                mActiveModule = mAIModules[i];
            }
        }
    }
}
