using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnAngleEvents : MonoBehaviour
{
    public class AngleEvents {
        public float angleValue;
        public UnityEvent onAngleGreater, onAngleLessThan, onAngleEqual;
    }

    public List<AngleEvents> angleEvents = new List<AngleEvents>();

    Quaternion initalAngle;

    [SerializeField] Vector3 axis;
    
    private void Awake() {
        initalAngle = transform.localRotation;
    }
}
