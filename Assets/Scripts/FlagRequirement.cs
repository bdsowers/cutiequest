using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagRequirement : MonoBehaviour
{
    public enum RequirementEvaluation
    {
        Awake,
        Start,
        Update,
    }

    public GameObject[] objects;
    public GameObject[] disabledObjects;

    public string requiredFlag;
    public RequirementEvaluation whenEvaluated = RequirementEvaluation.Start;

    private void Awake()
    {
        if (whenEvaluated == RequirementEvaluation.Awake)
        {
            EvaluateFlag();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (whenEvaluated == RequirementEvaluation.Start)
        {
            EvaluateFlag();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (whenEvaluated == RequirementEvaluation.Update)
        {
            EvaluateFlag();
        }
    }

    private void EvaluateFlag()
    {
        bool shouldBeActive = Game.instance.playerData.IsFlagSet(requiredFlag);

        for (int i = 0; i < objects.Length; ++i)
        {
            objects[i].SetActive(shouldBeActive);
        }

        for (int i = 0; i < disabledObjects.Length; ++i)
        {
            if (shouldBeActive)
            {
                disabledObjects[i].SetActive(false);
            }
        }
    }
}
