using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PositionOnLine : MonoBehaviour
{
    public UnityEvent<float> onValueChange;
    public UnityEvent onEnd;
    [SerializeField]
    public Transform objectToMove, startPoint, endPoint;
    [SerializeField]
    float value;

    [SerializeField]
    bool allowOutOfRange = false;

    [SerializeField]
    float setValue;
    [NaughtyAttributes.Button]
    public void ValueButton() {
        ChangeValue(setValue);
    }

    public void ResetValue() {
        ChangeValue(0f);
    }

    public void ChangeValue(float val) {
        if(!allowOutOfRange) {
            if(val > 1f) val=1f;
            else if(val < 0)val = 0f;
        }
        
        value = val;
        objectToMove.position = (value * (endPoint.position - startPoint.position)) + startPoint.position;

        if(val == 1f) onEnd?.Invoke();

        onValueChange.Invoke(value);
    }

    public void AddToValue(float val) {
        ChangeValue(value + val);
    }

    public void SubtractFromValue(float val) {
        ChangeValue(value - val);
    }

    public float GetValue => value;
}
