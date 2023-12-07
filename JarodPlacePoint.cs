using System.Xml.Linq;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;
using NaughtyAttributes;
using UnityEngine.Events;
using System;

public class JarodPlacePoint : MonoBehaviour
{

    public Dictionary<Grabbable, HandGrabEvent> highlightedObjects = new Dictionary<Grabbable, HandGrabEvent>();

    [InfoBox(
        "This approach assumes that any colliders that trigger this object's children colliders will have an attached rigidbody at their root.",
    EInfoBoxType.Normal)]

    [InfoBox(
            "A different approach to Autohand's place points that allows for multiple colliders rather than a single physics sphere check. Uses the 'Trigger Handler' component on child colliders for collision checks.",
        EInfoBoxType.Normal)]

    [SerializeField]
    TriggerHandler triggerHandler;

    public bool heldPlaceOnly = true;

    [SerializeField]
    protected bool removable = true;
    public bool IsRemovable => removable;
    public void SetRemoveable(bool isRemovable) { removable = isRemovable; }
    [SerializeField]
    protected bool removeOnDisable = true;
    [SerializeField]
    protected bool canPlace=true;
    [SerializeField]
    bool usePhysics = true;
    [SerializeField]
    bool parentOnPlace = true;

    [SerializeField]
    [ShowIf("usePhysics")]
    bool isKinematicOnPlace = true;
    [ShowNonSerializedField]
    bool isPlaced = false;
    public bool GetIsPlaced => isPlaced;

    public bool isPlacedOnStart;
    bool isHighlighting=false;

    [ShowIf("isPlacedOnStart")]
    [SerializeField]
    GameObject objectToPlaceOnStart;
    GameObject placedObject = null;
    public GameObject GetPlacedObject => placedObject;
    Transform previousParent = null;
    [SerializeField]
    public Transform placeOffset;

    [SerializeField]
    [ShowIf(EConditionOperator.And,"usePhysics","isPlacedOnStart")]
    Rigidbody placedObjectRigidbody = null;

    [SerializeField] 
    PlaceholderMesh placeHolderMeshObject;

    GameObject initOriginal;

    public UnityEvent onPlace, onRemove, onHighlight, onStopHighlight;
    public Action<Rigidbody> onPlaceRigidbody, onRemoveRigidbody;

    [SerializeField]
    bool disableGrabbable = false;

    [SerializeField]
    bool toggleRenderer = false;
    public Action<Rigidbody> onHighlightRigidbody;
    public Action<Rigidbody> onStopHighlightRigidbody;
    HandGrabEvent grabEvent;

    void Awake()
    {
        if(placeOffset==null) placeOffset = transform;
    }

    void OnEnable()
    {
        if (placedObject != null)
        {
            if (!removeOnDisable)
            {
                placedObject.transform.position = placeOffset.transform.position;
                placedObject.transform.rotation = placeOffset.transform.rotation;
            }
        }
        
        if(triggerHandler==null) return;

        triggerHandler.anyColliderEvents.onTriggerEnter.AddListener(OnHighlight);
        triggerHandler.anyColliderEvents.onTriggerExit.AddListener(OnStopHighlight);
    }

    
    void OnDisable()
    {
        if (placedObject != null)
        {
            if (removeOnDisable) Remove();
            placedObject = null;
        }

        placeHolderMeshObject?.OnStopHighlight();

        if(triggerHandler==null) return;
        triggerHandler.anyColliderEvents.onTriggerEnter.RemoveListener(OnHighlight);
        triggerHandler.anyColliderEvents.onTriggerExit.RemoveListener(OnStopHighlight);

    }

    void Start() {
        
        initOriginal = placeHolderMeshObject?.original;


        if (!isPlacedOnStart) return;
    
        if(objectToPlaceOnStart==null) return;
    

        if(usePhysics) {
            if(!objectToPlaceOnStart.TryGetComponent<Rigidbody>(out placedObjectRigidbody))return;
            Place(placedObjectRigidbody);
        }
        else {
            Place(objectToPlaceOnStart);
        }
    }

    void Place(GameObject _placedObject) {
        if(isPlaced) return;

        if(placedObject!=null) return;

        placeHolderMeshObject?.copy.gameObject.SetActive(false);
        placedObject = _placedObject;
        placedObject.transform.position=placeOffset.transform.position;
        placedObject.transform.rotation=placeOffset.transform.rotation;
        
        if(parentOnPlace) {
            previousParent = _placedObject.transform.parent;
            _placedObject.transform.SetParent(placeOffset.transform);
            
        }

        if(toggleRenderer) {
            Renderer[] renderers =_placedObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in renderers) {
                rend.enabled = false;
            }
        }

        Rigidbody rb;
        if(_placedObject.TryGetComponent<Rigidbody>(out rb))
            onPlaceRigidbody?.Invoke(rb);

