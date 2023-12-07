using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FloatToValue : MonoBehaviour
{
    public UnityEvent<float> outputValue;

    [SerializeField, Tooltip("Number of decimal places (0-2)")]
    private int decimalPlaces = 2;

    [SerializeField, Tooltip("Scale for the output value")]
    private ScaleType scaleType = ScaleType.ZeroToOne;

    public enum ScaleType
    {
        ZeroToOne,
        ZeroToHundred
    }

    public void ConvertFloatToValue(float value)
    {
        value = Mathf.Clamp(value, 0, 100) / 100f; // Converts range from 0-100 to 0-1

        // If scale is 0-100, adjust value
        if (scaleType == ScaleType.ZeroToHundred)
        {
            value *= 100;
        }

        float formattedValue = (float)Math.Round(value, decimalPlaces);
        outputValue.Invoke(formattedValue);

        //Debug.Log($"{this} : Value Output is ({formattedValue})");
    }

}
