using System.Collections;
using System.Collections.Generic;
using Autohand;
using UnityEngine;
using UnityEngine.Events;

public class TriggerHandlerRigidbodyEvents : MonoBehaviour
{
    enum EventType {
        Enter,
        Exit,
        Stay
    }

    public UnityEvent<Rigidbody> onBodyEnterTrigger, onBodyExitTrigger, onBodyStayInTrigger;

    TriggerHandler triggerHandler;
    void Awake()
    {
        triggerHandler = GetComponent<TriggerHandler>();
    }

    void Start()
    {
        triggerHandler.anyColliderEvents.onTriggerEnter.AddListener((col)=>RigidbodyEvent(col,EventType.Enter));
        triggerHandler.anyColliderEvents.onTriggerExit.AddListener((col)=>RigidbodyEvent(col,EventType.Exit));
        triggerHandler.anyColliderEvents.onTriggerStay.AddListener((col)=>RigidbodyEvent(col,EventType.Stay));
    }

    void RigidbodyEvent(Collider col, EventType ev){
        if (col.attachedRigidbody == null) return;
        switch(ev) {
            case EventType.Enter: onBodyEnterTrigger?.Invoke(col.attachedRigidbody); break;
            case EventType.Exit: onBodyExitTrigger?.Invoke(col.attachedRigidbody); break;
            case EventType.Stay: onBodyStayInTrigger?.Invoke(col.attachedRigidbody); break;
        }

    }
}
