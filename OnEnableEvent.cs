using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteAlways]
public class OnEnableEvent : MonoBehaviour
{
    public UnityEvent OnEnabled;
    
    private void OnEnable()
    {
        OnEnabled?.Invoke();
    }
}