        onPlace?.Invoke();
        isPlaced=true;
    }
    public void Place(Grabbable grabbable) {
        Place(grabbable.body);
    }

    public void Place(Collider collider) {
        if(isPlaced) return;
        Place(collider.attachedRigidbody.gameObject);
    }

    protected void Place(Rigidbody placeObjBody) {
        if(isPlaced) return;

        if(placeObjBody==null) return;

        Grabbable grabbable;
        if(!placeObjBody.TryGetComponent<Grabbable>(out grabbable)) return;
        
        
        if(grabbable.HeldCount()>0) {
            return;
        }

        if (removable)
        {
            grabEvent = (hand, grab) => Remove(true);
            grabbable.OnGrabEvent += grabEvent;
        }
        
        Place(placeObjBody.gameObject);
        placedObjectRigidbody=placeObjBody;
        if(isKinematicOnPlace)
            placeObjBody.isKinematic=true;

        if(!highlightedObjects.ContainsKey(grabbable)) return;
        
        HandGrabEvent releaseAction=highlightedObjects[grabbable];
        grabbable.OnReleaseEvent -= releaseAction;
        highlightedObjects.Remove(grabbable);
    
        grabbable.OnPlacePointAddEvent?.Invoke(null,grabbable);
    
        if(disableGrabbable) {
            grabbable.isGrabbable = false;
        }
    }

    public void Remove(bool forceRemove=false) {
        if(!isPlaced) return;
        Grabbable grabbable;

        if(!forceRemove)
            if(!removable) return;
            
        if(placedObject==null) return;

        if(parentOnPlace) {
            //placedObject.transform.SetParent(previousParent);
            placedObject.transform.SetParent(null);
            previousParent = null;
        }
        
        if(placedObject.TryGetComponent<Grabbable>(out grabbable)) {
            if (removable)
            {
                grabbable.OnGrabEvent -= grabEvent;
            }
        }

        Rigidbody rb;
        if(placedObject.TryGetComponent<Rigidbody>(out rb))
            onRemoveRigidbody?.Invoke(rb);

        if(toggleRenderer) {
            Renderer[] renderers =placedObject.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in renderers) {
                rend.enabled = true;
            }
        }

        placedObject = null;
        placeHolderMeshObject?.copy.gameObject.SetActive(true);

        if(usePhysics) {
            placedObjectRigidbody.isKinematic=false;
            placedObjectRigidbody=null;
        }

        grabbable.OnPlacePointRemoveEvent?.Invoke(null,grabbable);
        isPlaced=false;
        onRemove?.Invoke();
        
    }

    public void TryPlace(GameObject _placeObj) {
        if(CanPlace(_placeObj)) {
            Rigidbody rb;
            if(_placeObj.TryGetComponent<Rigidbody>(out rb))
                Place(rb);
            else
                Place(_placeObj); 
        }
    }

    public void OnHighlight(Collider placeCol) {
        if (isPlaced) return;
       
        if (!heldPlaceOnly)
        {
            Place(placeCol.attachedRigidbody.gameObject);
            return;
        }

        Grabbable grabbable;
        if(!placeCol.attachedRigidbody.TryGetComponent<Grabbable>(out grabbable)) return;
        if(highlightedObjects.ContainsKey(grabbable)) return;

        HandGrabEvent releaseAction = (hand, grab) => Place(placeCol.attachedRigidbody);

        highlightedObjects.Add(grabbable,releaseAction);
        grabbable.OnReleaseEvent+=releaseAction;

        isHighlighting=true;
        
        placeHolderMeshObject?.OnHighlight();
        onHighlight?.Invoke();

        
        if(placeCol.attachedRigidbody!=null)
            onHighlightRigidbody?.Invoke(placeCol.attachedRigidbody);
        //placeHolderMeshObject.original = placeCol.attachedRigidbody.gameObject;
        //placeHolderMeshObject.GenerateMeshAtRuntime();
    }

    public void OnStopHighlight(Collider placeCol) {

        if (!heldPlaceOnly)
        {
            Remove(placeCol.attachedRigidbody.gameObject);
            return;
        }
        
        Grabbable grabbable;
        if(!placeCol.attachedRigidbody.TryGetComponent<Grabbable>(out grabbable)) return;
        if(!highlightedObjects.ContainsKey(grabbable)) return;
        
        HandGrabEvent releaseAction=highlightedObjects[grabbable];
        grabbable.OnReleaseEvent -= releaseAction;
        highlightedObjects.Remove(grabbable);

        isHighlighting=false;
        placeHolderMeshObject?.OnStopHighlight();
        onStopHighlight?.Invoke();

        if(placeCol.attachedRigidbody!=null)
            onStopHighlightRigidbody?.Invoke(placeCol.attachedRigidbody);

    }
    
    public bool CanPlace(GameObject placeObj) {
        if(isPlaced) return false;
        if(!isHighlighting) return false;

        if(placedObject != null)
            return false;  

        Grabbable grabbable;
        if(placeObj.TryGetComponent<Grabbable>(out grabbable)) {
            //if (heldPlaceOnly && grabbable.HeldCount() == 0)
            if(grabbable.IsHeld())
                return false;
        }
        
        return true;
    }
}
