using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using NaughtyAttributes;

public class JarodPlacePointDebugger : MonoBehaviour
{
    [SerializeField]
    PlacePoint placePoint;
    [SerializeField] 
    Grabbable targetGrabbable;

    [Button]
    public void Remove() {
        placePoint.Remove();
    }

    [Button]
    public void ForcePlace() {
        placePoint.Place(targetGrabbable);
    }
}
