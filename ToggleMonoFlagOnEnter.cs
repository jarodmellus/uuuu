using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(TriggerHandler))]
[RequireComponent(typeof(MonoFlag))]
public class ToggleMonoFlagOnEnter : MonoBehaviour
{
    TriggerHandler trigger;
    MonoFlag flag;
    private void Awake() {
        flag=GetComponent<MonoFlag>();
        trigger=GetComponent<TriggerHandler>();
        trigger.anyColliderEvents.onTriggerEnter.AddListener((col)=>flag.Set(true));
        trigger.anyColliderEvents.onTriggerExit.AddListener((col)=>flag.Set(false));
    }


}
