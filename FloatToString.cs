using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatToString : MonoBehaviour
{
    public UnityEvent<string> Converted;


    public void ConvertFloatToString(float value)
    {
        Converted?.Invoke(value.ToString());
    }
}
