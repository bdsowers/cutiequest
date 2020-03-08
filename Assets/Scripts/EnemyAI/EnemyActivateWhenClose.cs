using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivateWhenClose : MonoBehaviour
{
    public float activationRange;
    public EnemyAI enemyAIController;
    public string activationCinematicEvent;
    public Killable killable;

    private bool mActivated;

    private void Update()
    {
        if (mActivated)
            return;

        float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
        if (distance <= activationRange)
        {
            Activate();
        }
    }

    private void Activate()
    {
        mActivated = true;

        enemyAIController.enabled = true;

        Game.instance.soundManager.PlaySound("boss_intro");

        if (killable != null)
        {
            killable.invulnerable = false;
        }

        if (!string.IsNullOrEmpty(activationCinematicEvent))
        {
            Game.instance.cinematicDirector.PostCinematicEvent(activationCinematicEvent);
        }
    }
}
