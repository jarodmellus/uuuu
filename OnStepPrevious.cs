using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStepPrevious : MonoBehaviour
{
    StepsManager stepsManager;
    public UnityEvent onStepPrevious;

    void Awake()
    {
        if(stepsManager==null)
            stepsManager=GetComponentInParent<StepsManager>();
    }

    void OnDisable()
    {
        stepsManager.onPrevious.RemoveAllListeners();
        stepsManager.onPrevious.AddListener(()=>onStepPrevious?.Invoke());
    }
}
