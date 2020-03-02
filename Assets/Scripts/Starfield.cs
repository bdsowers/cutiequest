using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Starfield : MonoBehaviour
{
    public Image starTemplate;

    // Start is called before the first frame update
    void Start()
    {
        int numStars = 200;
        for (int i = 0; i < numStars; ++i)
        {
            GameObject newStar = GameObject.Instantiate(starTemplate.gameObject, starTemplate.transform.parent);
            float x = Random.Range(-1000f, 1000f);
            float y = Random.Range(-1000f, 1000f);
            newStar.transform.localPosition = new Vector3(x, y, 0);
            newStar.transform.localScale = Vector3.one * Random.Range(0.05f, 0.12f);

            Image newImage = newStar.GetComponent<Image>();
            float intensity = Random.Range(0.5f, 1f);
            newImage.color = new Color(intensity, intensity, intensity, 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
