
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using NaughtyAttributes;

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
    public LayerMask_TriggerEventWrapperDictionary layerMasks;

    [Header("Tag")]
    [SerializeField] bool checkByTag;

    [Header("ReferenceTag")]
    [SerializeField] bool checkByReferenceTag;

    [ShowIf("checkByTag")]
    public String_TriggerEventWrapperDictionary tags;
    
    [ShowIf("checkByReferenceTag")]
    public ReferenceTag_TriggerEventWrapperDictionary referenceTags;


    [Header("Name")]
    [SerializeField] bool checkByName;   

    [ShowIf("checkByName")]  
    public String_TriggerEventWrapperDictionary names;

    [System.Serializable]
    public class String_TriggerEventWrapperDictionary : SerializableDictionary<string, TriggerEventWrapper> {}
    [System.Serializable]
    public class ReferenceTag_TriggerEventWrapperDictionary : SerializableDictionary<ReferenceTag, TriggerEventWrapper> {}
    [System.Serializable]
    public class LayerMask_TriggerEventWrapperDictionary : SerializableDictionary<LayerMask, TriggerEventWrapper> {}
    [System.Serializable]
    public class Rigidbody_TriggerEventWrapperDictionary : SerializableDictionary<Rigidbody, TriggerEventWrapper> {}

    public Rigidbody_TriggerEventWrapperDictionary onlyAccept;

    //Dictionary<Collider, Rigidbody> bodiesInTrigger = new Dictionary<Collider, Rigidbody>();
    //List<Rigidbody> bodiesInTrigger = new List<Rigidbody>();
    Dictionary<Rigidbody, List<Collider>> inTriggerArea = new Dictionary<Rigidbody,List<Collider>>();

    [SerializeField] List<Collider> dontAccept;

    [Tooltip("These events will be invoked on any successful trigger, regardless of the collider's identity")]
    public TriggerEventWrapper anyColliderEvents;
    public TriggerEventWrapper unacceptedEvents;
    void OnTriggerEnter(Collider other) {Check(TriggeredType.Enter,other);}
    void OnTriggerExit(Collider other) {Check(TriggeredType.Exit,other);}
    void OnTriggerStay(Collider other) {Check(TriggeredType.Stay,other);}


    void OnDisable()
    {
        inTriggerArea.Clear();
    }

    void Check(TriggeredType triggeredType, Collider other) { 

        if(other.GetComponent<TriggerHandlerIgnoreThisCollider>()!=null) {
            return;
        }

        if(other.attachedRigidbody==null) {
            //Debug.LogWarning("Collider on object " + other.ToString() + " does not have attached rigidbody" );

            return;

        }

        if(dontAccept.Count>0) {
            if(dontAccept.Contains(other)) {
                return;
            }
        }

        List<Collider> rbColList;
        switch(triggeredType) {
            case TriggeredType.Enter:
                if(!inTriggerArea.Keys.Contains(other.attachedRigidbody)) {
                    inTriggerArea.Add(other.attachedRigidbody, new List<Collider>(){other});
                }
                else {
                    rbColList = inTriggerArea[other.attachedRigidbody];
                    if(rbColList.Contains((other))) {
                        //object probably deactivated with no trigger exit called, continue
                        Debug.Log(other.name + "seems to already be in area, probably deactivated while in trigger!");
                    }
                    else
                    {
                        rbColList.Add(other);
                        return;
                    }
                    
                }
                break;
            case TriggeredType.Exit:
                if(!inTriggerArea.Keys.Contains(other.attachedRigidbody)) return;
                rbColList = inTriggerArea[other.attachedRigidbody];
                if(rbColList.Contains((other))) {
                    rbColList.Remove(other);
                }

                if(rbColList.Count==0) {
                    inTriggerArea.Remove(other.attachedRigidbody);
                }
                else {
                    return;
                }

                break;
            case TriggeredType.Stay:
                break;
        }

        if(other.attachedRigidbody==null) return; 
        List<UnityEvent<Collider>> eventsToInvoke = new List<UnityEvent<Collider>>();
        if(onlyAccept.Count>0) {
            if(!onlyAccept.ContainsKey(other.attachedRigidbody)) {
                goto DoInvoke;
            }

         
            eventsToInvoke.Add(GetEvent(onlyAccept[other.attachedRigidbody],triggeredType));
            //no need to check anything else
            goto DoInvoke;
        }

        if(checkByLayer) {
            foreach(KeyValuePair<LayerMask,TriggerEventWrapper> pair in layerMasks) {
                if((pair.Key | (1 << other.attachedRigidbody.gameObject.layer)) != 0) {
                    eventsToInvoke.Add(GetEvent(pair.Value,triggeredType));
                }
            }
        }

        if(checkByTag) {
            if(tags.ContainsKey(other.attachedRigidbody.tag)) {
                eventsToInvoke.Add(GetEvent(tags[other.attachedRigidbody.tag],triggeredType));
            }
        }

        if(checkByReferenceTag) {
            UseReferenceTag urt;
            if(other.attachedRigidbody.TryGetComponent<UseReferenceTag>(out urt)) {
                if(referenceTags.ContainsKey(urt.refTag)) {
                    eventsToInvoke.Add(GetEvent(referenceTags[urt.refTag],triggeredType));
                }
            }
  
        }

        if(checkByName) {
            if(names.ContainsKey(other.attachedRigidbody.name)) {
                eventsToInvoke.Add(GetEvent(names[other.attachedRigidbody.name],triggeredType));
            }
        }


        goto DoInvoke;


        DoInvoke:
            if(eventsToInvoke.Count==0) {
                switch(triggeredType) {
                    case TriggeredType.Enter:
                        eventsToInvoke.Add(unacceptedEvents.onTriggerEnter);
                        break;
                    case TriggeredType.Exit:
                        eventsToInvoke.Add(unacceptedEvents.onTriggerExit);
                        break;
                    case TriggeredType.Stay:
                        eventsToInvoke.Add(unacceptedEvents.onTriggerStay);
                        break;
                }
            }
            else {
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

    public void DeactivateSubject(Collider col) {
        col?.attachedRigidbody?.gameObject.SetActive(false);
    }

    /*
    public void ActivateSubject(Collider col) {
        col?.attachedRigidbody?.gameObject.SetActive(true);
    }
    */
}