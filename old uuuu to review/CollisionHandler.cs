using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using NaughtyAttributes;

public class CollisionHandler : MonoBehaviour {

    public enum CollisionType {
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
    public Dictionary<LayerMask, CollisionEventWrapper>  layerMasks;

    [Header("Tag")]
    [SerializeField] bool checkByTag;

    [ShowIf("checkByTag")]
    public Dictionary<string, CollisionEventWrapper>  tags;


    [Header("Name")]
    [SerializeField] bool checkByName;   

    [ShowIf("checkByName")]  
    public Dictionary<string, CollisionEventWrapper>  names;


    public Dictionary<Collider, CollisionEventWrapper>  onlyAccept;

    [SerializeField] List<Collider> dontAccept;

    [Tooltip("These events will be invoked on any successful Collision, regardless of the collider's identity")]
    public CollisionEventWrapper anyColliderEvents;
    void OnCollisionEnter(Collision col) {Check(CollisionType.Enter,col);}
    void OnCollisionExit(Collision col) {Check(CollisionType.Exit,col);}
    void OnCollisionStay(Collision col) {Check(CollisionType.Stay,col);}

    void Check(CollisionType CollisionType, Collision col) { 
        List<UnityEvent<Collider>> eventsToInvoke = new List<UnityEvent<Collider>>();
        if(onlyAccept.Count>0) {
            if(!onlyAccept.ContainsKey(col.collider)) {
                return;
            }
            //no need to check anything else
            goto DoInvoke;
        }

        if(dontAccept.Count>0) {
            if(dontAccept.Contains(col.collider)) {
                return;
            }
        }

        if(checkByLayer) {
            foreach(KeyValuePair<LayerMask,CollisionEventWrapper> pair in layerMasks) {
                if((pair.Key | (1 << col.gameObject.layer)) != 0) {
                    eventsToInvoke.Add(GetEvent(pair.Value,CollisionType));
                }
            }
        }

        if(checkByTag) {
            if(tags.ContainsKey(col.collider.tag)) {
                eventsToInvoke.Add(GetEvent(tags[col.collider.tag],CollisionType));
            }
        }

        if(checkByName) {
            if(names.ContainsKey(col.collider.name)) {
                eventsToInvoke.Add(GetEvent(names[col.collider.name],CollisionType));
            }
        }

        DoInvoke:
            switch(CollisionType) {
                case CollisionType.Enter:
                    eventsToInvoke.Add(anyColliderEvents.onCollisionEnter);
                    break;
                case CollisionType.Exit:
                    eventsToInvoke.Add(anyColliderEvents.onCollisionExit);
                    break;
                case CollisionType.Stay:
                    eventsToInvoke.Add(anyColliderEvents.onCollisionStay);
                    break;

            }

            foreach(UnityEvent<Collider> uEvent in eventsToInvoke) {
                uEvent?.Invoke(col.collider);
            }

    }

    UnityEvent<Collider> GetEvent(CollisionEventWrapper wrapper, CollisionType CollisionType) {
        switch(CollisionType) {
            case CollisionType.Enter:
                return wrapper.onCollisionEnter;
            case CollisionType.Exit:
                return wrapper.onCollisionExit;
            case CollisionType.Stay:
                return wrapper.onCollisionStay;
        }      
        return null;  
    }
    
}