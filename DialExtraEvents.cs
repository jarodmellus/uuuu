using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialExtraEvents : MonoBehaviour
{
    [SerializeField] int sigFigs = 0;
    ExperimentalGrabbableDial dial;

    
    public UnityEvent<string> onAngleChangeGetValueTimesMultiplierAsString;
    public UnityEvent<float> onAngleChangeGetValueAsPercentage;

    public float offsetForPercentage = 0f, multiplierForPercentage = 1f;
    
    private void Awake() {
        dial = GetComponent<ExperimentalGrabbableDial>();
        dial.AngleChangedEvent.AddListener((val)=>
            onAngleChangeGetValueTimesMultiplierAsString?.Invoke(
                (dial.GetRepresentedValue()).ToString("F" + sigFigs.ToString())
            )
        );

        dial.AngleChangedEvent.AddListener((val)=>
            onAngleChangeGetValueAsPercentage?.Invoke(
                (offsetForPercentage + (dial.GetNormalizedValue() * multiplierForPercentage))
            )
        );
    }
    
}
