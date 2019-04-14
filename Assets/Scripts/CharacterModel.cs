using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
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

    public void ChangeModel(string newModelName, bool castShadows = true)
    {
        if (string.IsNullOrEmpty(newModelName))
            return;

        for (int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);

        modelName = newModelName;

        GameObject model = GameObject.Instantiate(PrefabManager.instance.PrefabByName(modelName));
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localScale = Vector3.one * scale;
        model.transform.localRotation = Quaternion.Euler(0f, rotation, 0f);

        if (GetComponentInParent<SimpleMovement>() != null)
        {
            GetComponentInParent<SimpleMovement>().subMesh = model;
        }

        if (GetComponentInParent<SimpleAttack>() != null)
        {
            GetComponentInParent<SimpleAttack>().subMesh = model;
        }

        model.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        model.GetComponent<Animator>().applyRootMotion = false;

        if (!castShadows)
            model.GetComponentInChildren<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
    }
}
