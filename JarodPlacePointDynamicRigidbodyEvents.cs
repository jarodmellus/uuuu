using System;
using UnityEngine;
using UnityEngine.Events;

public class JarodPlacePointDynamicRigidbodyEvents : MonoBehaviour {
    [SerializeField]
    JarodPlacePoint placePoint;
    public UnityEvent<Rigidbody> onPlace,onRemove,onHighlight,onStopHighlight;

    void Start()
    {
        placePoint = GetComponent<JarodPlacePoint>();
        placePoint.onPlaceRigidbody += OnPlace;
        placePoint.onRemoveRigidbody += OnRemove;
        placePoint.onHighlightRigidbody += OnHighlight;
        placePoint.onStopHighlightRigidbody += OnStopHighlight;
    }

    void OnPlace(Rigidbody rb) {
        onPlace?.Invoke(rb);
    }
    //Todo: Did not implement in jarod place point

    void OnRemove(Rigidbody rb) {
        onRemove?.Invoke(rb);
    }

    void OnHighlight(Rigidbody rb) {
        onHighlight?.Invoke(rb);
    }
    
    void OnStopHighlight(Rigidbody rb) {
        onStopHighlight?.Invoke(rb); 
    }
    
}
