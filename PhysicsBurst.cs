using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsBurst : MonoBehaviour
{
    public UnityEvent onBurst;
    //[SerializeField]
    Vector3 force;
    Vector3 direction;
    [SerializeField]
    float velocity;
    [SerializeField]
    int steps = 5;
    [SerializeField]
    float time = .5f;
    Rigidbody rigidbody;

    TransformWrapper initialTransform;

    void Start()
    {
        initialTransform = new TransformWrapper(transform.localPosition,transform.localRotation, Vector3.one);
        rigidbody = GetComponent<Rigidbody>();
    }

    [NaughtyAttributes.Button]
    public void Burst() {
        gameObject.SetActive(true);
        transform.localPosition=initialTransform.Position;
        transform.localRotation=initialTransform.Rotation;
        rigidbody.velocity = velocity * transform.forward;
        onBurst?.Invoke();
    }

    void OnDrawGizmos()
    {
        /*
        for(int i = 0; i < steps; i++)
        {
            Debug.DrawLine(transform.position, transform.forward, Color.red, 1f);
        }

        Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
        */

        Vector3[] positions = PreviewTrajectory(transform.position,transform.forward*velocity, Physics.gravity,0f,3f);

        for(int i = 0; i < positions.Length; i++) {
            if(i==0) {
                continue;
            } else if(i==positions.Length-1) {

            } else {
                Debug.DrawRay(positions[i], (positions[i] - positions[i-1]), Color.red,.01f);
            }
            
        }
    }

     // C#
 
 // Vector3 position - The starting position for the launch
 // Vector3 velocity - The initial velocity
 // Vector3 gravity - e.g. Physics.gravity
 // float drag - The "drag" value on the Rigidbody
 // float time - The amount of time to demonstrate in the  trajectory preview
 
 public static Vector3[] PreviewTrajectory(Vector3 position, Vector3 velocity, Vector3 gravity, float drag, float time)
 {
     float timeStep = Time.fixedDeltaTime;
     int iterations = Mathf.CeilToInt(time / timeStep);
     if(iterations < 2)
     {
         Debug.LogError("PreviewTrajectory(Vector3, Vector3, Vector3, float, float): Unable to preview trajectory shorter than Time.fixedDeltaTime * 2");
         return new Vector3[0];
     }
     
     Vector3[] path = new Vector3[iterations];
     
     Vector3 pos = position;
     Vector3 vel = velocity;
     path[0] = pos;
     
     float dragScale = Mathf.Clamp01(1.0f - (drag * timeStep));
     
     for(int i = 1; i < iterations; i++)
     {
         vel = vel + (gravity * timeStep);
         vel *= dragScale;
         pos = pos + (vel * timeStep);
         
         path[i] = pos;
     }
     
     return path;
 }
}
