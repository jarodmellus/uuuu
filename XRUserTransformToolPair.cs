using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XRUserTransformToolPair : MonoBehaviour
{
    [SerializeField]
    public XRUserTransformObjectTool leftTool, rightTool;

    bool leftPossessed;
    bool rightPossessed;

    XRUserTransformable objectToScale;
    float activeScale = 1f;
    float currentDistanceBetween;
    float initDistanceBetween;

    [SerializeField]
    bool allowScale = false;

    void Start()
    {
        if (allowScale)
        {
            leftTool.onPossess.AddListener((obj) => CheckIfShouldScale());
            rightTool.onPossess.AddListener((obj) => CheckIfShouldScale());
            leftTool.onFree.AddListener((obj) => CheckIfShouldScale());
            rightTool.onFree.AddListener((obj) => CheckIfShouldScale());
        }

        enabled = false;
    }

    void CheckIfShouldScale() {
        if(leftTool.GetCurrentlyPossessedObject==null || rightTool.GetCurrentlyPossessedObject == null ) {
            if(enabled) {
                if(leftTool.GetCurrentlyPossessedObject!=null)
                    leftTool.FreeObject();
                if(rightTool.GetCurrentlyPossessedObject!=null)
                    rightTool.FreeObject();
            }
            enabled = false;
            //leftTool.FreeObject();
            //rightTool.FreeObject();
            leftTool.userScaling = false;
            rightTool.userScaling = false;
            return;
        }

        if(leftTool.GetCurrentlyPossessedObject!=rightTool.GetCurrentlyPossessedObject) return;

        objectToScale = leftTool.GetCurrentlyPossessedObject;
        objectToScale.GetBody.isKinematic = true;
        activeScale = objectToScale.transform.localScale.x;
        initDistanceBetween = Vector3.Distance(leftTool.transform.position,rightTool.transform.position);
        leftTool.userScaling = true;
        rightTool.userScaling = true;
        enabled = true;
    }

    void Update() {
        ObjectScale();
    }
    
    void ObjectScale() {
        currentDistanceBetween = Vector3.Distance(leftTool.transform.position,rightTool.transform.position);
        float newScale = Mathf.Clamp(activeScale * (currentDistanceBetween / initDistanceBetween),objectToScale.userScaleRange.x,objectToScale.userScaleRange.y);
        objectToScale.transform.localScale = Vector3.one * newScale;

    }
}
