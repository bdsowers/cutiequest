using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisableForPromo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if PROMO_BUILD
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach(Renderer renderer in renderers)
        {
            renderer.enabled = false;
        }

        Image[] images = GetComponentsInChildren<Image>(true);
        foreach(Image image in images)
        {
            image.enabled = false;
        }

        Text[] text = GetComponentsInChildren<Text>(true);
        foreach(Text label in text)
        {
            label.enabled = false;
        }

        RawImage[] rawImages = GetComponentsInChildren<RawImage>(true);
        foreach(RawImage image in rawImages)
        {
            image.enabled = false;
        }
#endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
