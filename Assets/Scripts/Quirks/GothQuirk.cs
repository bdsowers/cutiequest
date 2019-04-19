using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GothQuirk : Quirk
{
    private static bool mEnabled;
    public static bool quirkEnabled {  get { return mEnabled; } }

    private void OnEnable()
    {
        mEnabled = true;
    }

    private void OnDisable()
    {
        mEnabled = false;
    }

    void Start()
    {
        mEnabled = true;
        Game.instance.centralEvents.onSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(string newScene)
    {
        if (newScene == "Dungeon")
        {
            // Turn down them lights
            Light sceneLight = GameObject.FindObjectOfType<Light>();
            
            sceneLight.intensity = 0.1f;
            sceneLight.color = new Color(0.3f, 0.3f, 0.3f);

            Game.instance.avatar.highlightLight.SetActive(true);
            RenderSettings.ambientIntensity = 0f;
        }
    }

    private void LateUpdate()
    {
        Vector3 lightPos = Game.instance.avatar.highlightLight.transform.position;
        lightPos.y = 2f;
        Game.instance.avatar.highlightLight.transform.position = lightPos;
    }

    private void OnDestroy()
    {
        mEnabled = false;
        Game.instance.centralEvents.onSceneChanged -= OnSceneChanged;
    }
}
