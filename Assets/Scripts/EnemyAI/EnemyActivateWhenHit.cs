using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActivateWhenHit : MonoBehaviour
{
    public EnemyMelee enemyAIController;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Killable>().onHit += OnHit;
        SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, Vector3.back);
    }

    private void OnHit(Killable entity)
    {
        enemyAIController.enabled = true;
    }

    private void OnDestroy()
    {
        GetComponent<Killable>().onHit -= OnHit;
    }
}
