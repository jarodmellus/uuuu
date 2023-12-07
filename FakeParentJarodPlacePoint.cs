using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(JarodPlacePoint))]
public class FakeParentJarodPlacePoint : MonoBehaviour
{
    JarodPlacePoint jpp;
    bool doFollow = true;

    void Awake()
    {
        jpp = GetComponent<JarodPlacePoint>();
        
  
        jpp.onPlace.AddListener(() => doFollow = true);
        jpp.onRemove.AddListener(() => doFollow = false);
    }
/*
    void OnEnable()
    {
        jpp.onPlace.AddListener(() => doFollow = true);
        jpp.onRemove.AddListener(() => doFollow = false);
        jpp.Place
    }

    void OnDisable()
    {
        jpp.onPlace.RemoveListener(() => doFollow = true);
        jpp.onRemove.RemoveListener(() => doFollow = false);
    }
    */


    GameObject target;
    void Update() {
        if(!doFollow) return;
        if((target=jpp.GetPlacedObject)==null) return;

        target.transform.position = transform.position;
        target.transform.rotation = transform.rotation;

    }

}
