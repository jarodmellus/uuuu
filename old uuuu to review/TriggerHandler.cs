using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using NaughtyAttributes;
using Sirenix;

public class TriggerHandler : MonoBehaviour {

    public enum TriggeredType {
        Enter,
        Exit,
        Stay
    }

    [System.Serializable]
    public class TriggerEventWrapper {
        public UnityEvent<Collider> onTriggerEnter, onTriggerExit, onTriggerStay;
    }

    void OnValidate() {

    }
    
    [Header("Layer")]
    [SerializeField] bool checkByLayer;
 
    [ShowIf("checkByLayer")]
    [SerializeField]
    Dictionary<LayerMask, TriggerEventWrapper> layerMasks;

    [Header("Tag")]
    [SerializeField] bool checkByTag;

    [ShowIf("checkByTag")]
    public Dictionary<string, TriggerEventWrapper> tags;


    [Header("Name")]
    [SerializeField] bool checkByName;   

    [ShowIf("checkByName")]  
    public Dictionary<string, TriggerEventWrapper>  names;



    public Dictionary<Collider, TriggerEventWrapper>  onlyAccept;

    [SerializeField] List<Collider> dontAccept;

    [Tooltip("These events will be invoked on any successful trigger, regardless of the collider's identity")]
    public TriggerEventWrapper anyColliderEvents;
    void OnTriggerEnter(Collider other) {Check(TriggeredType.Enter,other);}
    void OnTriggerExit(Collider other) {Check(TriggeredType.Exit,other);}
    void OnTriggerStay(Collider other) {Check(TriggeredType.Stay,other);}

    void Check(TriggeredType triggeredType, Collider other) { 
        List<UnityEvent<Collider>> eventsToInvoke = new List<UnityEvent<Collider>>();
        if(onlyAccept.Count>0) {
            if(!onlyAccept.ContainsKey(other)) {
                return;
            }
            //no need to check anything else
            goto DoInvoke;
        }

        if(dontAccept.Count>0) {
            if(dontAccept.Contains(other)) {
                return;
            }
        }

        if(checkByLayer) {
            foreach(KeyValuePair<LayerMask,TriggerEventWrapper> pair in layerMasks) {
                if((pair.Key | (1 << other.gameObject.layer)) != 0) {
                    eventsToInvoke.Add(GetEvent(pair.Value,triggeredType));
                }
            }
        }

        if(checkByTag) {
            if(tags.ContainsKey(other.tag)) {
                eventsToInvoke.Add(GetEvent(tags[other.tag],triggeredType));
            }
        }

        if(checkByName) {
            if(names.ContainsKey(other.name)) {
                eventsToInvoke.Add(GetEvent(names[other.name],triggeredType));
            }
        }

        DoInvoke:
            switch(triggeredType) {
                case TriggeredType.Enter:
                    eventsToInvoke.Add(anyColliderEvents.onTriggerEnter);
                    break;
                case TriggeredType.Exit:
                    eventsToInvoke.Add(anyColliderEvents.onTriggerExit);
                    break;
                case TriggeredType.Stay:
                    eventsToInvoke.Add(anyColliderEvents.onTriggerStay);
                    break;

            }

            foreach(UnityEvent<Collider> uEvent in eventsToInvoke) {
                uEvent?.Invoke(other);
            }

    }

    UnityEvent<Collider> GetEvent(TriggerEventWrapper wrapper, TriggeredType triggeredType) {
        switch(triggeredType) {
            case TriggeredType.Enter:
                return wrapper.onTriggerEnter;
            case TriggeredType.Exit:
                return wrapper.onTriggerExit;
            case TriggeredType.Stay:
                return wrapper.onTriggerStay;
        }      
        return null;  
    }
    
}