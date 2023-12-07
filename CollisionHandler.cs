using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using NaughtyAttributes;

public class CollisionHandler : MonoBehaviour {

    public enum CollisionedType {
        Enter,
        Exit,
        Stay
    }

    [System.Serializable]
    public class CollisionEventWrapper {
        public UnityEvent<Collider> onCollisionEnter, onCollisionExit, onCollisionStay;
    }

    void OnValidate() {

    }
    
    [Header("Layer")]
    [SerializeField] bool checkByLayer;
 
    [ShowIf("checkByLayer")]
    public LayerMask_CollisionEventWrapperDictionary layerMasks;

    [Header("Tag")]
    [SerializeField] bool checkByTag;

    [ShowIf("checkByTag")]
    public String_CollisionEventWrapperDictionary tags;


    [Header("Name")]
    [SerializeField] bool checkByName;   

    [ShowIf("checkByName")]  
    public String_CollisionEventWrapperDictionary names;

    [System.Serializable]
    public class String_CollisionEventWrapperDictionary : SerializableDictionary<string, CollisionEventWrapper> {}
    [System.Serializable]
    public class LayerMask_CollisionEventWrapperDictionary : SerializableDictionary<LayerMask, CollisionEventWrapper> {}
    [System.Serializable]
    public class Rigidbody_CollisionEventWrapperDictionary : SerializableDictionary<Rigidbody, CollisionEventWrapper> {}

    public Rigidbody_CollisionEventWrapperDictionary onlyAccept;

    //Dictionary<Collider, Rigidbody> bodiesInCollision = new Dictionary<Collider, Rigidbody>();
    //List<Rigidbody> bodiesInCollision = new List<Rigidbody>();
    Dictionary<Rigidbody, List<Collider>> inColliderArea = new Dictionary<Rigidbody,List<Collider>>();
    

    [SerializeField] List<Collider> dontAccept;

    [Tooltip("These events will be invoked on any successful Collider, regardless of the collider's identity")]
    public CollisionEventWrapper anyColliderEvents;
    public CollisionEventWrapper unacceptedEvents;
    void OnCollisionEnter(Collision other) {Check(CollisionedType.Enter,other.collider);}
    void OnCollisionExit(Collision other) {Check(CollisionedType.Exit,other.collider);}
    void OnCollisionStay(Collision other) {Check(CollisionedType.Stay,other.collider);}

    void Check(CollisionedType CollisionedType, Collider other) { 

        if(other.attachedRigidbody==null) {
            Debug.LogWarning("Collider on object " + other.ToString() + " does not have attached rigidbody" );
            return;
        }

        if(dontAccept.Count>0) {
            if(dontAccept.Contains(other)) {
                return;
            }
        }

        List<Collider> rbColList;
        switch(CollisionedType) {
            case CollisionedType.Enter:
                if(!inColliderArea.Keys.Contains(other.attachedRigidbody)) {
                    inColliderArea.Add(other.attachedRigidbody, new List<Collider>(){other});
                }
                else {
                    rbColList = inColliderArea[other.attachedRigidbody];
                    if(rbColList.Contains((other))) return;
                    rbColList.Add(other);
                    return;
                }
                break;
            case CollisionedType.Exit:
                if(!inColliderArea.Keys.Contains(other.attachedRigidbody)) return;
                rbColList = inColliderArea[other.attachedRigidbody];
                if(rbColList.Contains((other))) {
                    rbColList.Remove(other);
                }

                if(rbColList.Count==0) {
                    inColliderArea.Remove(other.attachedRigidbody);
                }
                else {
                    return;
                }

                break;
            case CollisionedType.Stay:
                break;
        }

        if(other.attachedRigidbody==null) return; 
        List<UnityEvent<Collider>> eventsToInvoke = new List<UnityEvent<Collider>>();
        if(onlyAccept.Count>0) {
            if(!onlyAccept.ContainsKey(other.attachedRigidbody)) {
                goto DoInvoke;
            }

         
            eventsToInvoke.Add(GetEvent(onlyAccept[other.attachedRigidbody],CollisionedType));
            //no need to check anything else
            goto DoInvoke;
        }

        if(checkByLayer) {
            foreach(KeyValuePair<LayerMask,CollisionEventWrapper> pair in layerMasks) {
                if((pair.Key | (1 << other.attachedRigidbody.gameObject.layer)) != 0) {
                    eventsToInvoke.Add(GetEvent(pair.Value,CollisionedType));
                }
            }
        }

        if(checkByTag) {
            if(tags.ContainsKey(other.attachedRigidbody.tag)) {
                eventsToInvoke.Add(GetEvent(tags[other.attachedRigidbody.tag],CollisionedType));
            }
        }

        if(checkByName) {
            if(names.ContainsKey(other.attachedRigidbody.name)) {
                eventsToInvoke.Add(GetEvent(names[other.attachedRigidbody.name],CollisionedType));
            }
        }


        goto DoInvoke;


        DoInvoke:
            if(eventsToInvoke.Count==0) {
                switch(CollisionedType) {
                    case CollisionedType.Enter:
                        eventsToInvoke.Add(unacceptedEvents.onCollisionEnter);
                        break;
                    case CollisionedType.Exit:
                        eventsToInvoke.Add(unacceptedEvents.onCollisionExit);
                        break;
                    case CollisionedType.Stay:
                        eventsToInvoke.Add(unacceptedEvents.onCollisionStay);
                        break;
                }
            }
            else {
                switch(CollisionedType) {
                    case CollisionedType.Enter:
                        eventsToInvoke.Add(anyColliderEvents.onCollisionEnter);
                        break;
                    case CollisionedType.Exit:
                        eventsToInvoke.Add(anyColliderEvents.onCollisionExit);
                        break;
                    case CollisionedType.Stay:
                        eventsToInvoke.Add(anyColliderEvents.onCollisionStay);
                        break;

                }
            }

            foreach(UnityEvent<Collider> uEvent in eventsToInvoke) {
                uEvent?.Invoke(other);
            }

    }

    UnityEvent<Collider> GetEvent(CollisionEventWrapper wrapper, CollisionedType CollisionedType) {
        switch(CollisionedType) {
            case CollisionedType.Enter:
                return wrapper.onCollisionEnter;
            case CollisionedType.Exit:
                return wrapper.onCollisionExit;
            case CollisionedType.Stay:
                return wrapper.onCollisionStay;
        }      
        return null;  
    }
    
}