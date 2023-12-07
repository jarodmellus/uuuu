using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnHeadsetLook : MonoBehaviour
{
    [SerializeField]
    float maxDistance = 5f;
    [SerializeField]
    float radius = .5f;
    int bufferDurationFrames = 2;
    bool inBuffer = false;
    int bufferFrame;
    Collider col;
    public UnityEvent onHeadsetLook, onHeadsetUnlook;

    bool wasLookedAt=false,currentlyLookedAt=false;

    void OnBecameInvisible()
    {
        enabled = false;
    }

    void OnBecameVisible()
    {
        enabled = true;
    }

    void FixedUpdate()
    {
        if(Vector3.Distance(transform.position, HeadsetLook.instance.transform.position) > maxDistance) return;

        //allow time between entering and exiting so we don't accidentally re-enter or re-exit
        if(inBuffer) {
            bufferFrame++;
            if(bufferFrame>=bufferDurationFrames) inBuffer = false;
            return;
        }

        Collider[] cols = Physics.OverlapSphere(transform.position,radius,~0,QueryTriggerInteraction.Collide);

        currentlyLookedAt = false;
        HeadsetLook headsetLook;
        foreach(Collider c in cols) {
            if(!c.TryGetComponent<HeadsetLook>(out headsetLook)) continue;
            currentlyLookedAt = true;
            break;
        }

        if(currentlyLookedAt) {
            if (!wasLookedAt)
            {
                onHeadsetLook?.Invoke();
                EnterBuffer();
            }
        }
        else {
            if (wasLookedAt)
            {
                onHeadsetUnlook?.Invoke();
                EnterBuffer();
            }
        }

        wasLookedAt = currentlyLookedAt;

    }

    private void EnterBuffer()
    {
        inBuffer = true;
        bufferFrame = 0;
    }

    void OnDrawGizmos()
    { 
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(transform.position,radius);
    }
    
}
