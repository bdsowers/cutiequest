using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSelector : MonoBehaviour
{
    public GameObject forestBackground;
    public GameObject gothBackground;
    public GameObject caveBackground;
    public GameObject sciFiBackground;

    // Start is called before the first frame update
    void Start()
    {
        forestBackground.SetActive(false);
        gothBackground.SetActive(false);
        caveBackground.SetActive(false);
        sciFiBackground.SetActive(false);

        if (GothQuirk.quirkEnabled)
        {
            gothBackground.SetActive(true);
        }
        else
        {
            forestBackground.SetActive(true);
        }
    }
}
