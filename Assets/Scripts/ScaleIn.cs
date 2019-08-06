using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleIn : MonoBehaviour
{
    private Vector3 mTargetScale;

    // Start is called before the first frame update
    void Start()
    {
        mTargetScale = transform.localScale;
        transform.localScale = Vector3.zero;

        transform.DOScale(mTargetScale, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
