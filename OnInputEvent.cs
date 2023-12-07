using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class OnInputEvent : MonoBehaviour
{
    public InputAction inputAction;
    public UnityEvent onInputPerformed, onInputCanceled, onInputStarted;
    public UnityEvent<float> onInputValueChange;
    [SerializeField]
    float value;

    void OnEnable()
    {
        inputAction.Enable();
        inputAction.started += OnInputStarted;
        inputAction.canceled += OnInputCancelled;
        inputAction.performed += OnInputPerformed;
    }

    void OnDisable()
    {
        inputAction.Disable();
        inputAction.started -= OnInputStarted;
        inputAction.canceled -= OnInputCancelled;
        inputAction.performed -= OnInputPerformed;
    }

    float lastValue = 0f;
    void Update()
    {
        //if(!inputAction.triggered) return;  
        value = inputAction.ReadValue<float>();
        if (lastValue != value)
        {
            onInputValueChange?.Invoke(value);
            lastValue = value;
        }



    }

    void OnInputStarted(InputAction.CallbackContext ctx) => onInputStarted?.Invoke();
    void OnInputPerformed(InputAction.CallbackContext ctx) => onInputPerformed?.Invoke();
    void OnInputCancelled(InputAction.CallbackContext ctx) => onInputCanceled?.Invoke();

}
