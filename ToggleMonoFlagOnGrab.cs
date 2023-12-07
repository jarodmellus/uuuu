using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Autohand;

[RequireComponent(typeof(MonoFlag))]
[RequireComponent(typeof(Grabbable))]
[RequireComponent(typeof(GrabbableExtraEvents))]
public class ToggleMonoFlagOnGrab : MonoBehaviour
{
    MonoFlag flag;
    Grabbable grab;
    GrabbableExtraEvents extraEvents;
    void Awake() {
        flag = GetComponent<MonoFlag>();
        grab = GetComponent<Grabbable>();
        extraEvents = GetComponent<GrabbableExtraEvents>();
    }

    void Start() {
        extraEvents.OnFirstGrab.AddListener((hand,grab) => flag.Set(true));
        extraEvents.OnLastRelease.AddListener((hand, grab) => flag.Set(false));
    }
}
