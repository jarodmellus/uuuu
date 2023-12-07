using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Autohand;
using NaughtyAttributes;
using UnityEngine.Events;

public class ToolBelt : MonoBehaviour
{
    
    [SerializeField]
    InputActionReference toggleBeltAction;
    [SerializeField]
    GameObject beltObj;
    bool beltActive = false;
    Transform trackerOffsets;
    [SerializeField]
    Vector3 positionOffset, rotationOffset;
    public UnityEvent onResetTools;

    public UnityEvent onActivate, onDeactivate;

    Camera headCam;

    Grabbable[] _childGrabbables;

    void Start()
    {
        headCam = AutoHandPlayer.Instance.headCamera;
        trackerOffsets = AutoHandPlayer.Instance.trackingContainer;
        //transform.SetParent(headCam.transform);
        //transform.localPosition = positionOffset;
        //transform.LookAt(AutoHandPlayer.Instance.transform);
        
        
    }

    void OnEnable()
    {
        toggleBeltAction.action.Enable();
        toggleBeltAction.action.performed += (ctx) => ToggleBelt();
        
    }

    void OnDisable()
    {
        toggleBeltAction.action.Disable();
        toggleBeltAction.action.performed -= (ctx) => ToggleBelt();
    }


    [Button]
    void ToggleBelt() {
        beltActive=!beltActive;

        if(beltActive) {
            beltObj.SetActive(true);
           // transform.position = trackerOffsets.TransformPoint(positionOffset);
            //transform.LookAt(AutoHandPlayer.Instance.transform);
            //transform.localRotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);
            //transform.Rotate(rotationOffset,Space.Self);
            onActivate?.Invoke();
        } else {
            onDeactivate?.Invoke();
        }
    }

    public void ResetTools() {
        onResetTools?.Invoke();
    }

    Vector3 ConstrainedForward(Transform focus)
    {
        // Get the natural forward transition
        Vector3 naturalForward = focus.forward;
        // Nullify its y-component
        naturalForward.y = 0;
        // Normalize xz-plane and align it with the chosen y-axis
        Vector3 fixedForward = naturalForward.normalized;

        // This gives us a vector whose xz-plane us normalized
        // while preserving the fixed Y component
        return fixedForward;
    }

    [SerializeField] float AlignmentTolerance = 0.7f;
    Vector3 AverageHandForward;

    void Update()
    {
        AverageHandForward = AutoHandPlayer.Instance.handLeft.transform.forward.normalized + AutoHandPlayer.Instance.handRight.transform.forward.normalized;
        AverageHandForward.y = 0;

        Vector3 NormalizedHead = ConstrainedForward(headCam.transform).normalized;

        bool HeadAndHandsAligned = IsFacingSameDirection(AverageHandForward, NormalizedHead, AlignmentTolerance);

        transform.position = headCam.transform.position;
        transform.forward = NormalizedHead;

        //if(HeadAndHandsAligned)
        //    transform.forward = NormalizedHead;

    }

    static public bool IsFacingSameDirection(Vector3 FirstVector, Vector3 SecondVector, float Tolerance)
    {
        bool Result = false;
        float FinalDot = Vector3.Dot(SecondVector, FirstVector);

        if (FinalDot > Tolerance)
        {
            Result = true;
        }

        return Result;
    }

    private Vector3 GetMidpoint(Vector3 Start, Vector3 End)
    {
        Vector3 Result;
        Result = Vector3.Lerp(Start, End, 0.5f);
        return Result;
    }


}
