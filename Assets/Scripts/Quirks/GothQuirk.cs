using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GothQuirk : Quirk
{
    public override void Start()
    {
        base.Start();

        Game.instance.centralEvents.onSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(string newScene)
    {
        if (newScene == "Dungeon")
        {
            // Turn down them lights
            Light sceneLight = GameObject.FindGameObjectWithTag("MainLight").GetComponent<Light>();
            
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

    public override void OnDestroy()
    {
        base.OnDestroy();

        Game.instance.centralEvents.onSceneChanged -= OnSceneChanged;
    }
}
