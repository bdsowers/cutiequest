using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float throwSpeed = 1f;

    public bool isThrowing { get; private set; }

    public bool IsInRange()
    {
        return Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < 5f;
    }

    public void ThrowProjectile(int strength, Vector3 direction)
    {
        StartCoroutine(ThrowProjectileCoroutine(strength, direction));
    }

    public IEnumerator ThrowProjectileCoroutine(int strength, Vector3 direction)
    {
        isThrowing = true;

        GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().Play("Throw", 0, 0f);
        GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().speed = throwSpeed;

        yield return new WaitForSeconds(0.75f / throwSpeed);

        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.GetComponent<Projectile>().strength = strength;
        projectile.GetComponent<ConstantTranslation>().direction = direction;
        Transform handTransform = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        projectile.transform.position = handTransform.position;

        projectile.transform.localRotation = GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().transform.localRotation;
        projectile.SetLayerRecursive(gameObject.layer);

        // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
        float y = projectile.transform.localRotation.eulerAngles.y;
        y = Mathf.Round(y / 90) * 90.0f;
        projectile.transform.localRotation = Quaternion.Euler(projectile.transform.localRotation.eulerAngles.x, y, projectile.transform.localRotation.eulerAngles.z);

        yield return new WaitForSeconds(2f / throwSpeed);

        GetComponentInParent<SimpleMovement>().GetComponentInChildren<Animator>().speed = 1f;
        isThrowing = false;

        yield break;
    }
}
