using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel : MonoBehaviour
{
    public string modelName;
    public RuntimeAnimatorController animatorController;

    private void Awake()
    {
        GameObject model = GameObject.Instantiate(PrefabManager.instance.PrefabByName(modelName));
        model.transform.SetParent(transform);
        model.transform.localPosition = Vector3.zero;
        model.transform.localScale = Vector3.one * 0.5f;
        model.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

        GetComponentInParent<SimpleMovement>().subMesh = model;
        model.GetComponent<Animator>().runtimeAnimatorController = animatorController;
    }
}
