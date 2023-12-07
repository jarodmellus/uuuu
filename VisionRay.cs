using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Accessibility;

public class VisionRay : MonoBehaviour
{
    AnimalVision animalVision;
    [SerializeField]
    float visionDistance=4f;
    void FixedUpdate()
    {
        Physics.Raycast(transform.position,transform.forward,visionDistance);
    }

    void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position,transform.forward*visionDistance,Color.red);
    }

    void Awake()
    {
        animalVision = GetComponentInParent<AnimalVision>();
        animalVision.AddRay(this);
    }
}
