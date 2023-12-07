using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnRotate : MonoBehaviour
{
    public UnityEvent<float> onRotate;
    Quaternion lastRotation;
    [SerializeField]
    float angleWhereNormalizedOneHit=60f;

    void OnEnable()
    {
        lastRotation = transform.localRotation;
    }
    void Update()
    {
        if(lastRotation!=transform.localRotation)
        {
            float diffMag = Quaternion.Angle(lastRotation,transform.localRotation);
            onRotate?.Invoke(diffMag / angleWhereNormalizedOneHit);
            lastRotation = transform.localRotation;
        }
    }
}
