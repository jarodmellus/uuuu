using System.Runtime.CompilerServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using NaughtyAttributes;
using Autohand;
using TelegramSystem;
using Photon.Pun;
using System;
using Photon.Realtime;

public class XRUserTransformObjectTool : MonoBehaviour
{

    [SerializeField] Autohand.AutoHandPlayer autoHandPlayer;

    public LineRenderer lineRenderer;
    public Transform transformTo;

    [SerializeField] InputActionReference possessAction;

    [SerializeField] InputActionReference inputForwardAxis;

    [SerializeField] float rotationMultiplier = 100f, forwardMultiplier = .5f;

    Vector3 lastPosition;

    [SerializeField] float forceMagnitude = 10f;
 
    [SerializeField] float damping = 1f;

    [SerializeField] bool forceGrabIfClose = true;

    [SerializeField] [EnableIf("forceGrabIfClose")] float forceGrabDistance = .1f;

    [SerializeField] float rayLength = 20f;
    [SerializeField][DisableIf("true")] float objDistance=0f;

    [SerializeField] HighlightManager highlightedHM, positionHM;

    [SerializeField] bool useLeftHand = false;

    [SerializeField] bool ignoreAllOverride = false;
    [SerializeField][EnableIf("ignoreAllOverride")] string layerToIgnore = "IgnoreAll";

    [SerializeField] bool moveObject = true;
    public void SetObjectMovable(bool val) => moveObject = val;
    [SerializeField] bool rotateObject = true;
    [SerializeField] [Tooltip("Ratchting style rotation of object about the y axis using the controller's z rotation. XZ rotation is locked to 0.")] bool ratchet = false;
    [SerializeField] bool lockXZRotation = false;

    public void SetObjectRotatable(bool val) => rotateObject = val;
    RaycastHit hit;

    [SerializeField] LayerMask toolLayerMask;
    [SerializeField] bool toolActive=true;

    bool inputTryUseTool;
    bool selecting = true;

    XRUserTransformable lastFrameHighlightedObj, currentFrameHighlightedObj, currentlyPossessedObj;

    public XRUserTransformable GetCurrentlyPossessedObject => currentlyPossessedObj;

    public UnityEvent<XRUserTransformable> onPossess, onFree;
    public UnityEvent<XRUserTransformable> onHighlight, onStopHighlight;

    [HideInInspector]
    public bool userScaling;

    [SerializeField]
    float movePositionLerp = .0125f;

    [SerializeField]
    bool setXZRotationToZeroOnPossess=true;



    float maxCooldown=.25f;
    float currentCooldown = 0f;


    Quaternion lastControllerRotation;


    void Start()
    {
        if (possessAction != null)
        {
            possessAction.action.Enable();
            possessAction.action.started += ctx => inputTryUseTool = true;
            possessAction.action.canceled += ctx => inputTryUseTool = false;
        }

        if(inputForwardAxis!=null) {
            inputForwardAxis.action.Enable();
        }

        Autohand.Hand _hand = useLeftHand ? autoHandPlayer.handLeft : autoHandPlayer.handRight;

        _hand.OnGrabbed += (hand, grab) => gameObject.SetActive(false);

        lastControllerRotation = transformTo.rotation;
        
    }

    void OnDisable()
    {
        if(currentlyPossessedObj!=null) {
            if(currentlyPossessedObj.GetGrabbableNetworkController.IsHeldNetworked) {
                FreeObject();
            }
        }

        if(currentFrameHighlightedObj!=null) {
            if(currentFrameHighlightedObj.IsHighlighted) {
                currentFrameHighlightedObj.StopHighlight(useLeftHand);    
            }
        }
        
        StopAllHighlighting();

        lastFrameHighlightedObj = null;
        currentFrameHighlightedObj = null;
        currentlyPossessedObj = null;
    }

    void ResetObjectTransform() {
        if(currentlyPossessedObj==null) return;

        currentlyPossessedObj.transform.rotation = _lastRotation;
        currentlyPossessedObj.GetBody.velocity = Vector3.zero;
        currentlyPossessedObj.GetBody.angularVelocity = Vector3.zero;
    }

    Quaternion _lastRotation;
    public void PossessObject(XRUserTransformable objectToPossess)
    {
        if(objectToPossess.GetGrabbableNetworkController.IsHeldNetworked) {

            if(!objectToPossess.GetPhotonView.IsMine) {
            
                return;
            }
        }

        if (currentlyPossessedObj != null)
        {
            //no change to possessed obj, end
            if (currentlyPossessedObj == objectToPossess)
            {
                return;
            }
            else
            {
                //make sure we free the last object we were possessing with this hand
                FreeObject();
            }

        }

        currentlyPossessedObj = objectToPossess;

        onPossess?.Invoke(currentlyPossessedObj);

        currentlyPossessedObj.Possess(useLeftHand);

        Rigidbody possessedBody = currentlyPossessedObj.GetBody;

        possessedBody.velocity = Vector3.zero;
        possessedBody.angularVelocity = Vector3.zero;
        transformTo.position = possessedBody.position;
        objDistance = Vector3.Distance(possessedBody.position+calculatedOffset, transform.position);
        _lastRotation = currentlyPossessedObj.transform.rotation;

        if (rotateObject) {
            if (setXZRotationToZeroOnPossess && !ratchet)
                transformTo.rotation = Quaternion.Euler(0, currentlyPossessedObj.transform.eulerAngles.y, 0);
        }

        NetworkPlayerRaycastManager.instance.ActivateRay(false,useLeftHand,currentlyPossessedObj.GetPhotonView);
    }

