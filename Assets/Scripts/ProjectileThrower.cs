using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class ProjectileThrower : CharacterComponentBase
{
    public GameObject projectilePrefab;
    public float throwSpeed = 1f;

    public bool isThrowing { get; set; }

    public bool suppressStateUpdates { get; set; }

    public int numThrows = 1;

    public bool IsInRange()
    {
        float range = 5f;
        if (Game.instance.quirkRegistry.IsQuirkActive<ClingyQuirk>())
            range = 2f;

        return Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < range;
    }

    public void ThrowProjectile(int strength, Vector3 direction, Vector3? offset = null, float speedModifier = 1f)
    {
        StartCoroutine(ThrowProjectileCoroutine(strength, direction, offset, speedModifier));
    }

    public IEnumerator ThrowProjectileCoroutine(int strength, Vector3 direction, Vector3? offset = null, float speedModifier = 1f)
    {
        isThrowing = true;

        float actualThrowSpeed = throwSpeed * numThrows;
        if (!Game.instance.realTime)
            actualThrowSpeed = 5f;

        for (int i = 0; i < numThrows; ++i)
        {
            commonComponents.animator.Play("Throw", 0, 0f);
            commonComponents.animator.speed = actualThrowSpeed;

            yield return new WaitForSeconds(0.75f / actualThrowSpeed);

            GameObject projectile = GameObject.Instantiate(projectilePrefab);
            projectile.GetComponent<Projectile>().strength = strength;
            projectile.GetComponent<ConstantTranslation>().direction = direction;
            projectile.GetComponent<ConstantTranslation>().speed *= speedModifier;
            Transform handTransform = commonComponents.animator.GetBoneTransform(HumanBodyBones.RightHand);

            //projectile.transform.position = handTransform.position;
            projectile.transform.position = transform.position + Vector3.up * 0.5f;

            if (offset.HasValue)
            {
                projectile.transform.position += offset.Value;
            }

            projectile.transform.localRotation = commonComponents.animator.transform.localRotation;

            if (gameObject.layer == LayerMask.NameToLayer("Player"))
                projectile.SetLayerRecursive(LayerMask.NameToLayer("PlayerProjectile"));
            else
                projectile.SetLayerRecursive(LayerMask.NameToLayer("EnemyProjectile"));

            // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
            float y = projectile.transform.localRotation.eulerAngles.y;
            y = Mathf.Round(y / 90) * 90.0f;
            projectile.transform.localRotation = Quaternion.Euler(projectile.transform.localRotation.eulerAngles.x, y, projectile.transform.localRotation.eulerAngles.z);

            yield return new WaitForSeconds(2f / actualThrowSpeed);
        }

        if (!suppressStateUpdates)
        {
            commonComponents.animator.speed = 1f;
            isThrowing = false;
        }

        yield break;
    }
}
