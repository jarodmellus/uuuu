using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using UnityEngine.Events;

[RequireComponent(typeof(Grabbable))]
public class PlacePointSubject : MonoBehaviour
{
    [SerializeField] string subjectName="";
    [SerializeField]
    bool acceptAllPlacePoints = false;

    [SerializeField]
    [NaughtyAttributes.DisableIf("acceptAllPlacePoints")]
    List<PlacePoint> acceptedPlaces;
    Grabbable grabbable;
    PlacePoint currentPlace = null;
    public UnityEvent onValidPlace, onValidRemove;
    [NaughtyAttributes.DisableIf("acceptAllPlacePoints")]
    public UnityEvent onInvalidPlaced, onInvalidRemoved;

    void OnEnable()
    {
        grabbable=GetComponent<Grabbable>();  
        grabbable.OnPlacePointAddEvent+=OnAnyPlace;
        grabbable.OnPlacePointRemoveEvent+=OnAnyRemove;
    }

    void OnDisable()
    {
        grabbable.OnPlacePointAddEvent-=OnAnyPlace;
        grabbable.OnPlacePointRemoveEvent-=OnAnyRemove;
    }

    void OnAnyPlace(PlacePoint placePoint, Grabbable grabbable) {
        if(acceptAllPlacePoints) {
            onValidPlace?.Invoke();
        }
        else {
            if(acceptedPlaces.Contains(placePoint)) {
                onValidPlace?.Invoke();
                
            } else {
                onInvalidPlaced?.Invoke();
            }
        }

        currentPlace = placePoint;
    }

    void OnAnyRemove(PlacePoint placePoint, Grabbable grabbable) {
        //if(currentPlace==null) return;

        if(acceptAllPlacePoints) {
            onValidRemove?.Invoke();
        }
        else {
            if(acceptedPlaces.Contains(placePoint)) {
                onValidRemove?.Invoke();
            } else {
                onInvalidRemoved?.Invoke();
            }
        }

        currentPlace=null;
    }

}
