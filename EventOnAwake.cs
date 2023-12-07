using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventOnAwake : MonoBehaviour
{
    public UnityEvent OnAwake;


    //Eventually Thread this instead of just running it
    private void Awake()
    {
        OnAwake?.Invoke();
    }
}
