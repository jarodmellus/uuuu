using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MonoCondition))]
public class OnConditionStepPrevious : MonoBehaviour
{
    MonoCondition condition;
    StepsManager stepManager;

    private void Awake()
    {
        condition = GetComponent<MonoCondition>();
        stepManager = transform.parent.GetComponentInParent<StepsManager>();
        condition.onCondition.AddListener(()=>stepManager.Previous());
    }
}
