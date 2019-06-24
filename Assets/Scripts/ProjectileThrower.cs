using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float throwSpeed = 1f;

    public bool isThrowing { get; set; }

    public bool suppressStateUpdates { get; set; }

    public bool IsInRange()
    {
        float range = 5f;
        if (ClingyQuirk.quirkEnabled)
            range = 2f;

        return Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < range;
    }

    public void ThrowProjectile(int strength, Vector3 direction, Vector3? offset = null)
    {
        StartCoroutine(ThrowProjectileCoroutine(strength, direction, offset));
    }

    public IEnumerator ThrowProjectileCoroutine(int strength, Vector3 direction, Vector3? offset = null)
    {
        isThrowing = true;

        float actualThrowSpeed = throwSpeed;
        if (!Game.instance.realTime)
            actualThrowSpeed = 5f;

        GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().Play("Throw", 0, 0f);
        GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().speed = actualThrowSpeed;
        
        yield return new WaitForSeconds(0.75f / actualThrowSpeed);

        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.GetComponent<Projectile>().strength = strength;
        projectile.GetComponent<ConstantTranslation>().direction = direction;
        Transform handTransform = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);

        //projectile.transform.position = handTransform.position;
        projectile.transform.position = transform.position + Vector3.up * 0.5f;

        if (offset.HasValue)
        {
            projectile.transform.position += offset.Value;
        }

        projectile.transform.localRotation = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().transform.localRotation;

        //projectile.SetLayerRecursive(gameObject.layer);
        // todo bdsowers - yuck
        if (gameObject.layer == LayerMask.NameToLayer("Player"))
            projectile.SetLayerRecursive(LayerMask.NameToLayer("PlayerProjectile"));
        else
            projectile.SetLayerRecursive(LayerMask.NameToLayer("EnemyProjectile"));

        // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
        float y = projectile.transform.localRotation.eulerAngles.y;
        y = Mathf.Round(y / 90) * 90.0f;
        projectile.transform.localRotation = Quaternion.Euler(projectile.transform.localRotation.eulerAngles.x, y, projectile.transform.localRotation.eulerAngles.z);

        yield return new WaitForSeconds(2f / actualThrowSpeed);

        if (!suppressStateUpdates)
        {
            GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().speed = 1f;
            isThrowing = false;
        }

        yield break;
    }
}
