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
        ChangeModel(modelName);
    }

    public void ChangeModel(string newModelName)
    {
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);

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

        model.GetComponent<Animator>().runtimeAnimatorController = animatorController;
    }
}
