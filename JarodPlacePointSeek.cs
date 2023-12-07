using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class JarodPlacePointSeek : MonoBehaviour
{
    public List<GameObject> acceptedObjects;

    public UnityEvent<GameObject> onGoodPlace, onBadPlace;
    public UnityEvent<GameObject> onRemoveGood, onRemoveBad;

    JarodPlacePoint placePoint;

    private void Awake()
    {
        placePoint = GetComponent<JarodPlacePoint>();
    }

    private void Start()
    {
        if (placePoint != null)
        {
            placePoint.onPlaceRigidbody += OnPlace;
            placePoint.onRemoveRigidbody += OnRemove;
        }
    }

    public void OnPlace(Rigidbody rb){
        if(acceptedObjects.Contains(rb.gameObject)) {
            onGoodPlace?.Invoke(rb.gameObject);
        }
        else {
            onBadPlace?.Invoke(rb.gameObject);
        }
    }
    public void OnRemove(Rigidbody rb){
        if(acceptedObjects.Contains(rb.gameObject)) {
            onRemoveGood?.Invoke(rb.gameObject);
        }
        else {
            onRemoveBad?.Invoke(rb.gameObject);
        }
    }

}
