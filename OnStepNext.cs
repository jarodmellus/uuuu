using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnStepNext : MonoBehaviour
{
    StepsManager stepsManager;
    public UnityEvent onStepNext;

    void Awake()
    {
        if(stepsManager==null)
            stepsManager=GetComponentInParent<StepsManager>();
    }
/*
    void OnEnable()
    {
        stepsManager.onNext.AddListener(() => onStepNext?.Invoke());
    }

    void OnDisable()
    {
        stepsManager.onNext.RemoveListener(() => onStepNext?.Invoke());
    }
    */
}
