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

    public GameObject subMesh;

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

        SimpleMovement.OrientToDirection(GetComponentInChildren<Animator>().gameObject, direction);

        mIsAttacking = true;
        GameObject target = TargetInDirection(direction);

        Vector3 startPosition = subMesh.transform.position;
        Vector3 hitPosition = startPosition + (target.transform.position - startPosition) * 0.7f;
        bool damageDone = false;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            float d = time / 0.5f;
            if (d > 1f) d = 2f - d;
            subMesh.transform.position = Vector3.Lerp(startPosition, hitPosition, d);

            if (time > 0.6f && !damageDone)
            {
                DealDamage(target);
                damageDone = true;
            }

            yield return null;
        }

        subMesh.transform.position = startPosition;

        if (onAttackFinished != null)
        {
            onAttackFinished(gameObject, target);
        }

        mIsAttacking = false;
        yield break;
    }

    private void DealDamage(GameObject target)
    {
        Killable targetKillable = target.GetComponentInParent<Killable>();
        if (targetKillable != null)
        {
            int strength = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Strength, gameObject);
            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);

            // todo bdsowers - incorporate RNG into attacks at all?
            // todo bdsowers - will need to tweak these equations for sure ... or tweak health values
            // so that both strength and defense can always have an influence.
            int damage = strength * 4 - defense * 2;
            damage = Mathf.Max(damage, 1);

            targetKillable.TakeDamage(damage);
        }
    }
}
