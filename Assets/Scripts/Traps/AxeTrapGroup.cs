using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxeTrapGroup : MonoBehaviour
{
    public float delay { get; set; }
    public bool setup { get; set; }
    public Transform hitPoint;

    private bool mDamageWindow;
    private bool mHit;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!setup)
            return;

        delay -= Time.deltaTime;
        if (delay <= 0f)
        {
            delay += 4f;
            StartCoroutine(Animation());
        }

        if (mDamageWindow)
        {
            CheckDamage();
        }
    }

    void CheckDamage()
    {
        Killable killable = KillableMap.instance.KillableAtWorldPosition(hitPoint.position);
        if (!mHit && killable != null && killable.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            killable.TakeDamage(gameObject, 15, DamageReason.Trap);
            mHit = true;
        }
    }

    void SetRotation(float z)
    {
        Vector3 localRotation = transform.localRotation.eulerAngles;
        localRotation.z = z;
        transform.localRotation = Quaternion.Euler(localRotation);
    }

    IEnumerator Animation()
    {
        mHit = false;

        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime * 4f;
            SetRotation(time * 90f);
            if (time > 0.5f)
                mDamageWindow = true;

            yield return null;
        }

        SetRotation(90f);
        yield return new WaitForSeconds(0.5f);
        mDamageWindow = false;

        while (time > 0f)
        {
            time -= Time.deltaTime * 2f;
            SetRotation(time * 90f);
            yield return null;
        }

        SetRotation(0f);

        yield break;
    }
}
