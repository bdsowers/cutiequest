using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DirectionallyChallengedQuirk : Quirk
{
    private GameObject mMinimapRawImage;
    
    private float mTimer;
    private int mCurrentRotation;

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        mMinimapRawImage = CinematicId.FindObjectWithId("minimap_image");
        mTimer = Random.Range(10f, 15f);
    }

    // Update is called once per frame
    void Update()
    {
        if (mMinimapRawImage == null)
            return;

        // todo bdsowers - performance yuck
        GameObject fullMap = CinematicId.FindObjectWithId("full_map_image");
        if (fullMap != null)
            fullMap.transform.rotation = mMinimapRawImage.transform.rotation;

        mTimer -= Time.deltaTime;
        if (mTimer < 0f)
        {
            mTimer = Random.Range(30f, 45f);
            mTimer = 5f;
            int rotation = Random.Range(0, 4);
            while (rotation == mCurrentRotation)
                rotation = Random.Range(0, 4);

            mMinimapRawImage.transform.DOLocalRotate(new Vector3(0f, 0f, rotation * 90f), 0.75f);
        }
    }
}
