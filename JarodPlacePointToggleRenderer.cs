using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JarodPlacePoint))]
public class JarodPlacePointToggleRenderer : MonoBehaviour
{
    [SerializeField]
    GameObject objectToToggle;
    [SerializeField]
    bool disableObjectOnStart=true;

    void Start()
    {
        JarodPlacePoint jpp = GetComponent<JarodPlacePoint>();
        jpp.onPlace.AddListener(() => objectToToggle.SetActive(true));
        jpp.onRemove.AddListener(() => objectToToggle.SetActive(false));

        if(disableObjectOnStart)
            objectToToggle.SetActive(false);
    }
}
