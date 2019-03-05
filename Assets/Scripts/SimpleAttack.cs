using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : MonoBehaviour
{
    public delegate void AttackFinished(GameObject attacker, GameObject target);
    public event AttackFinished onAttackFinished;

    private int mPlayerLayer;
    private int mEnemyLayer;
    private bool mIsAttacking;

    public bool isAttacking {  get { return mIsAttacking; } }

    private void Start()
    {
        mPlayerLayer = LayerMask.NameToLayer("Player");
        mEnemyLayer = LayerMask.NameToLayer("Enemy");
    }

    private GameObject TargetInDirection(Vector3 direction)
    {
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.2f, 0f), direction);
        RaycastHit[] results = Physics.RaycastAll(ray, 1f);
        for (int i = 0; i < results.Length; ++i)
        {
            if ((gameObject.layer == mPlayerLayer && results[i].collider.gameObject.layer == mEnemyLayer) ||
                (gameObject.layer == mEnemyLayer && results[i].collider.gameObject.layer == mPlayerLayer))
            {
                return results[i].collider.gameObject;
            }
        }

        return null;
    }

    public bool CanAttack(Vector3 direction)
    {
        return TargetInDirection(direction) != null;
    }

    public void Attack(Vector3 direction)
    {
        if (mIsAttacking)
            return;

        StartCoroutine(AttackCoroutine(direction));
    }

    private IEnumerator AttackCoroutine(Vector3 direction)
    {
        if (mIsAttacking)
            yield break;

        mIsAttacking = true;
        GameObject target = TargetInDirection(direction);

        Vector3 startPosition = transform.position;
        Vector3 hitPosition = startPosition + (target.transform.position - startPosition) * 0.7f;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            float d = time / 0.5f;
            if (d > 1f) d = 2f - d;
            transform.position = Vector3.Lerp(startPosition, hitPosition, d);

            yield return null;
        }

        transform.position = startPosition;
        
        // Deal damage
        Killable targetKillable = target.GetComponentInParent<Killable>();
        if (targetKillable != null)
        {
            targetKillable.TakeDamage(1);
        }

        if (onAttackFinished != null)
        {
            onAttackFinished(gameObject, target);
        }

        mIsAttacking = false;
        yield break;
    }
}
