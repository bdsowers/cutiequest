using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LocalizedLabel : MonoBehaviour
{
    public string key;

    // Start is called before the first frame update
    void OnEnable()
    {
        string message = LocalizedText.Get(key);
        Text text = GetComponent<Text>();
        TextMeshProUGUI textMeshLabel = GetComponent<TextMeshProUGUI>();

        message = PigLatinQuirk.ApplyQuirkIfPresent(message);
        
        if (text != null)
        {
            text.text = message;
        }
        
        if (textMeshLabel != null)
        {
            textMeshLabel.SetText(message);
        }
    }
}
