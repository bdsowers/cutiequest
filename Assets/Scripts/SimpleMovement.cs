using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    public delegate void MoveFinished();
    public event MoveFinished onMoveFinished;

    public GameObject mesh;
    public List<GameObject> collisionIgnoreList;
    private bool mIsMoving = false;

    private Vector3 mMeshLocalPosition;

    public bool isMoving
    {
        get { return mIsMoving; }
    }


    private void Start()
    {
        mMeshLocalPosition = mesh.transform.localPosition;
    }

    public void Move(Vector3 direction)
    {
        StartCoroutine(MoveCoroutine(direction));
    }

    public bool CanMove(Vector3 direction)
    {
        Ray ray = new Ray(transform.position + new Vector3(0f, 0.2f, 0f), direction);
        RaycastHit[] results = Physics.RaycastAll(ray, 1f);
        for (int i = 0; i < results.Length; ++i)
        {
            if (results[i].collider.gameObject != gameObject && !collisionIgnoreList.Contains(results[i].collider.gameObject))
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator MoveCoroutine(Vector3 direction)
    {
        mIsMoving = true;

        Vector3 position = transform.position;
        Vector3 targetPosition = transform.position + direction;
        float time = 0f;
        while (time < 1f)
        {
            transform.position = Vector3.Lerp(position, targetPosition, time);
            time += Time.deltaTime * 3f;

            float bounceY = (time > 0.5 ? 1f - time : time);
            bounceY *= bounceY;
            mesh.transform.localPosition = mMeshLocalPosition + Vector3.up * bounceY;

            float rotation = (time > 0.5f ? 1 - time : time);
            rotation *= rotation;
            float rotationAngle = 60f;
            mesh.transform.localRotation = Quaternion.Euler(direction.z * rotation * rotationAngle, 0f, -direction.x * rotation * rotationAngle);

            float squashTime = 0.3f;
            float stretchTime = 0.75f;

            float scale = 1f;
            if (time < squashTime)
            {
                scale = 1f - time;
            }
            else if (time < stretchTime)
            {
                scale = (1f - squashTime) + time * 0.65f;
            }
            else
            {
                float maxStretch = (1f - squashTime) + stretchTime * 0.75f;

                scale = maxStretch - (time - stretchTime) * (1 - maxStretch);
            }

            transform.localScale = new Vector3(1f, 1f * scale, 1f);

            yield return null;
        }

        transform.position = targetPosition;
        transform.localScale = Vector3.one;
        mesh.transform.localRotation = Quaternion.identity;
        mesh.transform.localPosition = mMeshLocalPosition;

        mIsMoving = false;

        if (onMoveFinished != null)
        {
            onMoveFinished();
        }

        yield break;
    }
}
