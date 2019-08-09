using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndlessTileScroll : MonoBehaviour
{
    public float direction;
    public float speed;

    float mOffset;
    Image mImage;

    // Start is called before the first frame update
    void Start()
    {
        mImage = GetComponent<Image>();
        mImage.material = new Material(mImage.material);
    }

    // Update is called once per frame
    void Update()
    {
        mOffset += Time.deltaTime * direction * speed;
        mImage.material.SetTextureOffset("_MainTex", new Vector2(1f, -1f) * mOffset);
    }
}
