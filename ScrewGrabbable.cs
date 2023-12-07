using System;
using System.Collections;
using System.Collections.Generic;
using Autohand;
using NaughtyAttributes;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Grabbable))]
public class ScrewGrabbable : MonoBehaviour
{
    Grabbable grab;
    [Tooltip("Can this object be grabbed and screwed/unscrewed, or is the use of wrench or driver required?")]
    [SerializeField] bool handScrewable = true;
    [SerializeField] bool rightyTighty = true;
    [SerializeField] int numberOfTurns = 5;
    [Tooltip("1f is completely screwed in, 0f is completely unscrewed")]
    [SerializeField] float screwVal=1f;
    bool placeInScrewPlaceOnStart=false;
    [EnableIf("placeInScrewPlaceOnStart")]
    ScrewPlace placeHereOnStart;
    Rigidbody rigidbody;
    public Rigidbody body => rigidbody;
    ScrewPlace currentScrewPlace;

    public UnityEvent onScrewedIn, onUnscrewed;
    public UnityEvent<float> onScrewValueChange;

    [SerializeField]
    bool canBeRemoved = true;

    bool isBeingScrewed=true;
    
    void Awake()
    {
        grab = GetComponent<Grabbable>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Start()
    {
        grab.parentOnGrab = false;
        grab.isGrabbable = handScrewable;


/*
        if (placeInScrewPlaceOnStart)
        {
            placeHereOnStart.PlaceScrew(this);
        }

        OnBeginScrew();
*/



        
    }

    Quaternion previousRotation;
    float previousRotationY;

    void LateUpdate() {
        if (!isBeingScrewed) return;

        Quaternion rotationDelta = transform.localRotation * Quaternion.Inverse(previousRotation);
        //float deltaAngle = transform.localEulerAngles.y - previousRotationY;
        float deltaAngle = Mathf.Max(rotationDelta.eulerAngles.x,rotationDelta.eulerAngles.y,rotationDelta.eulerAngles.z);
        

        // Ensure deltaAngle is within the range of -180 to 180 degrees
        if (deltaAngle > 180.0f)
        {
            deltaAngle -= 360.0f;
        }
        else if (deltaAngle < -180.0f)
        {
            deltaAngle += 360.0f;
        }


        float sign = rightyTighty ? -1f: 1f;
        screwVal += sign * (deltaAngle/(numberOfTurns * 360f));
        if(screwVal>=.99f) {
            screwVal = 1f;
            transform.localRotation = previousRotation;
            onScrewedIn?.Invoke();
        }
     
        if (screwVal <= 0f)
        {
            if(canBeRemoved)
                OnUnscrewed();
            screwVal = 0f;
            transform.localRotation = previousRotation;
        }
        
        currentScrewPlace?.screwLine?.ChangeValue( screwVal);
        previousRotation = transform.localRotation;
    }

    void OnUnscrewed() {
        isBeingScrewed = false;
        rigidbody.constraints = RigidbodyConstraints.None;
        onUnscrewed?.Invoke();
        if (currentScrewPlace != null)
        {
            currentScrewPlace.RemoveFromPlace();
            currentScrewPlace = null;
        }
    }
    
    [Button]
    public void OnBeginScrew() {
        rigidbody.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        previousRotation = transform.localRotation;
        if (screwVal <= 0f) screwVal = 0.125f;
        isBeingScrewed = true;
    }

    public void OnBeginScrew(ScrewPlace screwPlace) {
        currentScrewPlace = screwPlace;
        OnBeginScrew();
    }


    public void SetScrewValue(float v)
    {
        screwVal = v;
    }
}
