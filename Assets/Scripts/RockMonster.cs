using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RockMonster : MonoBehaviour
{
    public GameObject legs;
    public GameObject eyes;
    public GameObject body;
    public EnemyMelee aiController;

    private bool mActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        aiController.UpdateMeleeAI = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, Game.instance.avatar.transform.position) < 3.5f)
        {
            Activate();
        }
    }

    void Activate()
    {
        if (mActivated)
            return;

        mActivated = true;

        eyes.transform.DOScale(Vector3.one, 0.5f);
        body.transform.DOLocalMove(new Vector3(0, -0.05f, 0), 0.5f);
        aiController.UpdateMeleeAI = true;
    }
}
