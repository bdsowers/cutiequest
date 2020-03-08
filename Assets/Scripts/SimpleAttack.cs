using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAttack : CharacterComponentBase
{
    public delegate void AttackFinished(GameObject attacker, GameObject target);
    public event AttackFinished onAttackFinished;

    private int mPlayerLayer;
    private int mEnemyLayer;
    private int mJunkLayer;
    private bool mIsAttacking;

    public GameObject subMesh;
    public bool noDamage;

    public bool isAttacking {  get { return mIsAttacking; } }

    private Vector3 mSubMeshLocalPosition;

    private void Start()
    {
        mPlayerLayer = LayerMask.NameToLayer("Player");
        mEnemyLayer = LayerMask.NameToLayer("Enemy");
        mJunkLayer = LayerMask.NameToLayer("Junk");

        if (subMesh != null)
        {
            mSubMeshLocalPosition = subMesh.transform.localPosition;
        }
    }

    private GameObject TargetInDirection(Vector3 direction)
    {
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.4f, 0f), direction);
        RaycastHit[] results = Physics.RaycastAll(ray, 1f);
        for (int i = 0; i < results.Length; ++i)
        {
            if (CanAttack(results[i].collider.gameObject))
            {
                return results[i].collider.gameObject;
            }
        }

        return null;
    }

    private bool CanAttack(GameObject target)
    {
        int attackerLayer = gameObject.layer;
        int targetLayer = target.layer;

        bool appropriateLayers = (((attackerLayer == mPlayerLayer && targetLayer == mEnemyLayer) ||
                (attackerLayer == mEnemyLayer && targetLayer == mPlayerLayer) ||
                (targetLayer == mJunkLayer)));

        if (!appropriateLayers)
            return false;

        Killable killable = target.GetComponentInParent<Killable>();

        return (killable != null && !killable.isDead);
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

        SimpleMovement.OrientToDirection(commonComponents.animator.gameObject, direction);

        mIsAttacking = true;
        GameObject target = TargetInDirection(direction);

        if (target == null)
        {
            mIsAttacking = false;
            yield break;
        }

        Vector3 startPosition = subMesh.transform.position;
        Vector3 hitPosition = startPosition + (target.transform.position - startPosition) * 0.7f;

        // Before we actually start playing the attack animation, ensure the target is still there;
        // there can be some lag before we start the attack animation, and the enemy may
        // have teleported out of the way.
        if (Vector3.Distance(transform.position, hitPosition) > 1.5f)
        {
            mIsAttacking = false;
            yield break;
        }

        bool damageDone = false;

        if (gameObject.layer == mPlayerLayer || noDamage)
            Game.instance.soundManager.PlaySound("enemy_hit");

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 3f;
            float d = time / 0.5f;
            if (d > 1f) d = 2f - d;
            subMesh.transform.position = Vector3.Lerp(startPosition, hitPosition, d);

            if (time > 0.6f && !damageDone)
            {
                // Ensure the target is still in an attackable position - some enemies can
                // teleport away very erratically, and killing them post-teleport looks awkward
                // But this shouldn't apply to enemies that are just running - they still get stitches
                if (target != null &&
                    Vector3.Distance(transform.position, hitPosition) < 1.5f &&
                    Vector3.Distance(target.transform.position, hitPosition) < 1.5f)
                {
                    DealDamage(target);

                    damageDone = true;
                }
            }

            yield return null;
        }

        subMesh.transform.localPosition = mSubMeshLocalPosition;

        if (onAttackFinished != null)
        {
            onAttackFinished(gameObject, target);
        }

        mIsAttacking = false;
        yield break;
    }

    private void DealDamage(GameObject target, bool canPierce = true)
    {
        if (noDamage)
            return;
        if (target == null)
            return;

        Killable targetKillable = target.GetComponentInParent<Killable>();
        if (targetKillable != null)
        {
            int strength = GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Strength, gameObject);
            int defense = targetKillable.GetComponent<CharacterStatistics>().ModifiedStatValue(CharacterStatType.Defense, targetKillable.gameObject);

            int damage = strength * 4 - defense * 2;

            if (InstaKill(targetKillable))
                damage = 9999999;

            targetKillable.TakeDamage(gameObject, damage, DamageReason.Melee);

            if (canPierce) AttemptPierce(targetKillable);
        }
    }

    private bool InstaKill(Killable target)
    {
        // If this is the player attacking and the player has the Saw, it may insta-kill
        if (gameObject.layer == mPlayerLayer && Game.instance.playerStats.IsItemEquipped<Saw>())
        {
            Enemy targetEnemy = target.GetComponent<Enemy>();
            if (targetEnemy != null && !targetEnemy.isBoss)
            {
                int val = Random.Range(0, 100);

                if (val < 5) // Don't apply luck - it's supposed to be very rare
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void AttemptPierce(Killable originalTarget)
    {
        // If this is the player attacking and the player has a spear, it may pierce
        if (gameObject.layer == mPlayerLayer && Game.instance.playerStats.IsItemEquipped<Spear>())
        {
            Vector3 dir = originalTarget.transform.position - transform.position;
            dir.y = 0f;
            dir.Normalize();

            Killable nextTarget = KillableMap.instance.KillableAtWorldPosition(originalTarget.transform.position + dir);
            if (nextTarget != null)
            {
                DealDamage(nextTarget.gameObject, false);
            }
        }
    }
}
