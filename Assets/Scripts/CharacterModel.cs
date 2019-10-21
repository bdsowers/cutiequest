using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameObjectExtensions;

public class CharacterModel : CharacterComponentBase
{
    public string modelName;
    public RuntimeAnimatorController animatorController;

    public float scale = 0.5f;
    public float rotation = 180f;

    private void Awake()
    {
        if (!string.IsNullOrEmpty(modelName))
        {
            ChangeModel(modelName);
        }
    }

    public void RemoveModel()
    {
        transform.gameObject.RemoveAllChildren();

        if (commonComponents.simpleMovement != null)
        {
            commonComponents.simpleMovement.subMesh = null;
        }

        if (commonComponents.simpleAttack != null)
        {
            commonComponents.simpleAttack.subMesh = null;
        }
    }

    public void ChangeModel(CharacterData characterData, bool castShadows = true)
    {
        if (characterData == null)
            return;

        ChangeModel(characterData.model, characterData.material, castShadows);
    }

    public void ChangeModel(string newModelName, Material material = null, bool castShadows = true)
    {
        if (string.IsNullOrEmpty(newModelName))
            return;

        transform.gameObject.RemoveAllChildren();

        modelName = newModelName;

        GameObject model = GameObject.Instantiate(PrefabManager.instance.PrefabByName(modelName));
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localScale = Vector3.one * scale;
        model.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);
        model.SetLayerRecursive(gameObject.layer);

        if (material != null)
        {
            model.GetComponentInChildren<Renderer>().material = material;
        }

        if (commonComponents.simpleMovement != null)
        {
            commonComponents.simpleMovement.subMesh = model;
        }

        if (commonComponents.simpleAttack != null)
        {
            commonComponents.simpleAttack.subMesh = model;
        }

        model.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        model.GetComponent<Animator>().applyRootMotion = false;

        if (!castShadows)
            model.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