    public void FreeObject()
    {
        if (currentlyPossessedObj == null) return;
        //if(!currentlyPossessedObj.GetGrabbableNetworkController.IsHeldNetworked) return;

        
        currentlyPossessedObj.Free(useLeftHand);
        currentCooldown = 0f;

        //currentlyPossessedObj.GetBody.useGravity = true;
        currentlyPossessedObj = null;
        NetworkPlayerRaycastManager.instance.DeactivateRays(useLeftHand);
        onFree?.Invoke(null);
    }

    void Update()
    {
        if (!lineRenderer.gameObject.activeSelf) return;

        lineRenderer.SetPosition(0, transform.position);

        if (currentlyPossessedObj != null)
        {
            calculatedOffset = currentlyPossessedObj.transform.TransformPoint(hitOffset)-currentlyPossessedObj.transform.position;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, currentlyPossessedObj.transform.position + calculatedOffset);
        }
        lastControllerRotation = transformTo.rotation;

    }

    void FixedUpdate()
    {
        if(currentCooldown<maxCooldown) {
            currentCooldown += Time.fixedDeltaTime;
            return;
        }

        ToolUpdate();
    
        if (currentlyPossessedObj == null) return;
        ObjectMove();
    }

    void ToolUpdate() {
        if (selecting)
        {
            TryInspect();
            TryUseTool();
        }
        else {
            if(currentlyPossessedObj==null) {
                onFree?.Invoke(null);
                selecting = true;
                return;
            }

            if (!inputTryUseTool)
            {
                selecting = true;
                if (currentlyPossessedObj == null)
                {
                    onFree?.Invoke(null);
                }
                else
                {
                    StopAllHighlighting();
                }

                FreeObject();
            }
        } 
    }

    void TryUseTool() {
        if(!inputTryUseTool) return;
        if(currentFrameHighlightedObj==null) return;

        PossessObject(currentFrameHighlightedObj);
        selecting = false;
        currentFrameHighlightedObj = null;
    }

    Vector3 hitOffset;
    void TryInspect()
    {
        if(!toolActive) return;
        if (ignoreAllOverride) toolLayerMask |= 1 << LayerMask.NameToLayer(layerToIgnore);
        if (!Physics.Raycast(transform.position, transform.forward, out hit, rayLength, toolLayerMask, QueryTriggerInteraction.Ignore))
        {
            //nothing is being hit

            if(lastFrameHighlightedObj!=null) {
                //stop highlighting the last thing we were highlighting

                lastFrameHighlightedObj.StopHighlight(useLeftHand);
                onStopHighlight?.Invoke(lastFrameHighlightedObj);
                lastFrameHighlightedObj = null;
            }
            if (currentlyPossessedObj != null)
            {
                currentlyPossessedObj.StopHighlight(useLeftHand);
                currentlyPossessedObj = null;
            }
            if (currentFrameHighlightedObj != null)
            {
                currentFrameHighlightedObj.StopHighlight(useLeftHand);
            }

            //nothing new has been hit
            currentFrameHighlightedObj = null;
            
            return;
        }

        
        //we hit something

        
        if (hit.collider.attachedRigidbody == null)
        {
            StopAllHighlighting();
            lastFrameHighlightedObj = null;
            currentFrameHighlightedObj = null;
            return;
        }

        if(!hit.collider.attachedRigidbody.TryGetComponent<XRUserTransformable>(out currentFrameHighlightedObj)) {
            StopAllHighlighting();
            currentFrameHighlightedObj = null;
            lastFrameHighlightedObj = null;
            return;
        }

        hitOffset = currentFrameHighlightedObj.transform.InverseTransformPoint(hit.point);

        if(currentFrameHighlightedObj.GetGrabbable.IsHeld()) {
            currentFrameHighlightedObj = null;
            lastFrameHighlightedObj = null;
            return;
        }

        //don't highlight an object someone else is possessing
        if(currentFrameHighlightedObj.GetGrabbableNetworkController.IsHeldNetworked) {
            if (!currentFrameHighlightedObj.GetPhotonView.IsMine) { 
                currentFrameHighlightedObj = null;
                lastFrameHighlightedObj = null;
                return; 
            }
        }

        //we're highlighting the same thing we hit last frame
        if (lastFrameHighlightedObj == currentFrameHighlightedObj) {
            return; 
        }

        //last frame we were highlighting something different, stop highlighting it
        if(lastFrameHighlightedObj!=null) {
            lastFrameHighlightedObj.StopHighlight(useLeftHand);
            lastFrameHighlightedObj = null;
            onStopHighlight?.Invoke(lastFrameHighlightedObj);
        }

       

        onHighlight?.Invoke(currentFrameHighlightedObj);
        currentFrameHighlightedObj.Highlight(useLeftHand);

        lastFrameHighlightedObj = currentFrameHighlightedObj;
    }

    void StopAllHighlighting() {
        if(currentlyPossessedObj!=null)
            currentlyPossessedObj.StopHighlight(useLeftHand);
        if(currentFrameHighlightedObj!=null)
            currentFrameHighlightedObj.StopHighlight(useLeftHand);
        if(lastFrameHighlightedObj!=null)
            lastFrameHighlightedObj.StopHighlight(useLeftHand);
    }

    float _forward;
    Vector3 _dir;
    Vector3 _dampingForce;
    float _dist;
    Vector3 calculatedOffset=Vector3.zero;

    void ObjectMove() {
        if (!moveObject && !rotateObject && !userScaling) return;
        if(userScaling) return;

        Rigidbody selectedBody = currentlyPossessedObj.GetBody;

        if (moveObject)
        {
            
            _dir = transformTo.position - (selectedBody.position);
            _dampingForce = -currentlyPossessedObj.GetBody.velocity * damping;
            transformTo.position = transform.position + (transform.forward * objDistance);
            _dist = Vector3.Distance(transformTo.position,selectedBody.position);
            selectedBody.AddForce(((_dir.normalized * forceMagnitude) + (_dampingForce)) * selectedBody.mass);
            _forward = (inputForwardAxis == null) ? 0f : inputForwardAxis.action.ReadValue<Vector2>().y;
            objDistance = Mathf.Clamp(objDistance + (_forward * forwardMultiplier),0f,rayLength);
            
            if (_dist > 0.0125f)
            {
                selectedBody.velocity = Vector3.Lerp(selectedBody.velocity, Vector3.zero, .0125f);
                selectedBody.angularVelocity = Vector3.Lerp(selectedBody.angularVelocity, Vector3.zero, .125f);
                selectedBody.MovePosition(Vector3.Lerp(selectedBody.position, transformTo.position-calculatedOffset, movePositionLerp));
            }
            else
            {
                //selectedBody.transform.position = transformTo.position-calculatedOffset;
                selectedBody.velocity = Vector3.zero;
                selectedBody.angularVelocity = Vector3.zero;
            }


            float distToHand = Vector3.Distance(selectedBody.position, transform.position);

            if (distToHand <= forceGrabDistance)
            {
                if(useLeftHand)
                    autoHandPlayer.handLeft.Grab();//selectedObject.GetComponent<Grabbable>());
                else 
                    autoHandPlayer.handRight.Grab();//selectedObject.GetComponent<Grabbable>());
            }
        }

        if (rotateObject)
        {

            if (ratchet)
            {
                Quaternion delta = transformTo.rotation * Quaternion.Inverse(lastControllerRotation);

                if (lockXZRotation)
                {
                    Vector3 deltaEuler = delta.eulerAngles;
                    //deltaEuler.x = Mathf.Round(deltaEuler.x / 45f) * 45f;
                    //deltaEuler.z = Mathf.Round(deltaEuler.z / 45f) * 45f;
                    //delta = Quaternion.Euler(deltaEuler);


                    delta = Quaternion.Euler(0f, deltaEuler.y, 0f);
                }

                selectedBody.MoveRotation(Quaternion.Slerp(selectedBody.rotation, selectedBody.rotation * delta, .75f));
            }
           else
            {
                if (rotateObject)
                    selectedBody.MoveRotation(Quaternion.Slerp(currentlyPossessedObj.transform.rotation, transformTo.rotation, .125f));
            }
        }



    }

    public void ActivateRay(int playerId, bool isBlue, bool isLeft) {
        Transform target = isBlue ? currentFrameHighlightedObj.transform : currentlyPossessedObj.transform;
        if(target==null) return;
        NetworkPlayerRaycastManager.instance.playerRaycastMap[playerId].ActivateRay(isBlue,isLeft,target);
    }
    
    public void DeactivateRays(int playerId, bool isLeft) {
        NetworkPlayerRaycastManager.instance.playerRaycastMap[playerId].DeactivateRays(isLeft);
    }

    public void ForceSetSelectedObject(XRUserTransformable objectToPossess)
    {
        currentlyPossessedObj = objectToPossess;
    }

    public void ToggleSnapTurn(bool shouldSnapTurn)
    {
        autoHandPlayer.snapTurnAngle = (shouldSnapTurn) ? 30f : 0f;
    }
}
