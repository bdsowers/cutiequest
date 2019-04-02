using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class ProjectileThrower : MonoBehaviour
{
    public GameObject projectilePrefab;

    public bool isThrowing { get; private set; }

    public bool IsInRange()
    {
        return Vector3.Distance(Game.instance.avatar.transform.position, transform.position) < 5f;
    }

    public void ThrowProjectile(int strength)
    {
        StartCoroutine(ThrowProjectileCoroutine(strength));
    }

    public IEnumerator ThrowProjectileCoroutine(int strength)
    {
        isThrowing = true;
        Vector3 avatarDirection = (Game.instance.avatar.transform.position - transform.position);
        avatarDirection.y = 0f;
        avatarDirection.x = Mathf.Round(avatarDirection.x);
        avatarDirection.z = Mathf.Round(avatarDirection.z);
        if (Mathf.Abs(avatarDirection.x) > Mathf.Abs(avatarDirection.z))
            avatarDirection.z = 0f;
        else
            avatarDirection.x = 0f;

        avatarDirection.Normalize();

        SimpleMovement.OrientToDirection(GetComponent<SimpleMovement>().subMesh, avatarDirection);
        GetComponentInChildren<Animator>().Play("Throw", 0, 0f);
        yield return new WaitForSeconds(0.75f);

        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.GetComponent<Projectile>().strength = strength;
        projectile.GetComponent<ConstantTranslation>().direction = avatarDirection;
        Transform handTransform = GetComponentInChildren<Animator>().GetBoneTransform(HumanBodyBones.RightHand);
        projectile.transform.position = handTransform.position;

        projectile.transform.localRotation = GetComponentInChildren<Animator>().transform.localRotation;
        projectile.SetLayerRecursive(gameObject.layer);

        // Round the Y rotation to the nearest 90 degree interval; root motion makes the rotation a little imprecise.
        float y = projectile.transform.localRotation.eulerAngles.y;
        y = Mathf.Round(y / 90) * 90.0f;
        projectile.transform.localRotation = Quaternion.Euler(projectile.transform.localRotation.eulerAngles.x, y, projectile.transform.localRotation.eulerAngles.z);

        yield return new WaitForSeconds(2f);

        isThrowing = false;

        yield break;
    }
}
