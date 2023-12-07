using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(MonoFlag))]
[RequireComponent(typeof(JarodPlacePoint))]
public class ToggleMonoFlagOnPlace : MonoBehaviour
{
    JarodPlacePoint placePoint;

    [SerializeField]
    bool trueOnPlace=true;
    [SerializeField]
    bool getFlagOnThisObject = true;
    [DisableIf("getFlagOnThisObject")]
    [SerializeField]
    MonoFlag flag;
    
    void Awake() {
        placePoint = GetComponent<JarodPlacePoint>();
        if(getFlagOnThisObject)
            flag = GetComponent<MonoFlag>();

        if (trueOnPlace)
        {
            placePoint.onPlace.AddListener(() => flag.Set(true));
            placePoint.onRemove.AddListener(() => flag.Set(false));
        }
        else {

            placePoint.onPlace.AddListener(() => flag.Set(false));
            placePoint.onRemove.AddListener(() => flag.Set(true));
        }
    }
}
