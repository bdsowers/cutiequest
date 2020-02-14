using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VectorExtensions;

public class MinimapCamera : MonoBehaviour
{
    public GameObject target;

    public GameObject miniMap;
    public GameObject fullMap;
    public GameObject toggleButtonDisplay;

    private float mOriginalOrthoSize;
    private Camera mCamera;

    private bool mShowingWholeMap = false;

    public bool showingWholeMap {  get { return mShowingWholeMap; } }

    public GameObject questR;

    // Start is called before the first frame update
    void Start()
    {
        mCamera = GetComponent<Camera>();
        mOriginalOrthoSize = mCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        if (fullMap == null)
            return;

        if (Game.instance.actionSet.ToggleMap.WasPressed && !questR.gameObject.activeSelf)
        {
            questR.gameObject.SetActive(true);
            questR.GetComponent<QuestR>().SetTab(2);
        }

        ToggleFullMap(questR.gameObject.activeSelf);
    }

    private void ToggleFullMap(bool newShowingWholeMap)
    {
        if (mShowingWholeMap == newShowingWholeMap)
            return;

        mShowingWholeMap = newShowingWholeMap;

        if (mShowingWholeMap)
        {
            LevelGenerator generator = GameObject.FindObjectOfType<LevelGenerator>();
            float midX = generator.dungeon.width / 2;
            float midY = generator.dungeon.height / 2;
            mCamera.transform.position = new Vector3(midX, mCamera.transform.position.y, -midY);

            mCamera.orthographicSize = Mathf.Max(generator.dungeon.width, generator.dungeon.height) / 2f + 1f;
        }
        else
        {
            mCamera.orthographicSize = mOriginalOrthoSize;
        }

        miniMap.SetActive(!mShowingWholeMap);

        toggleButtonDisplay.SetActive(!mShowingWholeMap);
    }

    private void LateUpdate()
    {
        if (mShowingWholeMap)
            return;

        Vector3 pos = target.transform.position;
        pos.y = transform.position.y;
        transform.position = pos;
    }
}
