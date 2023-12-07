using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RequireFlag : MonoBehaviour
{
    public UnityEvent onCheckTrue, onCheckFalse, onSetTrue, onSetFalse;
    [SerializeField]
    MonoFlag condition;

    void OnEnable()
    {
        condition.onCheckFalse.AddListener(OnCheckFalse);
        condition.onCheckTrue.AddListener(OnCheckTrue);
        condition.onSetFalse.AddListener(OnSetFalse);
        condition.onSetTrue.AddListener(OnSetTrue);
    }

    void OnDisable()
    {
        condition.onCheckFalse.RemoveListener(OnCheckFalse);
        condition.onCheckTrue.RemoveListener(OnCheckTrue);
        condition.onSetFalse.RemoveListener(OnSetFalse);
        condition.onSetTrue.RemoveListener(OnSetTrue);
    }

    public void CheckCondition() {
        Debug.Log("checked?");
        bool b = condition.Get();
        if(b) {
            OnCheckTrue();
        }
        else {
            OnCheckFalse();
        }

    }

    void OnSetTrue() {
        onSetTrue?.Invoke();
    }
    
    void OnSetFalse() {
        onSetFalse?.Invoke();
    }

    void OnCheckFalse() {
        onCheckFalse?.Invoke();
    }

    void OnCheckTrue() {
        onCheckTrue?.Invoke();
    }
}
