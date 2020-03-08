using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivateWhenHit : MonoBehaviour
{
    public EnemyMelee enemyMeleeController;
    public EnemyProjectileThrower enemyProjectileController;

    public string activationCinematicEvent;
    private bool mActivationCinematicEventFired;

    public string closeCinematicEvent;
    private bool mCloseCinematicEventFired;

    public bool playSound;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Killable>().onHit += OnHit;
        SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, Vector3.back);
    }

    private void OnHit(Killable entity)
    {
        if (!string.IsNullOrEmpty(activationCinematicEvent) && !mActivationCinematicEventFired)
        {
            if (playSound)
                Game.instance.soundManager.PlaySound("boss_intro");

            mActivationCinematicEventFired = true;
            Game.instance.cinematicDirector.PostCinematicEvent(activationCinematicEvent);

            Vector3 dir = Game.instance.avatar.transform.position - transform.position;
            dir.y = 0f;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                dir.y = 0f;
            else
                dir.x = 0f;
            dir.Normalize();

            SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, dir);
        }

        if (enemyMeleeController != null)
            enemyMeleeController.enabled = true;

        if (enemyProjectileController != null)
            enemyProjectileController.enabled = true;
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, Game.instance.avatar.transform.position);
        if (distance < 4f && !string.IsNullOrEmpty(closeCinematicEvent) && !mCloseCinematicEventFired)
        {
            mCloseCinematicEventFired = true;
            Game.instance.cinematicDirector.PostCinematicEvent(closeCinematicEvent);
        }
    }

    private void OnDestroy()
    {
        GetComponent<Killable>().onHit -= OnHit;
    }
}
